using Newtonsoft.Json;
using Slip.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;

namespace Slip.Utility
{
    // Pushes a MST_RoughDistribution split to ITS OWN branch's standalone API
    // only (BranchID 1 -> BranchApiBranch1Url, BranchID 2 -> BranchApiBranch2Url,
    // etc. in Web.config) - each branch's API holds only that branch's entries,
    // not the other branch's. Uses DbHelper for all DB access per the project
    // convention.
    public static class BranchApiClient
    {
        private class CachedToken
        {
            public string Token;
            public DateTime ExpiresAtUtc;
        }

        private static readonly Dictionary<string, CachedToken> _tokenCache = new Dictionary<string, CachedToken>();
        private static readonly object _tokenLock = new object();

        private static string GetBranchApiBaseUrl(int branchID)
        {
            string url = ConfigurationManager.AppSettings["BranchApiBranch" + branchID + "Url"];
            return string.IsNullOrWhiteSpace(url) ? null : url.TrimEnd('/');
        }

        // Never throws - a Branch API being down must not block or roll back the
        // local RoughDistribution save. Every failure is logged via ErrorLogger
        // instead.
        public static void SyncDistribution(int distributionID)
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("USP_MST_RoughDistribution_GetForBranchSync",
                    new SqlParameter("@DistributionID", distributionID));

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                DataRow row = ds.Tables[0].Rows[0];
                int branchID = Convert.ToInt32(row["BranchID"]);
                string baseUrl = GetBranchApiBaseUrl(branchID);

                if (string.IsNullOrEmpty(baseUrl))
                {
                    ErrorLogger.ErrorLog("Branch API sync skipped for DistributionID=" + distributionID +
                        ": no API URL configured for BranchID=" + branchID + " (BranchApiBranch" + branchID + "Url).");
                    return;
                }

                BranchRoughSyncRequest payload = BuildPayload(row);

                try
                {
                    PostRough(baseUrl, payload);
                }
                catch (Exception ex)
                {
                    ErrorLogger.ErrorLog("Branch API sync failed for DistributionID=" + distributionID +
                        " (RCode=" + payload.RCode + ", BranchID=" + branchID + ") -> " + baseUrl + ": " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
        }

        // Never throws - a Branch API being down must not block or roll back the
        // local Lot save. Every failure is logged via ErrorLogger instead.
        // Sibling to SyncDistribution, used by the new direct Inward -> Lot
        // split flow (there's no MST_RoughDistribution row to sync from
        // there - MST_Lot itself carries BranchID directly).
        public static void SyncLot(int lotID)
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("USP_MST_Lot_GetForBranchSync",
                    new SqlParameter("@LotID", lotID));

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                DataRow row = ds.Tables[0].Rows[0];
                if (row["BranchID"] == DBNull.Value)
                {
                    // No Branch assigned yet (split saved branch-less) - nothing
                    // to sync to until one is set via a later edit.
                    return;
                }
                int branchID = Convert.ToInt32(row["BranchID"]);
                string baseUrl = GetBranchApiBaseUrl(branchID);

                if (string.IsNullOrEmpty(baseUrl))
                {
                    ErrorLogger.ErrorLog("Branch API sync skipped for LotID=" + lotID +
                        ": no API URL configured for BranchID=" + branchID + " (BranchApiBranch" + branchID + "Url).");
                    return;
                }

                BranchRoughSyncRequest payload = BuildPayload(row);

                try
                {
                    PostRough(baseUrl, payload);
                }
                catch (Exception ex)
                {
                    ErrorLogger.ErrorLog("Branch API sync failed for LotID=" + lotID +
                        " (RCode=" + payload.RCode + ", BranchID=" + branchID + ") -> " + baseUrl + ": " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
        }

        // Calls Branch 1's AnjaliLabTech.API (https://localhost:7008, same
        // service already used above) at GET /api/Inward/FetchOldInwardData
        // and returns the raw JSON response body. Unlike SyncDistribution/
        // SyncLot this is a foreground, user-initiated action (the Lot Master
        // "Reset & Reload Old Data" button) - it throws on failure instead of
        // swallowing it, so the caller can show the user what went wrong.
        public static string FetchOldInwardDataJson()
        {
            string baseUrl = GetBranchApiBaseUrl(1);
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("No API URL configured for BranchID=1 (BranchApiBranch1Url).");
            }

            string token = GetToken(baseUrl, forceRefresh: false);
            try
            {
                return GetJson(baseUrl + "/api/Inward/FetchOldInwardData", token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                return GetJson(baseUrl + "/api/Inward/FetchOldInwardData", token);
            }
        }

        // Master Book row-menu "CLV Chart": calls Branch 1's AnjaliLabTech.API
        // at GET /api/Report/GetAssortmentChart?Kapan=...&Type=CLV (same
        // service as FetchOldInwardDataJson above). That endpoint is
        // [Authorize(Roles = "Admin")], not just [Authorize] - relies on the
        // BranchApiUsername account ("Admin") actually carrying the Admin
        // role claim, same as it already must for other admin-only calls.
        public static string GetAssortmentChartJson(string kapan, string type)
        {
            string baseUrl = GetBranchApiBaseUrl(1);
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("No API URL configured for BranchID=1 (BranchApiBranch1Url).");
            }

            string path = "/api/Report/GetAssortmentChart?Kapan=" + Uri.EscapeDataString(kapan ?? "") + "&Type=" + Uri.EscapeDataString(type ?? "");

            string token = GetToken(baseUrl, forceRefresh: false);
            try
            {
                return GetJson(baseUrl + path, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                return GetJson(baseUrl + path, token);
            }
        }

        private static string GetJson(string url, string bearerToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 600000; // 10 min - this can return years of historical data from a ~200-column legacy table, much longer than the inline sync calls above
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers["Authorization"] = "Bearer " + bearerToken;
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private static BranchRoughSyncRequest BuildPayload(DataRow row)
        {
            return new BranchRoughSyncRequest
            {
                RCode = ToStr(row["RCode"]),
                RName = ToStr(row["RName"]),
                RoughType = ToStr(row["RoughType"]),
                Pcs = ToInt(row["Pcs"]),
                Carat = ToDbl(row["Carat"]),
                OriginalCarat = ToDbl(row["OriginalCarat"]),
                Rate = ToDbl(row["Rate"]),
                Remarks = ToStr(row["Remarks"]),
                IsComplete = row["IsComplete"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["IsComplete"]),
                Height = ToDbl(row["Height"]),
                HeightName = ToStr(row["HeightName"]),
                Size = ToStr(row["Size"]),
                SizeCodes = ToStr(row["SizeCodes"]),
                ColorType = ToStr(row["ColorType"]),
                MachineNo = ToStr(row["MachineNo"]),
                Hours = ToDbl(row["Hours"]),
                GrowthRate = ToStr(row["GrowthRate"]),
                Receipe = ToStr(row["Receipe"]),
                FactoryCode = ToStr(row["FactoryCode"]),
                InvoiceNo = ToStr(row["InvoiceNo"]),
                Party = ToStr(row["Party"]),
                LotCount = ToInt(row["LotCount"])
            };
        }

        private static string ToStr(object v) => v == null || v == DBNull.Value ? null : Convert.ToString(v);
        private static int? ToInt(object v) => v == null || v == DBNull.Value ? (int?)null : Convert.ToInt32(v);
        private static double? ToDbl(object v) => v == null || v == DBNull.Value ? (double?)null : Convert.ToDouble(v);

        private static string GetToken(string baseUrl, bool forceRefresh)
        {
            if (!forceRefresh)
            {
                lock (_tokenLock)
                {
                    if (_tokenCache.TryGetValue(baseUrl, out CachedToken cached) && cached.ExpiresAtUtc > DateTime.UtcNow.AddMinutes(1))
                    {
                        return cached.Token;
                    }
                }
            }

            string username = ConfigurationManager.AppSettings["BranchApiUsername"];
            string password = ConfigurationManager.AppSettings["BranchApiPassword"];

            string loginBody = JsonConvert.SerializeObject(new BranchLoginRequest { UserName = username, Password = password });
            string responseJson = PostJson(baseUrl + "/api/Auth/login", loginBody, null);

            BranchLoginResponse loginResponse = JsonConvert.DeserializeObject<BranchLoginResponse>(responseJson);
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                throw new Exception("Branch API login did not return a token.");
            }

            lock (_tokenLock)
            {
                _tokenCache[baseUrl] = new CachedToken { Token = loginResponse.Token, ExpiresAtUtc = loginResponse.ExpiresAt.ToUniversalTime() };
            }

            return loginResponse.Token;
        }

        private static void PostRough(string baseUrl, BranchRoughSyncRequest payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            string token = GetToken(baseUrl, forceRefresh: false);

            try
            {
                PostJson(baseUrl + "/api/Rough", body, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Cached token rejected (e.g. expired earlier than our local clock
                // thought) - log in again once and retry before giving up.
                token = GetToken(baseUrl, forceRefresh: true);
                PostJson(baseUrl + "/api/Rough", body, token);
            }
        }

        private static string PostJson(string url, string jsonBody, string bearerToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 10000; // short timeout - this runs inline on the save request and must not hang the UI
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers["Authorization"] = "Bearer " + bearerToken;
            }

            byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
            request.ContentLength = bodyBytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bodyBytes, 0, bodyBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
