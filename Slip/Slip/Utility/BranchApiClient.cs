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
using System.Text.RegularExpressions;

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

        // Every Branch{N} with a configured BranchApiBranch{N}Url in
        // Web.config - what "all branches" means for SyncRoughToAllBranches/
        // SyncLotToAllBranches below. Scanning AppSettings keys (rather than
        // hardcoding 1/2) means a future Branch 3 just needs a config entry,
        // no code change.
        private static List<int> GetConfiguredBranchIDs()
        {
            var ids = new List<int>();
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                Match match = Regex.Match(key ?? "", @"^BranchApiBranch(\d+)Url$");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int branchID))
                {
                    ids.Add(branchID);
                }
            }
            return ids;
        }

        // Every Kapan (TRN_RoughInwardDetail) and every Lot (MST_Lot) is
        // inserted into EVERY configured branch's own AnjaliLabTech.API
        // instance separately and unconditionally - each branch is a fully
        // separate API/database, so each needs its own RID/LotID, tracked in
        // TRN_RoughInwardDetail_BranchRough / MST_Lot_BranchSync. After that
        // initial insert, only Branch assignment, Department, Head Manager
        // and IsReceived get re-pushed on change - but since the underlying
        // MST_Lot_Insert_Update_Delete UPDATE branch overwrites columns
        // unconditionally (no ISNULL-merge), re-pushing means resending the
        // FULL current payload, not just those 4 fields - see
        // SyncLotToAllBranches.

        // Ensures InwardDetailID's Kapan exists in BranchID's own Rough
        // table, inserting it if not already tracked. Never throws.
        private static void EnsureRoughSyncedToBranch(int inwardDetailID, int branchID, int createdBy)
        {
            try
            {
                DataSet trackDs = DbHelper.ExecuteDataSet("USP_TRN_RoughInwardDetail_BranchRough_Get",
                    new SqlParameter("@InwardDetailID", inwardDetailID),
                    new SqlParameter("@BranchID", branchID));

                if (trackDs.Tables.Count > 0 && trackDs.Tables[0].Rows.Count > 0)
                {
                    return;
                }

                string baseUrl = GetBranchApiBaseUrl(branchID);
                if (string.IsNullOrEmpty(baseUrl))
                {
                    return;
                }

                DataSet ds = DbHelper.ExecuteDataSet("USP_TRN_RoughInwardDetail_GetForBranchSync",
                    new SqlParameter("@InwardDetailID", inwardDetailID));

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                BranchRoughSyncRequest payload = BuildPayload(ds.Tables[0].Rows[0]);
                int rid = PostRoughGetRid(baseUrl, payload);

                if (rid > 0)
                {
                    DbHelper.ExecuteNonQuery("USP_TRN_RoughInwardDetail_BranchRough_Insert",
                        new SqlParameter("@InwardDetailID", inwardDetailID),
                        new SqlParameter("@BranchID", branchID),
                        new SqlParameter("@ApiRoughID", rid),
                        new SqlParameter("@CreatedBy", createdBy));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog("Rough branch sync failed for InwardDetailID=" + inwardDetailID +
                    ", BranchID=" + branchID + ": " + ex.Message);
            }
        }

        // Call once, right after a new Kapan line (TRN_RoughInwardDetail) is
        // created - pushes it to every configured branch.
        public static void SyncRoughToAllBranches(int inwardDetailID, int createdBy = 1)
        {
            try
            {
                foreach (int branchID in GetConfiguredBranchIDs())
                {
                    EnsureRoughSyncedToBranch(inwardDetailID, branchID, createdBy);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
        }

        // Call after a Rough Inward edit - re-pushes to every branch this
        // Kapan line has already been synced to (branches it hasn't reached
        // yet have nothing to update).
        public static void SyncRoughUpdateToAllBranches(int inwardDetailID)
        {
            try
            {
                DataSet trackDs = DbHelper.ExecuteDataSet("USP_TRN_RoughInwardDetail_BranchRough_GetForDetail",
                    new SqlParameter("@InwardDetailID", inwardDetailID));

                if (trackDs.Tables.Count == 0 || trackDs.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                DataSet ds = DbHelper.ExecuteDataSet("USP_TRN_RoughInwardDetail_GetForBranchSync",
                    new SqlParameter("@InwardDetailID", inwardDetailID));

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                BranchRoughSyncRequest payload = BuildPayload(ds.Tables[0].Rows[0]);

                foreach (DataRow trackRow in trackDs.Tables[0].Rows)
                {
                    int branchID = Convert.ToInt32(trackRow["BranchID"]);
                    string baseUrl = GetBranchApiBaseUrl(branchID);
                    if (string.IsNullOrEmpty(baseUrl))
                    {
                        continue;
                    }

                    try
                    {
                        PutRough(baseUrl, payload);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.ErrorLog("Rough update sync failed for InwardDetailID=" + inwardDetailID +
                            ", BranchID=" + branchID + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
        }

        // The single Lot sync entry point - call after ANY change that
        // should reach the branch copies: new Lot, Branch assignment,
        // Department change, Head Manager change. Inserts into any branch
        // not yet tracked, updates (full payload) every branch already
        // tracked. No-ops entirely once the Lot's local IsReceived is set -
        // callers should already be blocking the edit itself in that case
        // (see MasterController's IsReceived gate), this is a second line of
        // defense.
        public static void SyncLotToAllBranches(int lotID, int createdBy = 1)
        {
            try
            {
                DataSet lotDs = DbHelper.ExecuteDataSet("USP_MST_Lot_GetForBranchSync", new SqlParameter("@LotID", lotID));
                if (lotDs.Tables.Count == 0 || lotDs.Tables[0].Rows.Count == 0)
                {
                    return;
                }

                DataRow lotRow = lotDs.Tables[0].Rows[0];
                if (lotRow["IsReceived"] != DBNull.Value && Convert.ToBoolean(lotRow["IsReceived"]))
                {
                    return;
                }

                int inwardDetailID = Convert.ToInt32(lotRow["InwardDetailID"]);
                BranchLotSyncRequest payload = BuildLotPayload(lotRow);

                var existingBranches = new Dictionary<int, int>();
                DataSet trackDs = DbHelper.ExecuteDataSet("USP_MST_Lot_BranchSync_GetForLot", new SqlParameter("@LotID", lotID));
                if (trackDs.Tables.Count > 0)
                {
                    foreach (DataRow r in trackDs.Tables[0].Rows)
                    {
                        existingBranches[Convert.ToInt32(r["BranchID"])] = Convert.ToInt32(r["ApiLotID"]);
                    }
                }

                foreach (int branchID in GetConfiguredBranchIDs())
                {
                    string baseUrl = GetBranchApiBaseUrl(branchID);
                    if (string.IsNullOrEmpty(baseUrl))
                    {
                        continue;
                    }

                    try
                    {
                        EnsureRoughSyncedToBranch(inwardDetailID, branchID, createdBy);

                        DataSet ridDs = DbHelper.ExecuteDataSet("USP_TRN_RoughInwardDetail_BranchRough_Get",
                            new SqlParameter("@InwardDetailID", inwardDetailID),
                            new SqlParameter("@BranchID", branchID));
                        payload.RoughId = (ridDs.Tables.Count > 0 && ridDs.Tables[0].Rows.Count > 0)
                            ? (int?)Convert.ToInt32(ridDs.Tables[0].Rows[0]["ApiRoughID"])
                            : null;

                        if (existingBranches.TryGetValue(branchID, out int apiLotID))
                        {
                            payload.LotId = apiLotID;
                            PutLot(baseUrl, payload);
                        }
                        else
                        {
                            payload.LotId = null;
                            int newApiLotID = PostLotGetId(baseUrl, payload);
                            if (newApiLotID > 0)
                            {
                                DbHelper.ExecuteNonQuery("USP_MST_Lot_BranchSync_Insert",
                                    new SqlParameter("@LotID", lotID),
                                    new SqlParameter("@BranchID", branchID),
                                    new SqlParameter("@ApiLotID", newApiLotID),
                                    new SqlParameter("@CreatedBy", createdBy));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.ErrorLog("Lot branch sync failed for LotID=" + lotID +
                            ", BranchID=" + branchID + ": " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
        }

        // Call after a Lot is (soft-)deleted locally - removes it from every
        // branch it had been synced to, then clears the tracking rows.
        public static void DeleteLotFromAllBranches(int lotID)
        {
            try
            {
                DataSet trackDs = DbHelper.ExecuteDataSet("USP_MST_Lot_BranchSync_GetForLot", new SqlParameter("@LotID", lotID));
                if (trackDs.Tables.Count > 0)
                {
                    foreach (DataRow r in trackDs.Tables[0].Rows)
                    {
                        int branchID = Convert.ToInt32(r["BranchID"]);
                        int apiLotID = Convert.ToInt32(r["ApiLotID"]);
                        string baseUrl = GetBranchApiBaseUrl(branchID);
                        if (string.IsNullOrEmpty(baseUrl))
                        {
                            continue;
                        }

                        try
                        {
                            DeleteLotApi(baseUrl, apiLotID);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.ErrorLog("Lot delete sync failed for LotID=" + lotID +
                                ", BranchID=" + branchID + ": " + ex.Message);
                        }
                    }
                }

                DbHelper.ExecuteNonQuery("USP_MST_Lot_BranchSync_DeleteForLot", new SqlParameter("@LotID", lotID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
        }

        private static BranchLotSyncRequest BuildLotPayload(DataRow row)
        {
            return new BranchLotSyncRequest
            {
                LotCode = ToStr(row["LotCode"]),
                LotPcs = ToInt(row["Pcs"]),
                LotCarat = ToDbl(row["Carat"]),
                Remarks = ToStr(row["Remarks"]),
                ColorType = ToStr(row["ColorType"]),
                FactoryCode = ToStr(row["FactoryCode"]),
                MachineNo = ToStr(row["MachineNo"]),
                Hours = ToDbl(row["Hours"]),
                Height = ToDbl(row["Height"]),
                Size = ToStr(row["Size"]),
                Receipe = ToStr(row["Receipe"]),
                SizeCode = ToStr(row["SizeCodes"]),
                RoughRate = ToDbl(row["Rate"]),
                GrowthRate = ToStr(row["GrowthRate"]),
                RoughType = ToStr(row["RoughType"]),
                HeightName = ToStr(row["HeightName"]),
                BranchId = row["BranchID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["BranchID"]),
                TableNo = ToStr(row["TableNo"]),
                HeadManagerId = row["HeadManagerID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["HeadManagerID"]),
                IsReceived = row["IsReceived"] == DBNull.Value ? (int?)null : (Convert.ToBoolean(row["IsReceived"]) ? 1 : 0),
                InwardDetailId = Convert.ToInt32(row["InwardDetailID"]),
                RCodeYear = row["RCodeYear"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["RCodeYear"]),
                RCodeSerial = row["RCodeSerial"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["RCodeSerial"])
            };
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

        private static int PostRoughGetRid(string baseUrl, BranchRoughSyncRequest payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            string token = GetToken(baseUrl, forceRefresh: false);
            string responseJson;
            try
            {
                responseJson = SendJson("POST", baseUrl + "/api/Rough", body, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                responseJson = SendJson("POST", baseUrl + "/api/Rough", body, token);
            }

            var result = JsonConvert.DeserializeAnonymousType(responseJson, new { rid = 0 });
            return result?.rid ?? 0;
        }

        private static void PutRough(string baseUrl, BranchRoughSyncRequest payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            string token = GetToken(baseUrl, forceRefresh: false);
            try
            {
                SendJson("PUT", baseUrl + "/api/Rough", body, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                SendJson("PUT", baseUrl + "/api/Rough", body, token);
            }
        }

        private static int PostLotGetId(string baseUrl, BranchLotSyncRequest payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            string token = GetToken(baseUrl, forceRefresh: false);
            string responseJson;
            try
            {
                responseJson = SendJson("POST", baseUrl + "/api/Lot", body, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                responseJson = SendJson("POST", baseUrl + "/api/Lot", body, token);
            }

            var result = JsonConvert.DeserializeAnonymousType(responseJson, new { lotId = 0 });
            return result?.lotId ?? 0;
        }

        private static void PutLot(string baseUrl, BranchLotSyncRequest payload)
        {
            string body = JsonConvert.SerializeObject(payload);
            string token = GetToken(baseUrl, forceRefresh: false);
            try
            {
                SendJson("PUT", baseUrl + "/api/Lot", body, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                SendJson("PUT", baseUrl + "/api/Lot", body, token);
            }
        }

        private static void DeleteLotApi(string baseUrl, int apiLotID)
        {
            string token = GetToken(baseUrl, forceRefresh: false);
            try
            {
                SendJson("DELETE", baseUrl + "/api/Lot/" + apiLotID, null, token);
            }
            catch (WebException wex) when (wex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = GetToken(baseUrl, forceRefresh: true);
                SendJson("DELETE", baseUrl + "/api/Lot/" + apiLotID, null, token);
            }
        }

        // Generic PUT/DELETE/POST sender (PostJson above stays dedicated to
        // the original POST /api/Rough call path to avoid touching proven,
        // already-in-production behavior) - used by the new Rough-update/
        // Lot-insert/update/delete calls, which need verbs PostJson doesn't
        // support.
        private static string SendJson(string method, string url, string jsonBody, string bearerToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Timeout = 10000; // short timeout - runs inline on the save request and must not hang the UI
            if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers["Authorization"] = "Bearer " + bearerToken;
            }

            if (jsonBody != null)
            {
                request.ContentType = "application/json";
                byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                request.ContentLength = bodyBytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                }
            }
            else if (method == "DELETE")
            {
                request.ContentLength = 0;
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
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
