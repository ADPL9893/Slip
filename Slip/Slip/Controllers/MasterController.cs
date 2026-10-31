using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Slip.Models;
using Slip.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using ZXing;
using ZXing.Common;
using static Slip.Models.Filter.SessionExpireFilter;
namespace Slip.Controllers
{
    public class MasterController : BaseController
    {
        // GET: Master
        public ActionResult Lot()
        {
            return View();
        }

        public ActionResult LotEntry(int id = 0)
        {
            ViewBag.LotID = id;
            return View();
        }

        // Separate page from LotEntry - manages Branch/Department/Head
        // Manager assignment across every Kapan (Lot) split from one Inward
        // Detail row, via an editable, checkbox-select grid.
        public ActionResult LotBranchSplit(int InwardDetailID = 0)
        {
            ViewBag.InwardDetailID = InwardDetailID;
            return View();
        }

        public JsonResult GetData_For_MST_Lot()
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_Lot_GetList");
                var jsonResult = Json(new { success = true, List = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new { success = false, List = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        [HttpGet]
        public JsonResult Get_MST_LotByID(int id)
        {
            try
            {
                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_MST_Lot_GetByID", new SqlParameter("@LotID", id));
                var item = rows.FirstOrDefault();
                var jsonResult = Json(new { success = item != null, item = item }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Get_MST_RoughDistributionListForLot(int BranchID)
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_RoughDistribution_GetListForLot", new SqlParameter("@BranchID", BranchID));
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, list = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Get_MST_DepartmentList(int BranchID)
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_Department_GetList", new SqlParameter("@BranchID", BranchID));
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, list = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Every active department across every Branch, for the Kapan split
        // grid's per-row Department editor (filters client-side by each
        // row's own Branch, since the grid can have many different branches
        // on screen at once).
        [HttpGet]
        public JsonResult Get_MST_DepartmentList_All()
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_Department_GetAllList");
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, list = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Get_MST_HeadManagerList()
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_Employee_HeadList");
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, list = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Get_TRN_RoughInwardDetailListForLotSplit()
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_TRN_RoughInwardDetail_GetListForLotSplit");
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, list = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Batch-saves every staged split line for one Inward Detail balance row
        // in a single transaction and inserts each Lot. R.Code/Lot Code are
        // deliberately NOT generated here anymore - a Lot is created with no
        // Branch and no R.Code, and only gets its R.Code/Lot Code the first
        // time a Branch is assigned to it (see Lot_Save below). Pushes each
        // newly created Lot to its branch's standalone API afterwards (a
        // branch-less split has nowhere to sync to yet, so that's a no-op
        // until Lot_Save assigns one).
        [HttpPost]
        public JsonResult LotSplit_Save(List<MST_LotMaster> splits, int InwardDetailID, int SizeCodeID)
        {
            string message;
            bool isSuccess = false;
            List<int> newLotIDs = new List<int>();
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                if (splits == null || splits.Count == 0)
                {
                    return Json(new { success = false, message = "No split lines to save." });
                }

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (var split in splits)
                            {
                                using (SqlCommand insCmd = new SqlCommand("USP_MST_Lot_Insert", con, tran))
                                {
                                    insCmd.CommandType = CommandType.StoredProcedure;
                                    insCmd.Parameters.AddWithValue("@InwardDetailID", InwardDetailID);
                                    insCmd.Parameters.AddWithValue("@BranchID", (object)split.BranchID ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@RCode", DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@RCodeYear", DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@RCodeSerial", DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@LotCode", DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@DistributionPartNo", DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Lot_Pcs", split.Lot_Pcs);
                                    insCmd.Parameters.AddWithValue("@Lot_Carat", split.Lot_Carat);
                                    insCmd.Parameters.AddWithValue("@ValuePerCT", (object)split.ValuePerCT ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@FactoryCodeID", (object)split.FactoryCodeID ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@FactoryCode", (object)split.FactoryCode ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@ColorType", (object)split.ColorType ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@SizeCodeID", (object)split.SizeCodeID ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@SizeCodeType", (object)split.SizeCodeType ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@GradeID", (object)split.GradeID ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Grade", (object)split.Grade ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@LotType", (object)split.LotType ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@OGName", (object)split.OGName ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@MachineNo", (object)split.MachineNo ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Hours", (object)split.Hours ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Height", (object)split.Height ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Size", (object)split.Size ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Receipe", (object)split.Receipe ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@BoxNo", (object)split.BoxNo ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@RoughRate", (object)split.RoughRate ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@LotPurpose", (object)split.LotPurpose ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@LotPriority", (object)split.LotPriority ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@GrowthRate", (object)split.GrowthRate ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@CompareNo", (object)split.CompareNo ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@NewHeight", (object)split.NewHeight ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@CakePcs", (object)split.CakePcs ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@CakeWeight", (object)split.CakeWeight ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@RejPcs", (object)split.RejPcs ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@RejCarat", (object)split.RejCarat ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@OGPcs", (object)split.OGPcs ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@OGCarat", (object)split.OGCarat ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@DepartmentID", (object)split.DepartmentID ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@HeadManagerID", (object)split.HeadManagerID ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@Remarks", (object)split.Remarks ?? DBNull.Value);
                                    insCmd.Parameters.AddWithValue("@IsActive", split.IsActive);
                                    insCmd.Parameters.AddWithValue("@CreatedBy", currentUserId);

                                    using (SqlDataReader insRdr = insCmd.ExecuteReader())
                                    {
                                        insRdr.Read();
                                        bool rowSuccess = Convert.ToBoolean(insRdr["Success"]);
                                        if (!rowSuccess)
                                        {
                                            throw new Exception(Convert.ToString(insRdr["Message"]));
                                        }
                                        newLotIDs.Add(Convert.ToInt32(insRdr["LotID"]));
                                    }
                                }
                            }

                            tran.Commit();
                            isSuccess = true;
                            message = "Lot splits saved successfully.";
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message, ex);
                        }
                    }
                }

                // Push every newly-created split to its OWN branch's standalone API
                // only - BranchApiClient never throws, so a Branch API being
                // down/unreachable can't fail this save.
                foreach (int newLotID in newLotIDs)
                {
                    BranchApiClient.SyncLot(newLotID);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message, newLotIDs = newLotIDs });
        }

        // Updates a Lot. The first time a Branch is assigned to a Lot whose
        // R.Code is still NULL (the "deferred" workflow - Lots are created
        // branch-less and code-less, see LotSplit_Save), this generates its
        // R.Code (now with the branch digit, since the Branch is finally
        // known) and Lot Code before saving, in the same transaction.
        // Re-saving a Lot that already has an R.Code (e.g. changing its
        // Branch later) never regenerates it - it stays exactly as first
        // printed/QR'd.
        [HttpPost]
        public JsonResult Lot_Save(MST_LotMaster model, string Action = "UPDATE")
        {
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                Dictionary<string, object> r;

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            string existingRCode = null;
                            int? inwardDetailID = null;
                            int? sizeCodeID = null;

                            using (SqlCommand getCmd = new SqlCommand(
                                "SELECT RCode, InwardDetailID, SizeCodeID FROM dbo.MST_Lot WHERE LotID = @LotID", con, tran))
                            {
                                getCmd.Parameters.AddWithValue("@LotID", model.LotID);
                                using (SqlDataReader getRdr = getCmd.ExecuteReader())
                                {
                                    if (getRdr.Read())
                                    {
                                        existingRCode = getRdr["RCode"] != DBNull.Value ? Convert.ToString(getRdr["RCode"]) : null;
                                        inwardDetailID = getRdr["InwardDetailID"] != DBNull.Value ? Convert.ToInt32(getRdr["InwardDetailID"]) : (int?)null;
                                        sizeCodeID = getRdr["SizeCodeID"] != DBNull.Value ? Convert.ToInt32(getRdr["SizeCodeID"]) : (int?)null;
                                    }
                                }
                            }

                            object rCodeParam = DBNull.Value, rCodeYearParam = DBNull.Value, rCodeSerialParam = DBNull.Value,
                                   lotCodeParam = DBNull.Value, distributionPartNoParam = DBNull.Value;

                            if (model.BranchID.HasValue && string.IsNullOrEmpty(existingRCode) && inwardDetailID.HasValue && sizeCodeID.HasValue)
                            {
                                string rCode;
                                int rCodeYear, rCodeSerial, distributionPartNo;
                                string lotCode;

                                using (SqlCommand genCmd = new SqlCommand("usp_GenerateRoughCode_ForLot", con, tran))
                                {
                                    genCmd.CommandType = CommandType.StoredProcedure;
                                    genCmd.Parameters.AddWithValue("@SizeID", sizeCodeID.Value);
                                    genCmd.Parameters.AddWithValue("@BranchID", model.BranchID.Value);
                                    genCmd.Parameters.AddWithValue("@InwardDetailID", inwardDetailID.Value);
                                    genCmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);
                                    using (SqlDataReader genRdr = genCmd.ExecuteReader())
                                    {
                                        genRdr.Read();
                                        rCode = Convert.ToString(genRdr["RoughCode"]);
                                        rCodeYear = Convert.ToInt32(genRdr["RCodeYear"]);
                                        rCodeSerial = Convert.ToInt32(genRdr["RCodeSerial"]);
                                    }
                                }

                                using (SqlCommand lotCodeCmd = new SqlCommand("usp_GenerateLotCode", con, tran))
                                {
                                    lotCodeCmd.CommandType = CommandType.StoredProcedure;
                                    lotCodeCmd.Parameters.AddWithValue("@RoughDistributionID", 0);
                                    lotCodeCmd.Parameters.AddWithValue("@RCode", rCode);
                                    using (SqlDataReader lotRdr = lotCodeCmd.ExecuteReader())
                                    {
                                        lotRdr.Read();
                                        lotCode = Convert.ToString(lotRdr["LotCode"]);
                                        distributionPartNo = Convert.ToInt32(lotRdr["DistributionPartNo"]);
                                    }
                                }

                                rCodeParam = rCode;
                                rCodeYearParam = rCodeYear;
                                rCodeSerialParam = rCodeSerial;
                                lotCodeParam = lotCode;
                                distributionPartNoParam = distributionPartNo;
                            }

                            using (SqlCommand updCmd = new SqlCommand("USP_MST_Lot_Update", con, tran))
                            {
                                updCmd.CommandType = CommandType.StoredProcedure;
                                updCmd.Parameters.AddWithValue("@LotID", model.LotID);
                                updCmd.Parameters.AddWithValue("@BranchID", (object)model.BranchID ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@RCode", rCodeParam);
                                updCmd.Parameters.AddWithValue("@RCodeYear", rCodeYearParam);
                                updCmd.Parameters.AddWithValue("@RCodeSerial", rCodeSerialParam);
                                updCmd.Parameters.AddWithValue("@LotCode", lotCodeParam);
                                updCmd.Parameters.AddWithValue("@DistributionPartNo", distributionPartNoParam);
                                updCmd.Parameters.AddWithValue("@Lot_Pcs", model.Lot_Pcs);
                                updCmd.Parameters.AddWithValue("@Lot_Carat", model.Lot_Carat);
                                updCmd.Parameters.AddWithValue("@ValuePerCT", (object)model.ValuePerCT ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@GradeID", (object)model.GradeID ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@Grade", (object)model.Grade ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@LotType", (object)model.LotType ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@OGName", (object)model.OGName ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@MachineNo", (object)model.MachineNo ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@Hours", (object)model.Hours ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@Height", (object)model.Height ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@Size", (object)model.Size ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@Receipe", (object)model.Receipe ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@BoxNo", (object)model.BoxNo ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@RoughRate", (object)model.RoughRate ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@LotPurpose", (object)model.LotPurpose ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@LotPriority", (object)model.LotPriority ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@GrowthRate", (object)model.GrowthRate ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@CompareNo", (object)model.CompareNo ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@NewHeight", (object)model.NewHeight ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@CakePcs", (object)model.CakePcs ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@CakeWeight", (object)model.CakeWeight ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@RejPcs", (object)model.RejPcs ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@RejCarat", (object)model.RejCarat ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@OGPcs", (object)model.OGPcs ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@OGCarat", (object)model.OGCarat ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@DepartmentID", (object)model.DepartmentID ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@HeadManagerID", (object)model.HeadManagerID ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@Remarks", (object)model.Remarks ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                                updCmd.Parameters.AddWithValue("@ModifiedBy", currentUserId);

                                using (SqlDataReader updRdr = updCmd.ExecuteReader())
                                {
                                    r = new Dictionary<string, object>();
                                    if (updRdr.Read())
                                    {
                                        for (int i = 0; i < updRdr.FieldCount; i++)
                                        {
                                            r[updRdr.GetName(i)] = updRdr.GetValue(i);
                                        }
                                    }
                                }
                            }

                            bool rowSuccess = r.ContainsKey("Success") && Convert.ToBoolean(r["Success"]);
                            if (!rowSuccess)
                            {
                                throw new Exception(r.ContainsKey("Message") ? Convert.ToString(r["Message"]) : "ERROR");
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                bool success = r.ContainsKey("Success") && Convert.ToBoolean(r["Success"]);
                string message = r.ContainsKey("Message") ? Convert.ToString(r["Message"]) : "ERROR";

                // Covers "Branch assigned later": a split saved with no Branch
                // only has somewhere to sync to once a Branch is actually set.
                // BranchApiClient never throws, so this can't fail the save.
                if (success && model.BranchID.HasValue)
                {
                    BranchApiClient.SyncLot(model.LotID);
                }

                return Json(new
                {
                    success = success,
                    message = message,
                    LotID = r.ContainsKey("LotID") ? r["LotID"] : null,
                    RCode = r.ContainsKey("RCode") ? r["RCode"] : null,
                    LotCode = r.ContainsKey("LotCode") ? r["LotCode"] : model.LotCode
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) });
            }
        }

        // Breakdown of one Inward Detail row's already-split total (all its
        // non-deleted Lots, assigned or not) by Branch - used by the Lot
        // Entry edit screen to show what % of the split-so-far total each
        // Branch currently holds.
        [HttpGet]
        public JsonResult Get_LotBranchSplitSummary(int InwardDetailID, int? ExcludeLotID = null)
        {
            try
            {
                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_MST_Lot_GetBranchSplitSummary",
                    new SqlParameter("@InwardDetailID", InwardDetailID),
                    new SqlParameter("@ExcludeLotID", (object)ExcludeLotID ?? DBNull.Value));

                decimal totalPcs = 0, totalCarat = 0;
                foreach (var row in rows)
                {
                    totalPcs += row.ContainsKey("Pcs") && row["Pcs"] != DBNull.Value ? Convert.ToDecimal(row["Pcs"]) : 0;
                    totalCarat += row.ContainsKey("Carat") && row["Carat"] != DBNull.Value ? Convert.ToDecimal(row["Carat"]) : 0;
                }

                var list = rows.Select(row =>
                {
                    decimal pcs = row.ContainsKey("Pcs") && row["Pcs"] != DBNull.Value ? Convert.ToDecimal(row["Pcs"]) : 0;
                    decimal carat = row.ContainsKey("Carat") && row["Carat"] != DBNull.Value ? Convert.ToDecimal(row["Carat"]) : 0;
                    return new
                    {
                        BranchID = row.ContainsKey("BranchID") ? row["BranchID"] : null,
                        BranchName = row.ContainsKey("BranchName") ? row["BranchName"] : null,
                        Pcs = pcs,
                        Carat = carat,
                        PcsPercent = totalPcs > 0 ? Math.Round(pcs / totalPcs * 100, 1) : 0,
                        CaratPercent = totalCarat > 0 ? Math.Round(carat / totalCarat * 100, 1) : 0
                    };
                }).ToList();

                return Json(new { success = true, list = list, totalPcs = totalPcs, totalCarat = totalCarat }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // Per-Branch average share of a source's total, over each Branch's
        // last 5 assignments - reference data for the Lot Branch Assignment
        // page (not scoped to any one Inward row).
        //
        // The proc's raw AvgPcsPercent/AvgCaratPercent are each Branch's own
        // average computed independently (its last 5 assignments can come
        // from entirely different source rows than another Branch's last 5),
        // so they don't naturally sum to 100 the way one source row's live
        // split always does. Normalizing here (each branch's raw average /
        // sum of all branches' raw averages * 100) turns it into "if this
        // source were split the way it typically has been, here's roughly
        // the %age each Branch would get" - a distribution that always
        // totals 100, directly comparable to the live split boxes above it.
        [HttpGet]
        public JsonResult Get_LotBranchAvgSplitPercent()
        {
            try
            {
                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_MST_Lot_GetBranchAvgSplitPercent");

                decimal rawPcsTotal = 0, rawCaratTotal = 0;
                foreach (var row in rows)
                {
                    rawPcsTotal += row.ContainsKey("AvgPcsPercent") && row["AvgPcsPercent"] != DBNull.Value ? Convert.ToDecimal(row["AvgPcsPercent"]) : 0;
                    rawCaratTotal += row.ContainsKey("AvgCaratPercent") && row["AvgCaratPercent"] != DBNull.Value ? Convert.ToDecimal(row["AvgCaratPercent"]) : 0;
                }

                var list = rows.Select(row =>
                {
                    decimal rawPcs = row.ContainsKey("AvgPcsPercent") && row["AvgPcsPercent"] != DBNull.Value ? Convert.ToDecimal(row["AvgPcsPercent"]) : 0;
                    decimal rawCarat = row.ContainsKey("AvgCaratPercent") && row["AvgCaratPercent"] != DBNull.Value ? Convert.ToDecimal(row["AvgCaratPercent"]) : 0;
                    return new
                    {
                        BranchID = row.ContainsKey("BranchID") ? row["BranchID"] : null,
                        BranchName = row.ContainsKey("BranchName") ? row["BranchName"] : null,
                        AvgPcsPercent = rawPcsTotal > 0 ? Math.Round(rawPcs / rawPcsTotal * 100, 1) : 0,
                        AvgCaratPercent = rawCaratTotal > 0 ? Math.Round(rawCarat / rawCaratTotal * 100, 1) : 0,
                        SampleCount = row.ContainsKey("SampleCount") ? row["SampleCount"] : 0
                    };
                }).ToList();

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // Every Kapan (Lot) split from one Inward Detail row, for the
        // Lot Branch Assignment grid.
        [HttpGet]
        public JsonResult Get_LotsByInwardDetailID(int InwardDetailID)
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_Lot_GetListByInwardDetailID",
                    new SqlParameter("@InwardDetailID", InwardDetailID));
                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // "Transfer" action for the Kapan split grid - assigns @BranchID
        // (and optionally @DepartmentID/@HeadManagerID, if the user also
        // edited those cells) to one Lot, nothing else. Generates the Lot's
        // R.Code/Lot Code first if this is its first-ever Branch assignment
        // (RCode still NULL), exactly like Lot_Save does.
        [HttpPost]
        public JsonResult AssignLotBranch(int LotID, int BranchID, Guid? DepartmentID = null, int? HeadManagerID = null)
        {
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                Dictionary<string, object> r;

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            string existingRCode = null;
                            int? inwardDetailID = null;
                            int? sizeCodeID = null;

                            using (SqlCommand getCmd = new SqlCommand(
                                "SELECT RCode, InwardDetailID, SizeCodeID FROM dbo.MST_Lot WHERE LotID = @LotID", con, tran))
                            {
                                getCmd.Parameters.AddWithValue("@LotID", LotID);
                                using (SqlDataReader getRdr = getCmd.ExecuteReader())
                                {
                                    if (getRdr.Read())
                                    {
                                        existingRCode = getRdr["RCode"] != DBNull.Value ? Convert.ToString(getRdr["RCode"]) : null;
                                        inwardDetailID = getRdr["InwardDetailID"] != DBNull.Value ? Convert.ToInt32(getRdr["InwardDetailID"]) : (int?)null;
                                        sizeCodeID = getRdr["SizeCodeID"] != DBNull.Value ? Convert.ToInt32(getRdr["SizeCodeID"]) : (int?)null;
                                    }
                                }
                            }

                            object rCodeParam = DBNull.Value, rCodeYearParam = DBNull.Value, rCodeSerialParam = DBNull.Value,
                                   lotCodeParam = DBNull.Value, distributionPartNoParam = DBNull.Value;

                            if (string.IsNullOrEmpty(existingRCode) && inwardDetailID.HasValue && sizeCodeID.HasValue)
                            {
                                string rCode;
                                int rCodeYear, rCodeSerial, distributionPartNo;
                                string lotCode;

                                using (SqlCommand genCmd = new SqlCommand("usp_GenerateRoughCode_ForLot", con, tran))
                                {
                                    genCmd.CommandType = CommandType.StoredProcedure;
                                    genCmd.Parameters.AddWithValue("@SizeID", sizeCodeID.Value);
                                    genCmd.Parameters.AddWithValue("@BranchID", BranchID);
                                    genCmd.Parameters.AddWithValue("@InwardDetailID", inwardDetailID.Value);
                                    genCmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);
                                    using (SqlDataReader genRdr = genCmd.ExecuteReader())
                                    {
                                        genRdr.Read();
                                        rCode = Convert.ToString(genRdr["RoughCode"]);
                                        rCodeYear = Convert.ToInt32(genRdr["RCodeYear"]);
                                        rCodeSerial = Convert.ToInt32(genRdr["RCodeSerial"]);
                                    }
                                }

                                using (SqlCommand lotCodeCmd = new SqlCommand("usp_GenerateLotCode", con, tran))
                                {
                                    lotCodeCmd.CommandType = CommandType.StoredProcedure;
                                    lotCodeCmd.Parameters.AddWithValue("@RoughDistributionID", 0);
                                    lotCodeCmd.Parameters.AddWithValue("@RCode", rCode);
                                    using (SqlDataReader lotRdr = lotCodeCmd.ExecuteReader())
                                    {
                                        lotRdr.Read();
                                        lotCode = Convert.ToString(lotRdr["LotCode"]);
                                        distributionPartNo = Convert.ToInt32(lotRdr["DistributionPartNo"]);
                                    }
                                }

                                rCodeParam = rCode;
                                rCodeYearParam = rCodeYear;
                                rCodeSerialParam = rCodeSerial;
                                lotCodeParam = lotCode;
                                distributionPartNoParam = distributionPartNo;
                            }

                            using (SqlCommand updCmd = new SqlCommand("USP_MST_Lot_UpdateBranch", con, tran))
                            {
                                updCmd.CommandType = CommandType.StoredProcedure;
                                updCmd.Parameters.AddWithValue("@LotID", LotID);
                                updCmd.Parameters.AddWithValue("@BranchID", BranchID);
                                updCmd.Parameters.AddWithValue("@DepartmentID", (object)DepartmentID ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@HeadManagerID", (object)HeadManagerID ?? DBNull.Value);
                                updCmd.Parameters.AddWithValue("@RCode", rCodeParam);
                                updCmd.Parameters.AddWithValue("@RCodeYear", rCodeYearParam);
                                updCmd.Parameters.AddWithValue("@RCodeSerial", rCodeSerialParam);
                                updCmd.Parameters.AddWithValue("@LotCode", lotCodeParam);
                                updCmd.Parameters.AddWithValue("@DistributionPartNo", distributionPartNoParam);
                                updCmd.Parameters.AddWithValue("@ModifiedBy", currentUserId);

                                using (SqlDataReader updRdr = updCmd.ExecuteReader())
                                {
                                    r = new Dictionary<string, object>();
                                    if (updRdr.Read())
                                    {
                                        for (int i = 0; i < updRdr.FieldCount; i++)
                                        {
                                            r[updRdr.GetName(i)] = updRdr.GetValue(i);
                                        }
                                    }
                                }
                            }

                            bool rowSuccess = r.ContainsKey("Success") && Convert.ToBoolean(r["Success"]);
                            if (!rowSuccess)
                            {
                                throw new Exception(r.ContainsKey("Message") ? Convert.ToString(r["Message"]) : "ERROR");
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                BranchApiClient.SyncLot(LotID);

                return Json(new
                {
                    success = true,
                    message = r.ContainsKey("Message") ? Convert.ToString(r["Message"]) : "Branch assigned.",
                    LotID = LotID,
                    RCode = r.ContainsKey("RCode") ? r["RCode"] : null,
                    LotCode = r.ContainsKey("LotCode") ? r["LotCode"] : null
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) });
            }
        }

        [HttpPost]
        public JsonResult Lot_Delete(int LotID)
        {
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_MST_Lot_Delete",
                    new SqlParameter("@LotID", LotID),
                    new SqlParameter("@ModifiedBy", currentUserId));

                var r = rows.FirstOrDefault();
                bool success = r != null && r.ContainsKey("Success") && Convert.ToBoolean(r["Success"]);
                string message = r != null && r.ContainsKey("Message") ? Convert.ToString(r["Message"]) : "ERROR";

                return Json(new { success = success, message = message });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) });
            }
        }

        // Lot Master's "Reset & Reload Old Data" button: pulls the old
        // ADP_TEST system's Rough/Lot history from AnjaliLabTech.API (same
        // service as BranchApiClient's Branch 1 sync) and wipes + reloads
        // TRN_RoughInward/TRN_RoughInwardDetail/MST_Lot from it. Destructive
        // and irreversible - the confirmation happens client-side before
        // this is ever called.
        [HttpPost]
        public JsonResult ResetAndImportOldInwardData()
        {
            try
            {
                string rawJson = BranchApiClient.FetchOldInwardDataJson();
                JObject parsed = JObject.Parse(rawJson);

                string roughInwardJson = (parsed["roughInward"] ?? new JArray()).ToString();
                string roughInwardDetailJson = (parsed["roughInwardDetails"] ?? new JArray()).ToString();
                string lotJson = (parsed["lots"] ?? new JArray()).ToString();

                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 1;

                string message = DbHelper.ExecuteNonQueryWithMessage("USP_MST_ResetAndImport_OldInwardData", 600,
                    new SqlParameter("@RoughInwardJson", SqlDbType.NVarChar, -1) { Value = roughInwardJson },
                    new SqlParameter("@RoughInwardDetailJson", SqlDbType.NVarChar, -1) { Value = roughInwardDetailJson },
                    new SqlParameter("@LotJson", SqlDbType.NVarChar, -1) { Value = lotJson },
                    new SqlParameter("@BranchID", 1),
                    new SqlParameter("@CreatedBy", currentUserId));

                bool success = !string.IsNullOrEmpty(message) && !message.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase);
                return Json(new { success = success, message = message });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) });
            }
            }

        public JsonResult LotCode_Insert_Using_Excel(HttpPostedFileBase excelFile)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            List<MST_Lot> LotList = new List<MST_Lot>();
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    Message = "Please upload an Excel file";

                if (Message != "Please upload an Excel file")
                {

                    string Date = "";
                    using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                    {
                        ExcelWorksheet ws = package.Workbook.Worksheets[0];

                        int row = 1;

                        row = CommonMethods.FindRow(ws, 1, "Lot Code");
                        row++;

                        while (ws.Cells[row, 1].Value != null)
                        {
                            LotList.Add(new MST_Lot
                            {
                                LotCode = ws.Cells[row, 1].Text,
                                RCode = ws.Cells[row, 2].Text,
                                MainRCode = ws.Cells[row, 3].Text,
                                FactoryCode = ws.Cells[row, 4].Text,
                                ColorType = ws.Cells[row, 5].Text,
                                SizeCode = ws.Cells[row, 6].Text,
                                SizeCodeType = ws.Cells[row, 7].Text,
                            });
                            row++;
                        }

                        string xmlStr = "";
                        if (LotList.Count > 0)
                        {
                            XmlDocument Xmldata = CommonMethods.ConvertToXml(LotList);
                            xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                        }

                        Message = DbHelper.ExecuteNonQueryWithMessage("LotCode_Insert_Using_Excel",
                            new SqlParameter("@XML", xmlStr),
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }
            return Json(new { Message });
        }
        public JsonResult GetBranchdata_For_DropDown()
        {
            try
            {
                
                List<object> _list_Branch = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("GetBranchdata_For_DropDown");
                if (_DropDownList.Tables.Count > 0 && _DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_Branch.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    BranchList = _list_Branch
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        public ActionResult PreRough()
        {
            SetModulePermissions("Master", "PreRough");
            return View();
        }
        public JsonResult Getdata_R_Rough_Add()
        {
            try
            {
                List<object> _list__DIA_Rough = new List<object>();
                DataSet _DropDownList = DbHelper.ExecuteDataSet("Z_Getdata_R_Rough_Add");

                HashSet<string> uploadedJangads = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                HashSet<int> uploadedPreRoughIds = new HashSet<int>();
                try
                {
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT JangadNo, PreRoughID FROM MST_JangadImages WITH(NOLOCK) WHERE Status IN ('PreRough', 'Pre rough') OR PreRoughID IS NOT NULL", con))
                        {
                            con.Open();
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    if (rdr["JangadNo"] != DBNull.Value && !string.IsNullOrWhiteSpace(rdr["JangadNo"].ToString()))
                                    {
                                        uploadedJangads.Add(rdr["JangadNo"].ToString().Trim());
                                    }
                                    if (rdr["PreRoughID"] != DBNull.Value)
                                    {
                                        uploadedPreRoughIds.Add(Convert.ToInt32(rdr["PreRoughID"]));
                                    }
                                }
                            }
                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.ErrorLog(ex);
                }

                if (_DropDownList.Tables.Count > 0 && _DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, object> Values = new Dictionary<string, object>();
                        string rowJangadNo = "";
                        int rowPreRoughId = 0;
                        bool hasImageFlag = false;

                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            string colName = _DropDownList.Tables[0].Columns[j].ColumnName;
                            object colVal = _DropDownList.Tables[0].Rows[i][j];
                            Values.Add(colName, colVal != DBNull.Value ? colVal.ToString() : null);

                            if (colName.Equals("JangadNo", StringComparison.OrdinalIgnoreCase) && colVal != DBNull.Value)
                            {
                                rowJangadNo = colVal.ToString().Trim();
                            }
                            if ((colName.Equals("ID", StringComparison.OrdinalIgnoreCase) || colName.Equals("ProcessID", StringComparison.OrdinalIgnoreCase) || colName.Equals("PreRoughID", StringComparison.OrdinalIgnoreCase)) && colVal != DBNull.Value)
                            {
                                int.TryParse(colVal.ToString(), out rowPreRoughId);
                            }
                            if (colName.Equals("IsImageUpload", StringComparison.OrdinalIgnoreCase) && colVal != DBNull.Value)
                            {
                                string strVal = colVal.ToString();
                                hasImageFlag = strVal == "1" || strVal.Equals("true", StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        bool isUploaded = hasImageFlag || (!string.IsNullOrEmpty(rowJangadNo) && uploadedJangads.Contains(rowJangadNo)) || (rowPreRoughId > 0 && uploadedPreRoughIds.Contains(rowPreRoughId));
                        Values["IsImageUpload"] = isUploaded;

                        _list__DIA_Rough.Add(Values);
                    }
                }

                List<DataFields> DataField = new List<DataFields>();
                if (_DropDownList.Tables.Count > 1 && _DropDownList.Tables[1].Rows.Count > 0)
                {
                    DataField = _DropDownList.Tables[1].AsEnumerable()
                    .Select(row => new DataFields
                    {
                        dataField = row.Table.Columns.Contains("dataField") && row["dataField"] != DBNull.Value ? row["dataField"].ToString() : "",
                        caption = row.Table.Columns.Contains("caption") && row["caption"] != DBNull.Value ? row["caption"].ToString() : (row.Table.Columns.Contains("dataField") && row["dataField"] != DBNull.Value ? row["dataField"].ToString() : ""),
                        dataType = row.Table.Columns.Contains("dataType") && row["dataType"] != DBNull.Value ? row["dataType"].ToString() : "string",
                        format = row.Table.Columns.Contains("format") && row["format"] != DBNull.Value ? row["format"].ToString() : "",
                        alignment = row.Table.Columns.Contains("alignment") && row["alignment"] != DBNull.Value ? row["alignment"].ToString() : "left",
                        visible = row.Table.Columns.Contains("visible") && row["visible"] != DBNull.Value ? Convert.ToBoolean(row["visible"]) : true,
                        @fixed = row.Table.Columns.Contains("fixed") && row["fixed"] != DBNull.Value ? Convert.ToBoolean(row["fixed"]) : false
                    })
                    .ToList();
                }

                List<DataFields> DataFieldSummary = new List<DataFields>();
                if (_DropDownList.Tables.Count > 2 && _DropDownList.Tables[2].Rows.Count > 0)
                {
                    DataFieldSummary = _DropDownList.Tables[2].AsEnumerable()
                    .Select(row => new DataFields
                    {
                        column = row.Table.Columns.Contains("column") && row["column"] != DBNull.Value ? row["column"].ToString() : "",
                        valueFormat = row.Table.Columns.Contains("valueFormat") && row["valueFormat"] != DBNull.Value ? row["valueFormat"].ToString() : "#0.000",
                        summaryType = row.Table.Columns.Contains("summaryType") && row["summaryType"] != DBNull.Value ? row["summaryType"].ToString() : "sum",
                        displayFormat = row.Table.Columns.Contains("displayFormat") && row["displayFormat"] != DBNull.Value ? row["displayFormat"].ToString() : "{0}"
                    })
                    .ToList();
                }

                var jsonResult = Json(new
                {
                    success = true,
                    DIARoughList = _list__DIA_Rough,
                    DataField = DataField,
                    DataFieldSummary = DataFieldSummary
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                    success = false,
                    message = ex.Message,
                    DIARoughList = new List<object>(),
                    DataField = new List<DataFields>(),
                    DataFieldSummary = new List<DataFields>()
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }
        public string R_Rough_Add_Insert_Update_Delete(R_Rough_Add model, string Action)
        {
            string Message = "";
            try
            {
                model.UserID = SessionFacade.UserSession.UserID;

                XmlDocument Xmldata = CommonMethods.ConvertToXml(model);
                string xmlStr = "<DocumentElement><R_Rough_Add>" + Xmldata.DocumentElement.InnerXml + "</R_Rough_Add></DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("Z_R_Rough_Add_Insert_Update_Delete",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@ACTION", Action));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR : " + ex.InnerException.Message;
            }
            return Message;
        }

        [HttpPost]
        public ActionResult Upload_Jangad_Images(int? PreRoughID, int? DistributionID, string JangadNo, string Status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(JangadNo))
                {
                    return Json(new { success = false, message = "JangadNo is required." });
                }

                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No files selected for upload." });
                }

                // Determine target physical folder and normalized DB status
                bool isPreRough = (!string.IsNullOrEmpty(Status) && Status.ToLower().Replace(" ", "").Contains("pre"));
                string normalizedStatus = isPreRough ? "PreRough" : "Distribution";
                string folderName = isPreRough ? "PreRough_JangadImage" : "RoughDistribution_JangadImages";

                // Clean JangadNo for folder name
                string safeJangadNo = string.Join("_", JangadNo.Split(Path.GetInvalidFileNameChars())).Trim();
                string relativeDirectory = $"/Upload/{folderName}/{safeJangadNo}/";
                string physicalDirectory = Server.MapPath("~" + relativeDirectory);

                if (!Directory.Exists(physicalDirectory))
                {
                    Directory.CreateDirectory(physicalDirectory);
                }

                int uploadBy = 1;
                if (SessionFacade.UserSession != null && SessionFacade.UserSession.UserID > 0)
                {
                    uploadBy = SessionFacade.UserSession.UserID;
                }

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                int successCount = 0;
                List<string> savedPaths = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);
                        if (string.IsNullOrEmpty(ext)) ext = ".jpg";

                        string fileName = $"{safeJangadNo}_{timestamp}_{i}{ext}";
                        string fullPhysicalPath = Path.Combine(physicalDirectory, fileName);
                        file.SaveAs(fullPhysicalPath);

                        string webRelativePath = relativeDirectory + fileName;
                        savedPaths.Add(webRelativePath);

                        DbHelper.ExecuteNonQueryWithMessage("Jangad_Images_Insert",
                            new SqlParameter("@PreRoughID", (object)PreRoughID ?? DBNull.Value),
                            new SqlParameter("@DistributionID", (object)DistributionID ?? DBNull.Value),
                            new SqlParameter("@JangadNo", safeJangadNo),
                            new SqlParameter("@ImagePath", webRelativePath),
                            new SqlParameter("@UploadBy", uploadBy),
                            new SqlParameter("@Status", normalizedStatus));
                        successCount++;
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Images uploaded successfully",
                    isImageUpload = true,
                    files = savedPaths
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "Upload failed: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Get_Jangad_Images(int? PreRoughID, int? DistributionID, string JangadNo, string Status)
        {
            try
            {
                bool isPreRough = (!string.IsNullOrEmpty(Status) && Status.ToLower().Replace(" ", "").Contains("pre"));
                string normalizedStatus = string.IsNullOrEmpty(Status) ? null : (isPreRough ? "PreRough" : "Distribution");

                List<object> imageList = new List<object>();

                List<Dictionary<string, object>> imageRows = DbHelper.ExecuteReaderAsList("Get_Jangad_Images",
                    new SqlParameter("@JangadNo", string.IsNullOrEmpty(JangadNo) ? (object)DBNull.Value : JangadNo),
                    new SqlParameter("@PreRoughID", (object)PreRoughID ?? DBNull.Value),
                    new SqlParameter("@DistributionID", (object)DistributionID ?? DBNull.Value),
                    new SqlParameter("@Status", (object)normalizedStatus ?? DBNull.Value));

                foreach (var row in imageRows)
                {
                    string rawPath = row["ImagePath"] != null ? row["ImagePath"].ToString() : "";
                    string resolvedUrl = rawPath;
                    if (!string.IsNullOrEmpty(rawPath) && !rawPath.StartsWith("http") && !rawPath.StartsWith("/"))
                    {
                        resolvedUrl = "/" + rawPath;
                    }

                    imageList.Add(new
                    {
                        ID = row["ID"] != null ? Convert.ToInt32(row["ID"]) : 0,
                        PreRoughID = row["PreRoughID"] != null ? (int?)Convert.ToInt32(row["PreRoughID"]) : null,
                        DistributionID = row["DistributionID"] != null ? (int?)Convert.ToInt32(row["DistributionID"]) : null,
                        JangadNo = row["JangadNo"] != null ? row["JangadNo"].ToString() : "",
                        ImagePath = resolvedUrl,
                        Created = row["Created"] != null ? Convert.ToDateTime(row["Created"]).ToString("dd-MMM-yyyy HH:mm") : "",
                        UploadBy = row["UploadBy"] != null ? Convert.ToInt32(row["UploadBy"]) : 0,
                        Status = row["Status"] != null ? row["Status"].ToString() : ""
                    });
                }

                var jsonResult = Json(new
                {
                    success = true,
                    list = imageList
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new
                {
                    success = false,
                    message = "Error fetching images: " + ex.Message,
                    list = new List<object>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete_Jangad_Image(int imageId)
        {
            try
            {
                // Verify Delete permissions
                bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
                int userId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                string message = DbHelper.ExecuteNonQueryWithMessage("Jangad_Images_Insert",
                    new SqlParameter("@ImageID", imageId),
                    new SqlParameter("@Action", "DELETE"),
                    new SqlParameter("@UploadBy", userId));

                bool isSuccess = !string.IsNullOrEmpty(message) && message.ToLower().Contains("success");
                return Json(new { success = isSuccess, message = message });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult KapanImg()
        {
            SetModulePermissions("Master", "KapanImg");
            return View();
        }

        public ActionResult TRNLabour()
        {
            SetModulePermissions("Master", "TRNLabour");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canEdit = isAdmin || (ViewBag.CanEdit == true);
            bool canDelete = isAdmin || (ViewBag.CanDelete == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    string[] possibleActions = new string[] { "TRNLabour", "TRN Labour", "Labour" };

                    foreach (string actName in possibleActions)
                    {
                        List<Dictionary<string, object>> permRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                            new SqlParameter("@ControllerName", "Master"),
                            new SqlParameter("@ActionName", actName));

                        if (permRows.Count > 0)
                        {
                            var row = permRows[0];
                            bool foundPerm = false;
                            foreach (var kvp in row)
                            {
                                string col = kvp.Key;
                                if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canView = true; foundPerm = true; }
                                }
                                else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canAdd = true; foundPerm = true; }
                                }
                                else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canEdit = true; foundPerm = true; }
                                }
                                else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canDelete = true; foundPerm = true; }
                                }
                                else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canExport = true; foundPerm = true; }
                                }
                            }
                            if (foundPerm) break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            if (canAdd || canEdit || canDelete || canExport)
            {
                canView = true;
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanDelete = canDelete;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }
        public JsonResult Getdata_TRN_Labour()
        {
            try
            {
                List<object> _list__DIA_Rough = new List<object>();
                List<object> DIARoughList = new List<object>();
                DataSet _DropDownList = DbHelper.ExecuteDataSet("Getdata_TRN_Labour");
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list__DIA_Rough.Add(Values);
                    }
                }

                var DataField = _DropDownList.Tables[1].AsEnumerable()
                .Select(row => new DataFields
                {
                    dataField = row.Field<string>("dataField"),
                    dataType = row.Field<string>("dataType"),
                    format = row.Field<string>("format"),
                    alignment = row.Field<string>("alignment"),
                    visible = row.Field<bool>("visible"),
                    @fixed = row.Field<bool>("fixed"),
                })
                .ToList();

                var jsonResult = Json(new
                {
                    DIARoughList = _list__DIA_Rough,
                    DataField = DataField
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }
        #region User File Size Master
        public ActionResult UserFileSizeMaster()
        {
            SetModulePermissions("Master", "UserFileSizeMaster");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canEdit = isAdmin || (ViewBag.CanEdit == true);
            bool canDelete = isAdmin || (ViewBag.CanDelete == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    string[] possibleActions = new string[] { "UserFileSizeMaster", "User File Size Master", "UserFileSize", "User File Size" };

                    foreach (string actName in possibleActions)
                    {
                        List<Dictionary<string, object>> permRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                            new SqlParameter("@ControllerName", "Master"),
                            new SqlParameter("@ActionName", actName));

                        if (permRows.Count > 0)
                        {
                            var row = permRows[0];
                            bool foundPerm = false;
                            foreach (var kvp in row)
                            {
                                string col = kvp.Key;
                                if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canView = true; foundPerm = true; }
                                }
                                else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canAdd = true; foundPerm = true; }
                                }
                                else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canEdit = true; foundPerm = true; }
                                }
                                else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canDelete = true; foundPerm = true; }
                                }
                                else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    if (Convert.ToBoolean(kvp.Value)) { canExport = true; foundPerm = true; }
                                }
                            }
                            if (foundPerm) break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            if (canAdd || canEdit || canDelete || canExport)
            {
                canView = true;
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanDelete = canDelete;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        [HttpGet]
        public JsonResult GetUserFileSizeList()
        {
            try
            {
                List<object> _list = new List<object>();

                DataSet _DropDownList = DbHelper.ExecuteDataSet("MST_UserFileSize_Get");

                if (_DropDownList.Tables.Count > 0 && _DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list.Add(Values);
                    }
                }

                var jsonResult = Json(new { List = _list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new { }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        [HttpPost]
        public JsonResult UserFileSize_Insert_Update_Delete(UserFileSizeModel model, string Action)
        {
            string Message = "";
            bool isSuccess = false;
            try
            {
                model.UserID = SessionFacade.UserSession.UserID;

                XmlDocument Xmldata = CommonMethods.ConvertToXml(model);
                string xmlStr = "<DocumentElement><UserFileSizeModel>" + Xmldata.DocumentElement.InnerXml + "</UserFileSizeModel></DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("MST_UserFileSize_Insert_Update_Delete",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@ACTION", Action));

                if (Message.ToLower().Contains("successfully"))
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Json(new { success = isSuccess, message = Message });
        }
        #endregion

        #region Rough Distribution
        public ActionResult RoughDistributionList()
        {
            SetModulePermissions("Master", "RoughDistributionList");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canEdit = isAdmin || (ViewBag.CanEdit == true);
            bool canDelete = isAdmin || (ViewBag.CanDelete == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    string[] possibleActions = new string[] { "RoughDistributionList", "RoughDistribution", "Rough Distribution List", "Rough Distribution" };

                    foreach (string actName in possibleActions)
                    {
                        List<Dictionary<string, object>> permRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                            new SqlParameter("@ControllerName", "Master"),
                            new SqlParameter("@ActionName", actName));

                        if (permRows.Count > 0)
                        {
                            var row = permRows[0];
                            bool foundPerm = false;
                            foreach (var kvp in row)
                            {
                                string col = kvp.Key;
                                if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canView = true; foundPerm = true; }
                                }
                                else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canAdd = true; foundPerm = true; }
                                }
                                else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canEdit = true; foundPerm = true; }
                                }
                                else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canDelete = true; foundPerm = true; }
                                }
                                else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canExport = true; foundPerm = true; }
                                }
                            }
                            if (foundPerm)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            if (canAdd || canEdit || canDelete || canExport)
            {
                canView = true;
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanDelete = canDelete;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        [HttpGet]
        public JsonResult Get_MST_RoughDistributionList()
        {
            try
            {
                List<object> list = new List<object>();

                DataSet ds = DbHelper.ExecuteDataSet("Get_MST_RoughDistributionList");

                HashSet<int> uploadedDistIds = new HashSet<int>();
                HashSet<string> uploadedDistJangadsWithoutDistId = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT JangadNo, DistributionID, Status FROM MST_JangadImages WITH(NOLOCK) WHERE Status = 'Distribution' OR (DistributionID IS NOT NULL AND DistributionID > 0)", con))
                        {
                            con.Open();
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    if (rdr["DistributionID"] != DBNull.Value && Convert.ToInt32(rdr["DistributionID"]) > 0)
                                    {
                                        uploadedDistIds.Add(Convert.ToInt32(rdr["DistributionID"]));
                                    }
                                    else if (rdr["JangadNo"] != DBNull.Value && !string.IsNullOrWhiteSpace(rdr["JangadNo"].ToString()) && string.Equals(Convert.ToString(rdr["Status"]), "Distribution", StringComparison.OrdinalIgnoreCase))
                                    {
                                        uploadedDistJangadsWithoutDistId.Add(rdr["JangadNo"].ToString().Trim());
                                    }
                                }
                            }
                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.ErrorLog(ex);
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        string rowJangadNo = "";
                        int rowDistId = 0;
                        bool hasImageFlag = false;

                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            string colName = ds.Tables[0].Columns[j].ColumnName;
                            object colVal = ds.Tables[0].Rows[i][j];
                            row[colName] = colVal != DBNull.Value ? colVal : null;

                            if (colName.Equals("JangadNo", StringComparison.OrdinalIgnoreCase) && colVal != DBNull.Value)
                            {
                                rowJangadNo = colVal.ToString().Trim();
                            }
                            if (colName.Equals("DistributionID", StringComparison.OrdinalIgnoreCase) && colVal != DBNull.Value)
                            {
                                int.TryParse(colVal.ToString(), out rowDistId);
                            }
                            if (colName.Equals("IsImageUpload", StringComparison.OrdinalIgnoreCase) && colVal != DBNull.Value)
                            {
                                string strVal = colVal.ToString();
                                hasImageFlag = strVal == "1" || strVal.Equals("true", StringComparison.OrdinalIgnoreCase);
                            }
                        }

                        bool isUploaded = hasImageFlag || (rowDistId > 0 && uploadedDistIds.Contains(rowDistId)) || (!string.IsNullOrEmpty(rowJangadNo) && uploadedDistJangadsWithoutDistId.Contains(rowJangadNo));
                        row["IsImageUpload"] = isUploaded;

                        if (!row.ContainsKey("Priority") || row["Priority"] == null || string.IsNullOrWhiteSpace(row["Priority"].ToString()))
                        {
                            row["Priority"] = "REGULAR";
                        }

                        if (row.ContainsKey("InwardDetailID") && row["InwardDetailID"] != null && !row.ContainsKey("PreRoughID"))
                        {
                            row["PreRoughID"] = row["InwardDetailID"];
                        }

                        list.Add(row);
                    }
                }

                var jsonResult = Json(new { success = true, List = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new { success = false, List = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        public ActionResult RoughDistribution(int id = 0)
        {
            SetModulePermissions("Master", "RoughDistribution");
            ViewBag.PreRoughID = id;
            return View();
        }

        [HttpGet]
        public JsonResult GetRoughInwardLotsForDistribution()
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_TRN_RoughInward_GetLotsForDistribution");

                var jsonResult = Json(new
                {
                    success = true,
                    DIARoughList = list
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                    success = false,
                    message = ex.Message,
                    DIARoughList = new List<object>()
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        [HttpGet]
        public JsonResult GetRoughInwardLotDetails(int id = 0)
        {
            try
            {
                Dictionary<string, object> masterDetails = new Dictionary<string, object>();
                List<Dictionary<string, object>> childList = new List<Dictionary<string, object>>();

                DataSet ds = DbHelper.ExecuteDataSet("USP_TRN_RoughInward_GetLotDetailForDistribution",
                    new SqlParameter("@InwardDetailID", id));

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        string colName = ds.Tables[0].Columns[j].ColumnName;
                        masterDetails[colName] = row[j] != DBNull.Value ? row[j] : null;
                    }
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        Dictionary<string, object> childRow = new Dictionary<string, object>();
                        for (int j = 0; j < ds.Tables[1].Columns.Count; j++)
                        {
                            string colName = ds.Tables[1].Columns[j].ColumnName;
                            childRow[colName] = ds.Tables[1].Rows[i][j] != DBNull.Value ? ds.Tables[1].Rows[i][j] : null;
                        }
                        if (!childRow.ContainsKey("Priority") || childRow["Priority"] == null || string.IsNullOrWhiteSpace(childRow["Priority"].ToString()))
                        {
                            childRow["Priority"] = "REGULAR";
                        }
                        childList.Add(childRow);
                    }
                }

                var jsonResult = Json(new
                {
                    success = true,
                    MasterDetails = masterDetails,
                    ChildList = childList
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                var jsonResult = Json(new
                {
                    success = false,
                    message = ex.Message,
                    MasterDetails = new Dictionary<string, object>(),
                    ChildList = new List<Dictionary<string, object>>()
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }


        [HttpPost]
        public JsonResult SaveRoughDistribution(List<MST_RoughDistribution> list, List<int> deletedIDs = null, string Action = "INSERT", int SizeCodeID = 0)
        {
            return RoughDistribution_Insert_Update_Delete(list, Action, deletedIDs, SizeCodeID);
        }

        [HttpPost]
        public JsonResult RoughDistribution_Insert_Update_Delete(List<MST_RoughDistribution> list, string Action = "INSERT", List<int> deletedIDs = null, int SizeCodeID = 0)
        {
            string Message = "";
            bool isSuccess = false;
            List<int> newDistributionIDs = new List<int>();
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                // Normalize deletedIDs if Action == "DELETE" and list is passed
                if ((deletedIDs == null || deletedIDs.Count == 0) && list != null && list.Count > 0 && Action == "DELETE")
                {
                    deletedIDs = list.Where(x => x.DistributionID > 0).Select(x => x.DistributionID).ToList();
                }

                // 1. Process Deferred Deletions if any
                if (deletedIDs != null && deletedIDs.Count > 0)
                {
                    // LIFO Sequential Deletion Validation across Database Records
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (int delId in deletedIDs)
                        {
                            if (delId > 0)
                            {
                                int branchId = 0;
                                int inwardDetailId = 0;
                                string rCode = "";
                                bool isReceived = false;

                                using (SqlCommand getCmd = new SqlCommand("SELECT BranchID, InwardDetailID, RCode, IsReceived FROM MST_RoughDistribution WITH(NOLOCK) WHERE DistributionID = @DistributionID", con))
                                {
                                    getCmd.Parameters.AddWithValue("@DistributionID", delId);
                                    using (SqlDataReader rdr = getCmd.ExecuteReader())
                                    {
                                        if (rdr.Read())
                                        {
                                            branchId = Convert.ToInt32(rdr["BranchID"] != DBNull.Value ? rdr["BranchID"] : 0);
                                            inwardDetailId = Convert.ToInt32(rdr["InwardDetailID"] != DBNull.Value ? rdr["InwardDetailID"] : 0);
                                            rCode = Convert.ToString(rdr["RCode"] != DBNull.Value ? rdr["RCode"] : "");
                                            isReceived = rdr["IsReceived"] != DBNull.Value && Convert.ToBoolean(rdr["IsReceived"]);
                                        }
                                    }
                                }

                                if (isReceived)
                                {
                                    con.Close();
                                    return Json(new
                                    {
                                        success = false,
                                        message = "Cannot delete: this distribution (R.Code '" + rCode + "') has already been received."
                                    });
                                }

                                if (!string.IsNullOrEmpty(rCode))
                                {
                                    // Fetch sibling active records for this branch under the same Jangad/lot (InwardDetailID).
                                    // Ordering by DistributionID (always-increasing identity) instead of parsing a
                                    // sequence number out of the RCode text - the new RCode format shares one serial
                                    // across branches for the same row and reuses it verbatim across repeat splits
                                    // (DistributionPartNo), so the trailing digits of RCode are no longer a reliable
                                    // "created later" indicator.
                                    string higherRCode = null;
                                    int maxHigherId = delId;
                                    using (SqlCommand sibCmd = new SqlCommand("SELECT DistributionID, RCode FROM MST_RoughDistribution WITH(NOLOCK) WHERE BranchID = @BranchID AND InwardDetailID = @InwardDetailID AND IsActive = 1", con))
                                    {
                                        sibCmd.Parameters.AddWithValue("@BranchID", branchId);
                                        sibCmd.Parameters.AddWithValue("@InwardDetailID", inwardDetailId);
                                        using (SqlDataReader sibRdr = sibCmd.ExecuteReader())
                                        {
                                            while (sibRdr.Read())
                                            {
                                                int sibId = Convert.ToInt32(sibRdr["DistributionID"]);
                                                string sibRCode = Convert.ToString(sibRdr["RCode"] ?? "");
                                                if (!deletedIDs.Contains(sibId) && sibId > maxHigherId)
                                                {
                                                    maxHigherId = sibId;
                                                    higherRCode = sibRCode;
                                                }
                                            }
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(higherRCode))
                                    {
                                        con.Close();
                                        return Json(new
                                        {
                                            success = false,
                                            message = "Sequential Deletion Restriction (LIFO): Cannot delete R.Code '" + rCode + "'. You must delete the latest entry ('" + higherRCode + "') for this branch first."
                                        });
                                    }
                                }
                            }
                        }
                        con.Close();
                    }

                    // Execute Database Deletions
                    foreach (int delId in deletedIDs)
                    {
                        if (delId > 0)
                        {
                            System.Text.StringBuilder delXmlBuilder = new System.Text.StringBuilder();
                            delXmlBuilder.Append("<DocumentElement>");
                            delXmlBuilder.Append("<MST_RoughDistribution>");
                            delXmlBuilder.Append("<DistributionID>").Append(delId).Append("</DistributionID>");
                            delXmlBuilder.Append("</MST_RoughDistribution>");
                            delXmlBuilder.Append("</DocumentElement>");

                            Message = DbHelper.ExecuteNonQueryWithMessage("RoughDistribution_Insert_Update_Delete",
                                new SqlParameter("@XML", delXmlBuilder.ToString()),
                                new SqlParameter("@ACTION", "DELETE"));
                        }
                    }
                }

                // 2. Process Insertions / Staged Items (Only process items where DistributionID == 0 or newly added)
                var newItems = list != null ? list.Where(x => x.DistributionID == 0).ToList() : new List<MST_RoughDistribution>();

                if (newItems.Count > 0)
                {
                    // R.Code is generated server-side, atomically, per item - via
                    // dbo.usp_GenerateRoughCode(@SizeID, @BranchID, @InwardDetailID, @Year).
                    // The SP shares one serial per InwardDetailID across branches and reuses
                    // a branch's existing RCode (bumping DistributionPartNo) on repeat splits
                    // of the same row+branch - see usp_GenerateRoughCode for the full rule.
                    // Sibling new items for the SAME InwardDetailID in THIS batch aren't in the
                    // DB yet (the bulk insert happens after this loop), so the SP can't see them
                    // via its own lookup - rowSerialCache carries the first-generated Year/Serial
                    // for a row forward to any later item in the batch sharing that row.
                    var rowSerialCache = new Dictionary<int, Tuple<int, int>>();
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (var item in newItems)
                        {
                            using (SqlCommand genCmd = new SqlCommand("usp_GenerateRoughCode", con))
                            {
                                genCmd.CommandType = CommandType.StoredProcedure;
                                genCmd.Parameters.AddWithValue("@SizeID", SizeCodeID);
                                genCmd.Parameters.AddWithValue("@BranchID", item.BranchID);
                                genCmd.Parameters.AddWithValue("@InwardDetailID", item.InwardDetailID);
                                genCmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);
                                if (rowSerialCache.TryGetValue(item.InwardDetailID, out var known))
                                {
                                    genCmd.Parameters.AddWithValue("@KnownYear", known.Item1);
                                    genCmd.Parameters.AddWithValue("@KnownSerial", known.Item2);
                                }
                                using (SqlDataReader genRdr = genCmd.ExecuteReader())
                                {
                                    if (genRdr.Read())
                                    {
                                        item.RCode = Convert.ToString(genRdr["RoughCode"]);
                                        item.RCodeYear = Convert.ToInt32(genRdr["RCodeYear"]);
                                        item.RCodeSerial = Convert.ToInt32(genRdr["RCodeSerial"]);
                                        item.DistributionPartNo = Convert.ToInt32(genRdr["DistributionPartNo"]);
                                    }
                                }
                            }

                            if (!rowSerialCache.ContainsKey(item.InwardDetailID) && item.RCodeYear.HasValue && item.RCodeSerial.HasValue)
                            {
                                rowSerialCache[item.InwardDetailID] = Tuple.Create(item.RCodeYear.Value, item.RCodeSerial.Value);
                            }
                        }
                        con.Close();
                    }

                    System.Text.StringBuilder xmlBuilder = new System.Text.StringBuilder();
                    xmlBuilder.Append("<DocumentElement>");
                    foreach (var item in newItems)
                    {
                        xmlBuilder.Append("<MST_RoughDistribution>");
                        xmlBuilder.Append("<DistributionID>").Append(item.DistributionID).Append("</DistributionID>");
                        xmlBuilder.Append("<InwardDetailID>").Append(item.InwardDetailID).Append("</InwardDetailID>");
                        xmlBuilder.Append("<BranchID>").Append(item.BranchID).Append("</BranchID>");
                        xmlBuilder.Append("<RCode>").Append(System.Security.SecurityElement.Escape(item.RCode ?? "")).Append("</RCode>");
                        xmlBuilder.Append("<RCodeYear>").Append(item.RCodeYear ?? 0).Append("</RCodeYear>");
                        xmlBuilder.Append("<RCodeSerial>").Append(item.RCodeSerial ?? 0).Append("</RCodeSerial>");
                        xmlBuilder.Append("<DistributionPartNo>").Append(item.DistributionPartNo ?? 1).Append("</DistributionPartNo>");
                        xmlBuilder.Append("<RoughPcs>").Append(item.RoughPcs).Append("</RoughPcs>");
                        xmlBuilder.Append("<RoughWeight>").Append(item.RoughWeight.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)).Append("</RoughWeight>");
                        if (item.Rate.HasValue)
                            xmlBuilder.Append("<Rate>").Append(item.Rate.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).Append("</Rate>");
                        else
                            xmlBuilder.Append("<Rate>0.00</Rate>");
                        xmlBuilder.Append("<GradNo>").Append(System.Security.SecurityElement.Escape(item.GradNo ?? "")).Append("</GradNo>");
                        if (item.FromHeight.HasValue)
                            xmlBuilder.Append("<FromHeight>").Append(item.FromHeight.Value.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)).Append("</FromHeight>");
                        else
                            xmlBuilder.Append("<FromHeight>0.000</FromHeight>");
                        if (item.ToHeight.HasValue)
                            xmlBuilder.Append("<ToHeight>").Append(item.ToHeight.Value.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)).Append("</ToHeight>");
                        else
                            xmlBuilder.Append("<ToHeight>0.000</ToHeight>");
                        xmlBuilder.Append("<Remarks>").Append(System.Security.SecurityElement.Escape(item.Remarks ?? "")).Append("</Remarks>");
                        xmlBuilder.Append("<Priority>").Append(System.Security.SecurityElement.Escape(string.IsNullOrWhiteSpace(item.Priority) ? "REGULAR" : item.Priority.Trim())).Append("</Priority>");
                        xmlBuilder.Append("<IsActive>").Append(item.IsActive ? "1" : "0").Append("</IsActive>");
                        xmlBuilder.Append("<UserID>").Append(currentUserId).Append("</UserID>");
                        xmlBuilder.Append("</MST_RoughDistribution>");
                    }
                    xmlBuilder.Append("</DocumentElement>");

                    // Raw ADO.NET here (instead of DbHelper.ExecuteNonQueryWithMessage) because
                    // we need both the @MESSAGE output param AND the second result set the SP
                    // returns (the DistributionID(s) just inserted, via OUTPUT INSERTED), so the
                    // QR codes can be generated for exactly the new rows right after Save.
                    using (SqlConnection con = new SqlConnection(conn))
                    using (SqlCommand insCmd = new SqlCommand("RoughDistribution_Insert_Update_Delete", con))
                    {
                        insCmd.CommandType = CommandType.StoredProcedure;
                        insCmd.Parameters.AddWithValue("@XML", xmlBuilder.ToString());
                        insCmd.Parameters.AddWithValue("@ACTION", "INSERT");
                        var messageParam = new SqlParameter("@MESSAGE", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                        insCmd.Parameters.Add(messageParam);

                        con.Open();
                        using (SqlDataReader insRdr = insCmd.ExecuteReader())
                        {
                            while (insRdr.Read())
                            {
                                newDistributionIDs.Add(Convert.ToInt32(insRdr["DistributionID"]));
                            }
                        }
                        Message = Convert.ToString(messageParam.Value);
                    }

                    // Push every newly-created split to its OWN branch's standalone API
                    // only (BranchApiClient maps BranchID -> BranchApiBranch{N}Url) -
                    // BranchApiClient never throws, so a Branch API being down/unreachable
                    // can't fail this save.
                    foreach (int newDistributionID in newDistributionIDs)
                    {
                        BranchApiClient.SyncDistribution(newDistributionID);
                    }
                }

                // Final Success Evaluation
                if (string.IsNullOrEmpty(Message) || Message.ToLower().Contains("inserted") || Message.ToLower().Contains("deleted") || Message.ToLower().Contains("success") || Message.ToLower().Contains("record is"))
                {
                    isSuccess = true;
                    if (string.IsNullOrEmpty(Message))
                        Message = "Distributions updated successfully!";
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = Message, newDistributionIDs = newDistributionIDs });
        }

        [HttpPost]
        public JsonResult RoughDistribution_Delete(int distributionID)
        {
            var deleteList = new List<MST_RoughDistribution>
            {
                new MST_RoughDistribution { DistributionID = distributionID }
            };
            return RoughDistribution_Insert_Update_Delete(deleteList, "DELETE");
        }
        #endregion

        #region Rough Code Master (MST_VoucherDetails)
        [HttpGet]
        public ActionResult RoughCodeMaster()
        {
            SetModulePermissions("Master", "RoughCodeMaster");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canEdit = isAdmin || (ViewBag.CanEdit == true);
            bool canDelete = isAdmin || (ViewBag.CanDelete == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    string[] possibleActions = new string[] { "RoughCodeMaster", "Rough Code Master", "RoughCode Master", "RoughCode" };

                    foreach (string actName in possibleActions)
                    {
                        List<Dictionary<string, object>> permRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                            new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                            new SqlParameter("@ControllerName", "Master"),
                            new SqlParameter("@ActionName", actName));

                        if (permRows.Count > 0)
                        {
                            var row = permRows[0];
                            bool foundPerm = false;
                            foreach (var kvp in row)
                            {
                                string col = kvp.Key;
                                if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canView = true; foundPerm = true; }
                                }
                                else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canAdd = true; foundPerm = true; }
                                }
                                else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canEdit = true; foundPerm = true; }
                                }
                                else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canDelete = true; foundPerm = true; }
                                }
                                else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                                {
                                    bool val = Convert.ToBoolean(kvp.Value);
                                    if (val) { canExport = true; foundPerm = true; }
                                }
                            }
                            if (foundPerm)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            if (canAdd || canEdit || canDelete || canExport)
            {
                canView = true;
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanDelete = canDelete;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        [HttpGet]
        public JsonResult GetRoughCodeMasterDropdowns()
        {
            try
            {
                List<object> _list_Branch = new List<object>();
                List<object> _list_VoucherType = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    // 1. Fetch Branches from MST_GeneralSetting WHERE MasterType = 'Branch'
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT ID, Value FROM MST_GeneralSetting WITH(NOLOCK) WHERE MasterType = 'Branch' ORDER BY Value", con))
                        {
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    _list_Branch.Add(new
                                    {
                                        id = Convert.ToInt32(rdr["ID"]),
                                        value = Convert.ToString(rdr["Value"])
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Fallback query via Getdata_For_DropDown
                        try
                        {
                            using (SqlCommand spCmd = new SqlCommand("Getdata_For_DropDown", con))
                            {
                                spCmd.CommandType = CommandType.StoredProcedure;
                                DataSet ds = new DataSet();
                                SqlDataAdapter da = new SqlDataAdapter(spCmd);
                                da.Fill(ds);
                                if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[5].Rows.Count; i++)
                                    {
                                        var row = ds.Tables[5].Rows[i];
                                        var colId = ds.Tables[5].Columns.Contains("ID") ? "ID" : ds.Tables[5].Columns[0].ColumnName;
                                        var colVal = ds.Tables[5].Columns.Contains("Value") ? "Value" : (ds.Tables[5].Columns.Count > 1 ? ds.Tables[5].Columns[1].ColumnName : colId);
                                        _list_Branch.Add(new
                                        {
                                            id = Convert.ToInt32(row[colId]),
                                            value = Convert.ToString(row[colVal])
                                        });
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    // 2. Fetch Voucher Types from SYS_Module WHERE IsVoucher = 1 (or fallback to all modules)
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT ModuleID, ModuleName, DisplayName FROM SYS_Module WITH(NOLOCK) WHERE ISNULL(IsVoucher, 0) = 1 ORDER BY DisplayName", con))
                        {
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    _list_VoucherType.Add(new
                                    {
                                        id = Convert.ToInt32(rdr["ModuleID"]),
                                        moduleName = Convert.ToString(rdr["ModuleName"]),
                                        value = Convert.ToString(rdr["DisplayName"])
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("SELECT ModuleID, ModuleName, DisplayName FROM SYS_Module WITH(NOLOCK) ORDER BY DisplayName", con))
                            {
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    while (rdr.Read())
                                    {
                                        _list_VoucherType.Add(new
                                        {
                                            id = Convert.ToInt32(rdr["ModuleID"]),
                                            moduleName = Convert.ToString(rdr["ModuleName"]),
                                            value = Convert.ToString(rdr["DisplayName"])
                                        });
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    con.Close();
                }

                // Ensure "Rough Distribution" and standard modules exist in VoucherTypeList
                if (!_list_VoucherType.Any(x => ((dynamic)x).moduleName == "Rough Distribution" || ((dynamic)x).value == "Rough Distribution"))
                {
                    _list_VoucherType.Insert(0, new { id = 999, moduleName = "Rough Distribution", value = "Rough Distribution" });
                }

                return Json(new
                {
                    success = true,
                    BranchList = _list_Branch,
                    VoucherTypeList = _list_VoucherType
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Get_MST_VoucherDetailsList()
        {
            try
            {
                List<MST_VoucherDetails> list = new List<MST_VoucherDetails>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_MST_VoucherDetailsList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        try
                        {
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    list.Add(new MST_VoucherDetails
                                    {
                                        ConfigID = Convert.ToInt32(rdr["ConfigID"]),
                                        BranchID = Convert.ToInt32(rdr["BranchID"]),
                                        BranchName = rdr["BranchName"] != DBNull.Value ? Convert.ToString(rdr["BranchName"]) : "",
                                        VoucherType = Convert.ToString(rdr["VoucherType"]),
                                        VoucherName = rdr["VoucherName"] != DBNull.Value ? Convert.ToString(rdr["VoucherName"]) : "",
                                        NumericMethod = Convert.ToString(rdr["NumericMethod"]),
                                        Prefix = rdr["Prefix"] != DBNull.Value ? Convert.ToString(rdr["Prefix"]) : "",
                                        Separator = rdr["Separator"] != DBNull.Value ? Convert.ToString(rdr["Separator"]) : "",
                                        StartFrom = Convert.ToInt32(rdr["StartFrom"]),
                                        CurrentNo = rdr["CurrentNo"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["CurrentNo"]) : null,
                                        IsActive = Convert.ToBoolean(rdr["IsActive"]),
                                        EntryDate = rdr["EntryDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["EntryDate"]) : null
                                    });
                                }
                            }
                        }
                        catch
                        {
                            using (SqlCommand fallbackCmd = new SqlCommand(@"
                                SELECT 
                                    v.ConfigID,
                                    v.BranchID,
                                    ISNULL(b.Value, '') AS BranchName,
                                    v.VoucherType,
                                    v.VoucherName,
                                    v.NumericMethod,
                                    v.Prefix,
                                    v.Separator,
                                    v.StartFrom,
                                    v.CurrentNo,
                                    v.IsActive,
                                    v.EntryDate
                                FROM MST_VoucherDetails v WITH(NOLOCK)
                                LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = v.BranchID
                                ORDER BY v.ConfigID DESC", con))
                            {
                                using (SqlDataReader rdr = fallbackCmd.ExecuteReader())
                                {
                                    while (rdr.Read())
                                    {
                                        list.Add(new MST_VoucherDetails
                                        {
                                            ConfigID = Convert.ToInt32(rdr["ConfigID"]),
                                            BranchID = Convert.ToInt32(rdr["BranchID"]),
                                            BranchName = rdr["BranchName"] != DBNull.Value ? Convert.ToString(rdr["BranchName"]) : "",
                                            VoucherType = Convert.ToString(rdr["VoucherType"]),
                                            VoucherName = rdr["VoucherName"] != DBNull.Value ? Convert.ToString(rdr["VoucherName"]) : "",
                                            NumericMethod = Convert.ToString(rdr["NumericMethod"]),
                                            Prefix = rdr["Prefix"] != DBNull.Value ? Convert.ToString(rdr["Prefix"]) : "",
                                            Separator = rdr["Separator"] != DBNull.Value ? Convert.ToString(rdr["Separator"]) : "",
                                            StartFrom = Convert.ToInt32(rdr["StartFrom"]),
                                            CurrentNo = rdr["CurrentNo"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["CurrentNo"]) : null,
                                            IsActive = Convert.ToBoolean(rdr["IsActive"]),
                                            EntryDate = rdr["EntryDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["EntryDate"]) : null
                                        });
                                    }
                                }
                            }
                        }
                        con.Close();
                    }
                }

                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_VoucherDetails>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult VoucherDetails_Insert_Update_Delete(MST_VoucherDetails model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("VoucherDetails_Insert_Update_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ConfigID", model.ConfigID);
                        cmd.Parameters.AddWithValue("@BranchID", model.BranchID);
                        cmd.Parameters.AddWithValue("@VoucherType", model.VoucherType ?? "");
                        cmd.Parameters.AddWithValue("@VoucherName", model.VoucherName ?? "");
                        cmd.Parameters.AddWithValue("@NumericMethod", model.NumericMethod ?? "Automatic");
                        cmd.Parameters.AddWithValue("@Prefix", model.Prefix ?? "");
                        cmd.Parameters.AddWithValue("@Separator", model.Separator ?? "");
                        cmd.Parameters.AddWithValue("@StartFrom", model.StartFrom > 0 ? model.StartFrom : 1);
                        cmd.Parameters.AddWithValue("@CurrentNo", (object)model.CurrentNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                        cmd.Parameters.AddWithValue("@ACTION", Action);
                        cmd.Parameters.AddWithValue("@UserID", currentUserId);
                        cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 100);
                        cmd.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;

                        con.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            message = Convert.ToString(cmd.Parameters["@MESSAGE"].Value);
                        }
                        catch
                        {
                            if (Action == "INSERT")
                            {
                                using (SqlCommand insertCmd = new SqlCommand(@"
                                    INSERT INTO MST_VoucherDetails (BranchID, VoucherType, VoucherName, NumericMethod, Prefix, Separator, StartFrom, CurrentNo, IsActive, EntryBy, EntryDate)
                                    VALUES (@BranchID, @VoucherType, @VoucherName, @NumericMethod, @Prefix, @Separator, @StartFrom, @CurrentNo, @IsActive, @UserID, GETDATE())", con))
                                {
                                    insertCmd.Parameters.AddWithValue("@BranchID", model.BranchID);
                                    insertCmd.Parameters.AddWithValue("@VoucherType", model.VoucherType ?? "");
                                    insertCmd.Parameters.AddWithValue("@VoucherName", model.VoucherName ?? "");
                                    insertCmd.Parameters.AddWithValue("@NumericMethod", model.NumericMethod ?? "Automatic");
                                    insertCmd.Parameters.AddWithValue("@Prefix", model.Prefix ?? "");
                                    insertCmd.Parameters.AddWithValue("@Separator", model.Separator ?? "");
                                    insertCmd.Parameters.AddWithValue("@StartFrom", model.StartFrom > 0 ? model.StartFrom : 1);
                                    insertCmd.Parameters.AddWithValue("@CurrentNo", (object)model.CurrentNo ?? DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                                    insertCmd.Parameters.AddWithValue("@UserID", currentUserId);
                                    insertCmd.ExecuteNonQuery();
                                    message = "Record Is Inserted";
                                }
                            }
                            else if (Action == "UPDATE")
                            {
                                using (SqlCommand updateCmd = new SqlCommand(@"
                                    UPDATE MST_VoucherDetails 
                                    SET BranchID = @BranchID, VoucherType = @VoucherType, VoucherName = @VoucherName,
                                        NumericMethod = @NumericMethod, Prefix = @Prefix, Separator = @Separator,
                                        StartFrom = @StartFrom, IsActive = @IsActive, UpdatedBy = @UserID, UpdatedDate = GETDATE()
                                    WHERE ConfigID = @ConfigID", con))
                                {
                                    updateCmd.Parameters.AddWithValue("@ConfigID", model.ConfigID);
                                    updateCmd.Parameters.AddWithValue("@BranchID", model.BranchID);
                                    updateCmd.Parameters.AddWithValue("@VoucherType", model.VoucherType ?? "");
                                    updateCmd.Parameters.AddWithValue("@VoucherName", model.VoucherName ?? "");
                                    updateCmd.Parameters.AddWithValue("@NumericMethod", model.NumericMethod ?? "Automatic");
                                    updateCmd.Parameters.AddWithValue("@Prefix", model.Prefix ?? "");
                                    updateCmd.Parameters.AddWithValue("@Separator", model.Separator ?? "");
                                    updateCmd.Parameters.AddWithValue("@StartFrom", model.StartFrom > 0 ? model.StartFrom : 1);
                                    updateCmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                                    updateCmd.Parameters.AddWithValue("@UserID", currentUserId);
                                    updateCmd.ExecuteNonQuery();
                                    message = "Record Is Updated";
                                }
                            }
                            else if (Action == "DELETE")
                            {
                                using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM MST_VoucherDetails WHERE ConfigID = @ConfigID", con))
                                {
                                    deleteCmd.Parameters.AddWithValue("@ConfigID", model.ConfigID);
                                    deleteCmd.ExecuteNonQuery();
                                    message = "Record Is Deleted";
                                }
                            }
                        }
                        con.Close();
                    }
                }

                if (!string.IsNullOrEmpty(message) && (message.ToLower().Contains("inserted") || message.ToLower().Contains("updated") || message.ToLower().Contains("deleted") || message.ToLower().Contains("success") || message.ToLower().Contains("record is")))
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult Get_RoughCodeConfig(int BranchID, string VoucherType, string ActionType = "GET_CONFIG", string ManualRCode = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_RoughCodeConfig", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@VoucherType", VoucherType ?? "");
                        cmd.Parameters.AddWithValue("@ActionType", ActionType ?? "GET_CONFIG");
                        cmd.Parameters.AddWithValue("@ManualRCode", ManualRCode ?? "");

                        con.Open();

                        if (ActionType == "GET_CONFIG")
                        {
                            Dictionary<string, object> config = null;
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    config = new Dictionary<string, object>();
                                    for (int i = 0; i < rdr.FieldCount; i++)
                                    {
                                        config.Add(rdr.GetName(i), rdr.IsDBNull(i) ? null : rdr.GetValue(i));
                                    }
                                }
                            }
                            con.Close();
                            return Json(new { success = true, Config = config });
                        }
                        else if (ActionType == "GENERATE_AUTO" || ActionType == "PREVIEW_AUTO" || ActionType == "COMMIT_AUTO")
                        {
                            string generatedRCode = "";
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    generatedRCode = rdr["GeneratedRCode"] != DBNull.Value ? Convert.ToString(rdr["GeneratedRCode"]) : "";
                                }
                            }
                            con.Close();
                            return Json(new { success = true, GeneratedRCode = generatedRCode });
                        }
                        else if (ActionType == "CHECK_DUPLICATE")
                        {
                            bool isExists = false;
                            try
                            {
                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (rdr.Read())
                                    {
                                        int count = rdr["IsExists"] != DBNull.Value ? Convert.ToInt32(rdr["IsExists"]) : 0;
                                        isExists = count > 0;
                                    }
                                }
                            }
                            catch
                            {
                                // Direct fallback query in case stored procedure does not support CHECK_DUPLICATE ActionType
                                if (con.State != ConnectionState.Open) con.Open();
                                using (SqlCommand fallbackCmd = new SqlCommand("SELECT COUNT(1) FROM MST_RoughDistribution WITH(NOLOCK) WHERE RCode = @RCode", con))
                                {
                                    fallbackCmd.Parameters.AddWithValue("@RCode", (ManualRCode ?? "").Trim());
                                    int count = Convert.ToInt32(fallbackCmd.ExecuteScalar() ?? 0);
                                    isExists = count > 0;
                                }
                            }
                            con.Close();
                            return Json(new { success = true, IsExists = isExists });
                        }

                        con.Close();
                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Party Master (MST_PartyMaster)

        public ActionResult PartyMaster()
        {
            SetModulePermissions("Master", "PartyMaster");
            return View();
        }

        private static object SafeGetColumn(SqlDataReader rdr, string column)
        {
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                if (string.Equals(rdr.GetName(i), column, StringComparison.OrdinalIgnoreCase))
                {
                    return rdr.IsDBNull(i) ? null : rdr.GetValue(i);
                }
            }
            return null;
        }

        private MST_PartyMaster MapPartyMaster(Dictionary<string, object> rdr)
        {
            return new MST_PartyMaster
            {
                PartyId = rdr["PartyId"] != null ? (Guid)rdr["PartyId"] : Guid.Empty,
                PartyCode = rdr["PartyCode"] != null ? Convert.ToString(rdr["PartyCode"]) : "",
                PartyName = rdr["PartyName"] != null ? Convert.ToString(rdr["PartyName"]) : "",
                ContactPerson = rdr["ContactPerson"] != null ? Convert.ToString(rdr["ContactPerson"]) : "",
                IsOutSide = rdr["IsOutSide"] != null && Convert.ToBoolean(rdr["IsOutSide"]),
                MobileNo = rdr["MobileNo"] != null ? Convert.ToString(rdr["MobileNo"]) : "",
                Email = rdr["Email"] != null ? Convert.ToString(rdr["Email"]) : "",
                Address = rdr["Address"] != null ? Convert.ToString(rdr["Address"]) : "",
                CityName = rdr["CityName"] != null ? Convert.ToString(rdr["CityName"]) : "",
                StateName = rdr["StateName"] != null ? Convert.ToString(rdr["StateName"]) : "",
                CountryName = rdr["CountryName"] != null ? Convert.ToString(rdr["CountryName"]) : "",
                Pincode = rdr["Pincode"] != null ? Convert.ToString(rdr["Pincode"]) : "",
                GSTNo = rdr["GSTNo"] != null ? Convert.ToString(rdr["GSTNo"]) : "",
                PANNo = rdr["PANNo"] != null ? Convert.ToString(rdr["PANNo"]) : "",
                IsActive = rdr["IsActive"] != null && Convert.ToBoolean(rdr["IsActive"]),
                Remarks = rdr["Remarks"] != null ? Convert.ToString(rdr["Remarks"]) : "",
                CreatedOn = rdr["CreatedOn"] != null ? (DateTime?)Convert.ToDateTime(rdr["CreatedOn"]) : null,
                CreatedBy = rdr["CreatedBy"] != null ? (Guid?)rdr["CreatedBy"] : null,
                ModifiedOn = rdr["ModifiedOn"] != null ? (DateTime?)Convert.ToDateTime(rdr["ModifiedOn"]) : null,
                ModifiedBy = rdr["ModifiedBy"] != null ? (Guid?)rdr["ModifiedBy"] : null
            };
        }

        public JsonResult Get_MST_PartyMasterList(string SearchText)
        {
            try
            {
                List<MST_PartyMaster> list = new List<MST_PartyMaster>();

                List<Dictionary<string, object>> partyRows = DbHelper.ExecuteReaderAsList("USP_MST_PartyMaster_GetList",
                    new SqlParameter("@SearchText", string.IsNullOrWhiteSpace(SearchText) ? (object)DBNull.Value : SearchText));

                foreach (var row in partyRows)
                {
                    list.Add(MapPartyMaster(row));
                }

                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_PartyMaster>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_PartyMasterById(Guid PartyId)
        {
            try
            {
                MST_PartyMaster item = null;

                List<Dictionary<string, object>> partyRows = DbHelper.ExecuteReaderAsList("USP_MST_PartyMaster_GetById",
                    new SqlParameter("@PartyId", PartyId));

                if (partyRows.Count > 0)
                {
                    item = MapPartyMaster(partyRows[0]);
                }

                return Json(new { success = item != null, item = item }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PartyMaster_Save(MST_PartyMaster model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                bool isUpdate = string.Equals(Action, "UPDATE", StringComparison.OrdinalIgnoreCase) && model.PartyId != Guid.Empty;

                List<SqlParameter> saveParams = new List<SqlParameter>();
                if (isUpdate)
                {
                    saveParams.Add(new SqlParameter("@PartyId", model.PartyId));
                }
                saveParams.Add(new SqlParameter("@PartyCode", model.PartyCode ?? ""));
                saveParams.Add(new SqlParameter("@PartyName", model.PartyName ?? ""));
                saveParams.Add(new SqlParameter("@ContactPerson", (object)model.ContactPerson ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@IsOutSide", model.IsOutSide));
                saveParams.Add(new SqlParameter("@MobileNo", (object)model.MobileNo ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@Email", (object)model.Email ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@Address", (object)model.Address ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@CityName", (object)model.CityName ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@StateName", (object)model.StateName ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@CountryName", (object)model.CountryName ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@Pincode", (object)model.Pincode ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@GSTNo", (object)model.GSTNo ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@PANNo", (object)model.PANNo ?? DBNull.Value));
                saveParams.Add(new SqlParameter("@Remarks", (object)model.Remarks ?? DBNull.Value));
                saveParams.Add(new SqlParameter(isUpdate ? "@ModifiedBy" : "@CreatedBy", DBNull.Value));

                List<Dictionary<string, object>> saveRows = DbHelper.ExecuteReaderAsList("USP_MST_PartyMaster_Save", saveParams.ToArray());
                if (saveRows.Count > 0)
                {
                    var row = saveRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
                else
                {
                    isSuccess = true;
                }

                if (isSuccess && string.IsNullOrEmpty(message))
                {
                    message = isUpdate ? "Party updated successfully." : "Party saved successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult PartyMaster_Delete(Guid PartyId)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                List<Dictionary<string, object>> deleteRows = DbHelper.ExecuteReaderAsList("USP_MST_PartyMaster_Delete",
                    new SqlParameter("@PartyId", PartyId),
                    new SqlParameter("@ModifiedBy", DBNull.Value));

                if (deleteRows.Count > 0)
                {
                    var row = deleteRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult PartyMaster_ActiveInactive(Guid PartyId, bool IsActive)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                List<Dictionary<string, object>> activeInactiveRows = DbHelper.ExecuteReaderAsList("USP_MST_PartyMaster_ActiveInactive",
                    new SqlParameter("@PartyId", PartyId),
                    new SqlParameter("@IsActive", IsActive),
                    new SqlParameter("@ModifiedBy", DBNull.Value));

                if (activeInactiveRows.Count > 0)
                {
                    var row = activeInactiveRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        #endregion

        #region Branch Master (MST_CompanyBranch)

        public ActionResult BranchMaster()
        {
            SetModulePermissions("Master", "BranchMaster");
            return View();
        }

        #region Priority Master

        public ActionResult PriorityMaster()
        {
            SetModulePermissions("Master", "PriorityMaster");
            return View();
        }

        private MST_Priority MapPriority(Dictionary<string, object> rdr)
        {
            return new MST_Priority
            {
                PriorityID = rdr["PriorityID"] != null ? Convert.ToInt32(rdr["PriorityID"]) : 0,
                PriorityName = rdr["PriorityName"] != null ? Convert.ToString(rdr["PriorityName"]) : "",
                ColorHex = rdr["ColorHex"] != null ? Convert.ToString(rdr["ColorHex"]) : "",
                IsDefault = rdr["IsDefault"] != null && Convert.ToBoolean(rdr["IsDefault"]),
                IsActive = rdr["IsActive"] != null && Convert.ToBoolean(rdr["IsActive"]),
                CreatedBy = rdr["CreatedBy"] != null ? Convert.ToInt32(rdr["CreatedBy"]) : 0,
                CreatedOn = rdr["CreatedOn"] != null ? (DateTime?)Convert.ToDateTime(rdr["CreatedOn"]) : null,
                ModifiedBy = rdr["ModifiedBy"] != null ? (int?)Convert.ToInt32(rdr["ModifiedBy"]) : null,
                ModifiedOn = rdr["ModifiedOn"] != null ? (DateTime?)Convert.ToDateTime(rdr["ModifiedOn"]) : null
            };
        }

        public JsonResult Get_MST_PriorityList(string SearchText)
        {
            try
            {
                List<MST_Priority> list = new List<MST_Priority>();

                List<Dictionary<string, object>> priorityRows = DbHelper.ExecuteReaderAsList("USP_MST_Priority_GetList",
                    new SqlParameter("@SearchText", string.IsNullOrWhiteSpace(SearchText) ? (object)DBNull.Value : SearchText));

                foreach (var row in priorityRows)
                {
                    list.Add(MapPriority(row));
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_Priority>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Get_MST_PriorityDropdownList()
        {
            try
            {
                List<Dictionary<string, object>> list = DbHelper.ExecuteReaderAsList("USP_MST_Priority_DropdownList");
                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, list = new List<object>(), message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_PriorityById(int PriorityID)
        {
            try
            {
                MST_Priority item = null;

                List<Dictionary<string, object>> priorityRows = DbHelper.ExecuteReaderAsList("USP_MST_Priority_GetById",
                    new SqlParameter("@PriorityID", PriorityID));

                if (priorityRows.Count > 0)
                {
                    item = MapPriority(priorityRows[0]);
                }

                return Json(new { success = item != null, item = item }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PriorityMaster_Save(MST_Priority model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                bool isUpdate = string.Equals(Action, "UPDATE", StringComparison.OrdinalIgnoreCase) && model.PriorityID > 0;

                string spName = isUpdate ? "USP_MST_Priority_Update" : "USP_MST_Priority_Insert";
                List<SqlParameter> priorityParams = new List<SqlParameter>();
                if (isUpdate)
                {
                    priorityParams.Add(new SqlParameter("@PriorityID", model.PriorityID));
                }
                priorityParams.Add(new SqlParameter("@PriorityName", (model.PriorityName ?? "").Trim().ToUpper()));
                priorityParams.Add(new SqlParameter("@ColorHex", string.IsNullOrWhiteSpace(model.ColorHex) ? "#0284c7" : model.ColorHex));
                priorityParams.Add(new SqlParameter("@IsDefault", model.IsDefault));
                priorityParams.Add(new SqlParameter(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId));

                List<Dictionary<string, object>> prioritySaveRows = DbHelper.ExecuteReaderAsList(spName, priorityParams.ToArray());
                if (prioritySaveRows.Count > 0)
                {
                    var row = prioritySaveRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }

                if (isSuccess && string.IsNullOrEmpty(message))
                {
                    message = isUpdate ? "Priority updated successfully." : "Priority saved successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult PriorityMaster_Delete(int PriorityID)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                List<Dictionary<string, object>> priorityDeleteRows = DbHelper.ExecuteReaderAsList("USP_MST_Priority_Delete",
                    new SqlParameter("@PriorityID", PriorityID),
                    new SqlParameter("@ModifiedBy", currentUserId));

                if (priorityDeleteRows.Count > 0)
                {
                    var row = priorityDeleteRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult PriorityMaster_ActiveInactive(int PriorityID, bool IsActive)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                List<Dictionary<string, object>> activeInactiveRows = DbHelper.ExecuteReaderAsList("USP_MST_Priority_ActiveInactive",
                    new SqlParameter("@PriorityID", PriorityID),
                    new SqlParameter("@IsActive", IsActive),
                    new SqlParameter("@ModifiedBy", currentUserId));

                if (activeInactiveRows.Count > 0)
                {
                    var row = activeInactiveRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        #endregion

        private MST_CompanyBranch MapBranch(Dictionary<string, object> rdr)
        {
            return new MST_CompanyBranch
            {
                BranchID = rdr["BranchID"] != null ? Convert.ToInt32(rdr["BranchID"]) : 0,
                BranchCode = rdr["BranchCode"] != null ? Convert.ToString(rdr["BranchCode"]) : "",
                BranchName = rdr["BranchName"] != null ? Convert.ToString(rdr["BranchName"]) : "",
                BranchTypeID = rdr["CompanyBranchTypeID"] != null ? Convert.ToInt32(rdr["CompanyBranchTypeID"]) : 0,
                BranchTypeName = rdr.ContainsKey("BranchTypeName") && rdr["BranchTypeName"] != null ? Convert.ToString(rdr["BranchTypeName"]) : "",
                GSTIN = rdr["GSTIN"] != null ? Convert.ToString(rdr["GSTIN"]) : "",
                StateCode = rdr["StateCode"] != null ? Convert.ToString(rdr["StateCode"]) : "",
                Address = rdr["Address"] != null ? Convert.ToString(rdr["Address"]) : "",
                City = rdr["City"] != null ? Convert.ToString(rdr["City"]) : "",
                ContactPerson = rdr["ContactPerson"] != null ? Convert.ToString(rdr["ContactPerson"]) : "",
                Phone = rdr["Phone"] != null ? Convert.ToString(rdr["Phone"]) : "",
                IsActive = rdr["IsActive"] != null && Convert.ToBoolean(rdr["IsActive"]),
                CreatedBy = rdr["CreatedBy"] != null ? Convert.ToInt32(rdr["CreatedBy"]) : 0,
                CreatedOn = rdr["CreatedOn"] != null ? (DateTime?)Convert.ToDateTime(rdr["CreatedOn"]) : null,
                ModifiedBy = rdr["ModifiedBy"] != null ? (int?)Convert.ToInt32(rdr["ModifiedBy"]) : null,
                ModifiedOn = rdr["ModifiedOn"] != null ? (DateTime?)Convert.ToDateTime(rdr["ModifiedOn"]) : null
            };
        }

        public JsonResult Get_MST_BranchTypeList()
        {
            try
            {
                List<MST_BranchType> list = new List<MST_BranchType>();

                List<Dictionary<string, object>> branchTypeRows = DbHelper.ExecuteReaderAsList("USP_MST_CompanyBranchType_GetList");
                foreach (var row in branchTypeRows)
                {
                    list.Add(new MST_BranchType
                    {
                        BranchTypeID = Convert.ToInt32(row["ID"]),
                        BranchTypeName = row["BranchTypeName"] != null ? Convert.ToString(row["BranchTypeName"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_BranchType>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_BranchList(string SearchText)
        {
            try
            {
                List<MST_CompanyBranch> list = new List<MST_CompanyBranch>();

                List<Dictionary<string, object>> branchRows = DbHelper.ExecuteReaderAsList("USP_MST_CompanyBranch_GetList",
                    new SqlParameter("@SearchText", string.IsNullOrWhiteSpace(SearchText) ? (object)DBNull.Value : SearchText));

                foreach (var row in branchRows)
                {
                    list.Add(MapBranch(row));
                }

                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_CompanyBranch>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_BranchById(int BranchID)
        {
            try
            {
                MST_CompanyBranch item = null;

                List<Dictionary<string, object>> branchRows = DbHelper.ExecuteReaderAsList("USP_MST_CompanyBranch_GetById",
                    new SqlParameter("@BranchID", BranchID));

                if (branchRows.Count > 0)
                {
                    item = MapBranch(branchRows[0]);
                }

                return Json(new { success = item != null, item = item }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult BranchMaster_Save(MST_CompanyBranch model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                bool isUpdate = string.Equals(Action, "UPDATE", StringComparison.OrdinalIgnoreCase) && model.BranchID > 0;

                string spName = isUpdate ? "USP_MST_CompanyBranch_Update" : "USP_MST_CompanyBranch_Insert";
                List<SqlParameter> branchParams = new List<SqlParameter>();
                if (isUpdate)
                {
                    branchParams.Add(new SqlParameter("@BranchID", model.BranchID));
                }
                branchParams.Add(new SqlParameter("@BranchCode", model.BranchCode ?? ""));
                branchParams.Add(new SqlParameter("@BranchName", model.BranchName ?? ""));
                branchParams.Add(new SqlParameter("@CompanyBranchTypeID", model.BranchTypeID));
                branchParams.Add(new SqlParameter("@GSTIN", (object)model.GSTIN ?? DBNull.Value));
                branchParams.Add(new SqlParameter("@StateCode", (object)model.StateCode ?? DBNull.Value));
                branchParams.Add(new SqlParameter("@Address", (object)model.Address ?? DBNull.Value));
                branchParams.Add(new SqlParameter("@City", (object)model.City ?? DBNull.Value));
                branchParams.Add(new SqlParameter("@ContactPerson", (object)model.ContactPerson ?? DBNull.Value));
                branchParams.Add(new SqlParameter("@Phone", (object)model.Phone ?? DBNull.Value));
                branchParams.Add(new SqlParameter(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId));

                List<Dictionary<string, object>> branchSaveRows = DbHelper.ExecuteReaderAsList(spName, branchParams.ToArray());
                if (branchSaveRows.Count > 0)
                {
                    var row = branchSaveRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }

                if (isSuccess && string.IsNullOrEmpty(message))
                {
                    message = isUpdate ? "Branch updated successfully." : "Branch saved successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult BranchMaster_Delete(int BranchID)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                List<Dictionary<string, object>> branchDeleteRows = DbHelper.ExecuteReaderAsList("USP_MST_CompanyBranch_Delete",
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@ModifiedBy", currentUserId));

                if (branchDeleteRows.Count > 0)
                {
                    var row = branchDeleteRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        [HttpPost]
        public JsonResult BranchMaster_ActiveInactive(int BranchID, bool IsActive)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                List<Dictionary<string, object>> activeInactiveRows = DbHelper.ExecuteReaderAsList("USP_MST_CompanyBranch_ActiveInactive",
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@IsActive", IsActive),
                    new SqlParameter("@ModifiedBy", currentUserId));

                if (activeInactiveRows.Count > 0)
                {
                    var row = activeInactiveRows[0];
                    object successVal = row.ContainsKey("Success") ? row["Success"] : null;
                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                    object msgVal = row.ContainsKey("Message") ? row["Message"] : null;
                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }

        #endregion

        #region Rough Inward (TRN_RoughInward)

        public ActionResult RoughInward()
        {
            SetModulePermissions("Master", "RoughInward");
            return View();
        }

        public JsonResult Get_MST_PurposeList()
        {
            try
            {
                List<MST_Purpose> list = new List<MST_Purpose>();

                List<Dictionary<string, object>> purposeRows = DbHelper.ExecuteReaderAsList("USP_MST_Purpose_GetList");
                foreach (var row in purposeRows)
                {
                    list.Add(new MST_Purpose
                    {
                        PurposeID = Convert.ToInt32(row["PurposeID"]),
                        PurposeName = row["PurposeName"] != null ? Convert.ToString(row["PurposeName"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_Purpose>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_ReceipeList()
        {
            try
            {
                List<MST_Receipe> list = new List<MST_Receipe>();

                List<Dictionary<string, object>> receipeRows = DbHelper.ExecuteReaderAsList("USP_MST_Receipe_GetList");
                foreach (var row in receipeRows)
                {
                    list.Add(new MST_Receipe
                    {
                        ReceipeID = Convert.ToInt32(row["ReceipeID"]),
                        Receipe = row["Receipe"] != null ? Convert.ToString(row["Receipe"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_Receipe>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_GradeList()
        {
            try
            {
                List<MST_Grade> list = new List<MST_Grade>();

                List<Dictionary<string, object>> gradeRows = DbHelper.ExecuteReaderAsList("USP_MST_Grade_GetList");
                foreach (var row in gradeRows)
                {
                    list.Add(new MST_Grade
                    {
                        GradeID = Convert.ToInt32(row["ID"]),
                        Grade = row["Grade"] != null ? Convert.ToString(row["Grade"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_Grade>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_FactoryCodeList()
        {
            try
            {
                List<MST_FactoryCode> list = new List<MST_FactoryCode>();

                List<Dictionary<string, object>> factoryCodeRows = DbHelper.ExecuteReaderAsList("USP_MST_FactoryCode_GetList");
                foreach (var row in factoryCodeRows)
                {
                    list.Add(new MST_FactoryCode
                    {
                        FactoryCodeID = Convert.ToInt32(row["ID"]),
                        FactoryCode = row["FactoryCode"] != null ? Convert.ToString(row["FactoryCode"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_FactoryCode>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_SizeCodeList()
        {
            try
            {
                var list = new List<object>();

                List<Dictionary<string, object>> sizeCodeRows = DbHelper.ExecuteReaderAsList("USP_MST_SizeCode_GetList");
                foreach (var row in sizeCodeRows)
                {
                    list.Add(new
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        ShortName = row["ShortName"] != null ? Convert.ToString(row["ShortName"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_MST_DimensionList()
        {
            try
            {
                var list = new List<object>();

                List<Dictionary<string, object>> dimensionRows = DbHelper.ExecuteReaderAsList("USP_MST_Dimension_GetList");
                foreach (var row in dimensionRows)
                {
                    list.Add(new
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        DimensionName = row["DimensionName"] != null ? Convert.ToString(row["DimensionName"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Size And Final ShapeGroup Wise Price Discount

        public ActionResult SizeAndFnlShpGrpWisePriceDisc()
        {
            SetModulePermissions("Master", "SizeAndFnlShpGrpWisePriceDisc");
            return View();
        }

        public JsonResult Get_MST_Size_ShapeTypeList()
        {
            try
            {
                var list = new List<object>();

                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_Mst_Size_GetShapeTypeList");
                foreach (var row in rows)
                {
                    list.Add(new
                    {
                        ShapeType = row["ShapeType"] != null ? Convert.ToString(row["ShapeType"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_Mst_ShapeFinalGroupList(string shapeType)
        {
            try
            {
                var list = new List<object>();

                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_Mst_ShapeFinalGroup_GetList",
                    new SqlParameter("@ShapeType", shapeType));
                foreach (var row in rows)
                {
                    list.Add(new
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        ShapeGroup = row["ShapeGroup"] != null ? Convert.ToString(row["ShapeGroup"]) : ""
                    });
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_SizeAndFnlShpGrpWisePriceDisc_GridData(string shapeType, string type)
        {
            try
            {
                var sizeList = new List<object>();
                var discList = new List<object>();

                DataSet ds = DbHelper.ExecuteDataSet("USP_Mst_SizeAndFnlShpGrpWisePriceDisc_GetGridData",
                    new SqlParameter("@ShapeType", shapeType),
                    new SqlParameter("@Type", type));

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        sizeList.Add(new
                        {
                            SizeID = Convert.ToInt32(row["SizeID"]),
                            FromSize = row["FromSize"] != DBNull.Value ? Convert.ToDecimal(row["FromSize"]) : 0,
                            ToSize = row["ToSize"] != DBNull.Value ? Convert.ToDecimal(row["ToSize"]) : 0
                        });
                    }
                }

                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        discList.Add(new
                        {
                            MstSizeID = Convert.ToInt32(row["MstSizeID"]),
                            MStFinalShapGroupID = Convert.ToInt32(row["MStFinalShapGroupID"]),
                            DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToDecimal(row["DiscountPer"]) : (decimal?)null
                        });
                    }
                }

                return Json(new { success = true, sizeList = sizeList, discList = discList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, sizeList = new List<object>(), discList = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save_SizeAndFnlShpGrpWisePriceDisc(List<MST_SizeAndFnlShpGrpWisePriceDisc> list)
        {
            string message;
            try
            {
                string xmlStr = "";
                if (list != null && list.Count > 0)
                {
                    XmlDocument xmlData = CommonMethods.ConvertToXml(list);
                    xmlStr = "<DocumentElement>" + xmlData.DocumentElement.InnerXml + "</DocumentElement>";
                }

                message = DbHelper.ExecuteNonQueryWithMessage("USP_Mst_SizeAndFnlShpGrpWisePriceDisc_Save",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@CreatedBy", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + ex.Message;
            }

            return Json(new { success = message.ToLower().Contains("success"), message = message });
        }

        #endregion

        public JsonResult Get_UserBranch()
        {
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                int? branchId = null;
                string branchName = null;

                List<Dictionary<string, object>> userBranchRows = DbHelper.ExecuteReaderAsList("USP_SEC_User_GetBranch",
                    new SqlParameter("@UserID", currentUserId));

                if (userBranchRows.Count > 0)
                {
                    var row = userBranchRows[0];
                    branchId = row["BranchID"] != null ? (int?)Convert.ToInt32(row["BranchID"]) : null;
                    branchName = row["BranchName"] != null ? Convert.ToString(row["BranchName"]) : null;
                }

                return Json(new { success = branchId != null, branchId = branchId, branchName = branchName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private TRN_RoughInward MapRoughInwardHeader(SqlDataReader rdr)
        {
            DateTime cDate = rdr["ChallanDate"] != DBNull.Value ? Convert.ToDateTime(rdr["ChallanDate"]) : DateTime.MinValue;
            return new TRN_RoughInward
            {
                InwardID = Convert.ToInt32(rdr["InwardID"]),
                ChallanNo = rdr["ChallanNo"] != DBNull.Value ? Convert.ToString(rdr["ChallanNo"]) : "",
                ChallanDate = cDate,
                ChallanDateStr = cDate > new DateTime(1753, 1, 1) ? cDate.ToString("yyyy-MM-dd") : "",
                FactoryCodeID = rdr["FactoryCodeID"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["FactoryCodeID"]) : null,
                FactoryCodeName = SafeGetColumn(rdr, "FactoryCodeName") != null ? Convert.ToString(SafeGetColumn(rdr, "FactoryCodeName")) : "",
                SourceType = rdr["SourceType"] != DBNull.Value ? Convert.ToString(rdr["SourceType"]) : "",
                FromBranchID = rdr["FromBranchID"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["FromBranchID"]) : null,
                FromBranchName = SafeGetColumn(rdr, "FromBranchName") != null ? Convert.ToString(SafeGetColumn(rdr, "FromBranchName")) : "",
                FromPartyID = rdr["FromPartyID"] != DBNull.Value ? (Guid?)rdr["FromPartyID"] : null,
                PartyName = SafeGetColumn(rdr, "PartyName") != null ? Convert.ToString(SafeGetColumn(rdr, "PartyName")) : "",
                ToBranchID = rdr["ToBranchID"] != DBNull.Value ? Convert.ToInt32(rdr["ToBranchID"]) : 0,
                ToBranchName = SafeGetColumn(rdr, "ToBranchName") != null ? Convert.ToString(SafeGetColumn(rdr, "ToBranchName")) : "",
                PurposeID = rdr["PurposeID"] != DBNull.Value ? Convert.ToInt32(rdr["PurposeID"]) : 0,
                PurposeName = SafeGetColumn(rdr, "PurposeName") != null ? Convert.ToString(SafeGetColumn(rdr, "PurposeName")) : "",
                SizeCode = SafeGetColumn(rdr, "SizeCode") != null ? Convert.ToString(SafeGetColumn(rdr, "SizeCode")) : "",
                ColorType = SafeGetColumn(rdr, "ColorType") != null ? Convert.ToString(SafeGetColumn(rdr, "ColorType")) : "",
                Remarks = rdr["Remarks"] != DBNull.Value ? Convert.ToString(rdr["Remarks"]) : "",
                TotalPcs = rdr["TotalPcs"] != DBNull.Value ? Convert.ToInt32(rdr["TotalPcs"]) : 0,
                TotalCarat = rdr["TotalCarat"] != DBNull.Value ? Convert.ToDecimal(rdr["TotalCarat"]) : 0,
                TotalAmount = rdr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(rdr["TotalAmount"]) : 0,
                IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                CreatedBy = rdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(rdr["CreatedBy"]) : 0,
                CreatedOn = rdr["CreatedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["CreatedOn"]) : null,
                ModifiedBy = rdr["ModifiedBy"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["ModifiedBy"]) : null,
                ModifiedOn = rdr["ModifiedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["ModifiedOn"]) : null
            };
        }

        public JsonResult Get_TRN_RoughInwardList(string SearchText)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_TRN_RoughInward_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", string.IsNullOrWhiteSpace(SearchText) ? (object)DBNull.Value : SearchText);
                    cmd.Parameters.AddWithValue("@ToBranchID", DBNull.Value);

                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    con.Close();
                }

                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataColumn col in dt.Columns)
                    {
                        object val = row[col];
                        if (val == DBNull.Value)
                        {
                            dict[col.ColumnName] = null;
                        }
                        else if (col.DataType == typeof(DateTime))
                        {
                            dict[col.ColumnName] = Convert.ToDateTime(val).ToString("yyyy-MM-dd");
                        }
                        else if (col.DataType == typeof(string))
                        {
                            dict[col.ColumnName] = val.ToString().Trim();
                        }
                        else
                        {
                            dict[col.ColumnName] = val;
                        }
                    }
                    list.Add(dict);
                }

                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_TRN_RoughInwardById(int InwardID)
        {
            try
            {
                TRN_RoughInward item = null;

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_TRN_RoughInward_GetById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InwardID", InwardID);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            item = MapRoughInwardHeader(rdr);
                        }

                        if (item != null)
                        {
                            item.Details = new List<TRN_RoughInwardDetail>();
                            if (rdr.NextResult())
                            {
                                while (rdr.Read())
                                {
                                    item.Details.Add(new TRN_RoughInwardDetail
                                    {
                                        InwardDetailID = Convert.ToInt32(rdr["InwardDetailID"]),
                                        InwardID = Convert.ToInt32(rdr["InwardID"]),
                                        SrNo = rdr["SrNo"] != DBNull.Value ? Convert.ToInt32(rdr["SrNo"]) : 0,
                                        LotNo = rdr["LotNo"] != DBNull.Value ? Convert.ToString(rdr["LotNo"]) : "",
                                        GrowthRate = rdr["GrowthRate"] != DBNull.Value ? Convert.ToString(rdr["GrowthRate"]) : "",
                                        AvgGrowthRate = rdr["AvgGrowthRate"] != DBNull.Value ? (decimal?)Convert.ToDecimal(rdr["AvgGrowthRate"]) : null,
                                        RecipeID = rdr["RecipeID"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["RecipeID"]) : null,
                                        RecipeName = SafeGetColumn(rdr, "RecipeName") != null ? Convert.ToString(SafeGetColumn(rdr, "RecipeName")) : "",
                                        GradeID = rdr["GradeID"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["GradeID"]) : null,
                                        GradeName = SafeGetColumn(rdr, "GradeName") != null ? Convert.ToString(SafeGetColumn(rdr, "GradeName")) : "",
                                        Pcs = rdr["Pcs"] != DBNull.Value ? Convert.ToInt32(rdr["Pcs"]) : 0,
                                        Carat = rdr["Carat"] != DBNull.Value ? Convert.ToDecimal(rdr["Carat"]) : 0,
                                        Rate = rdr["Rate"] != DBNull.Value ? (decimal?)Convert.ToDecimal(rdr["Rate"]) : null,
                                        Amount = rdr["Amount"] != DBNull.Value ? (decimal?)Convert.ToDecimal(rdr["Amount"]) : null,
                                        Remarks = rdr["Remarks"] != DBNull.Value ? Convert.ToString(rdr["Remarks"]) : "",
                                        MachineNo = SafeGetColumn(rdr, "MachineNo") != null ? Convert.ToString(SafeGetColumn(rdr, "MachineNo")) : "",
                                        Size = SafeGetColumn(rdr, "Size") != null ? Convert.ToString(SafeGetColumn(rdr, "Size")) : "",
                                        DimensionID = SafeGetColumn(rdr, "DimensionID") != null ? (int?)Convert.ToInt32(SafeGetColumn(rdr, "DimensionID")) : null,
                                        DimensionName = SafeGetColumn(rdr, "DimensionName") != null ? Convert.ToString(SafeGetColumn(rdr, "DimensionName")) : "",
                                        FromHeight = SafeGetColumn(rdr, "FromHeight") != null ? (decimal?)Convert.ToDecimal(SafeGetColumn(rdr, "FromHeight")) : null,
                                        ToHeight = SafeGetColumn(rdr, "ToHeight") != null ? (decimal?)Convert.ToDecimal(SafeGetColumn(rdr, "ToHeight")) : null,
                                        Hours = SafeGetColumn(rdr, "Hours") != null ? (decimal?)Convert.ToDecimal(SafeGetColumn(rdr, "Hours")) : null,
                                        BoxNo = SafeGetColumn(rdr, "BoxNo") != null ? Convert.ToString(SafeGetColumn(rdr, "BoxNo")) : "",
                                        SizeCode = SafeGetColumn(rdr, "SizeCode") != null ? Convert.ToString(SafeGetColumn(rdr, "SizeCode")) : "",
                                        SizeCodeId = SafeGetColumn(rdr, "SizeCodeId") != null ? (int?)Convert.ToInt32(SafeGetColumn(rdr, "SizeCodeId")) : null,
                                        ColorType = SafeGetColumn(rdr, "ColorType") != null ? Convert.ToString(SafeGetColumn(rdr, "ColorType")) : "",
                                        //NotAllow = SafeGetColumn(rdr, "NotAllow") != null ? Convert.ToBoolean(SafeGetColumn(rdr, "NotAllow")) : false,
                                        NotAllow = SafeGetColumn(rdr, "NotAllow") != null ? (Convert.ToInt32(SafeGetColumn(rdr, "NotAllow")) == 1) : false,
                                    });
                                }
                            }
                        }
                    }
                    con.Close();
                }

                string cDateFormatted = "";
                if (item != null && item.ChallanDate > new DateTime(1753, 1, 1))
                {
                    cDateFormatted = item.ChallanDate.ToString("yyyy-MM-dd");
                }

                return Json(new { success = item != null, item = item, challanDateFormatted = cDateFormatted }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult RoughInward_Save(TRN_RoughInward model, string Action = "INSERT")
        //{
        //    string message = "";
        //    bool isSuccess = false;
        //    try
        //    {
        //        int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
        //        bool isUpdate = string.Equals(Action, "UPDATE", StringComparison.OrdinalIgnoreCase) && model.InwardID > 0;

        //        if (string.Equals(model.SourceType, "BRANCH", StringComparison.OrdinalIgnoreCase)
        //            && model.FromBranchID.HasValue && model.FromBranchID.Value == model.ToBranchID)
        //        {
        //            return Json(new { success = false, message = "From Branch and To Branch cannot be the same." });
        //        }

        //        using (SqlConnection con = new SqlConnection(conn))
        //        {
        //            con.Open();
        //            using (SqlTransaction tran = con.BeginTransaction())
        //            {
        //                try
        //                {
        //                    int inwardId;
        //                    string spName = isUpdate ? "USP_TRN_RoughInward_Update" : "USP_TRN_RoughInward_Insert";

        //                    using (SqlCommand cmd = new SqlCommand(spName, con, tran))
        //                    {
        //                        cmd.CommandType = CommandType.StoredProcedure;
        //                        if (isUpdate)
        //                        {
        //                            cmd.Parameters.AddWithValue("@InwardID", model.InwardID);
        //                        }

        //                        DateTime saveChallanDate = model.ChallanDate;
        //                        if (!string.IsNullOrWhiteSpace(model.ChallanDateStr))
        //                        {
        //                            DateTime parsed;
        //                            if (DateTime.TryParse(model.ChallanDateStr, out parsed) && parsed >= new DateTime(1753, 1, 1))
        //                            {
        //                                saveChallanDate = parsed;
        //                            }
        //                        }
        //                        if (saveChallanDate < new DateTime(1753, 1, 1))
        //                        {
        //                            saveChallanDate = DateTime.Today;
        //                        }

        //                        cmd.Parameters.AddWithValue("@ChallanNo", model.ChallanNo ?? "");
        //                        cmd.Parameters.AddWithValue("@ChallanDate", saveChallanDate);
        //                        cmd.Parameters.AddWithValue("@FactoryCodeID", (object)model.FactoryCodeID ?? DBNull.Value);
        //                        cmd.Parameters.AddWithValue("@SourceType", model.SourceType ?? "");
        //                        cmd.Parameters.AddWithValue("@FromBranchID", (object)model.FromBranchID ?? DBNull.Value);
        //                        cmd.Parameters.AddWithValue("@FromPartyID", (object)model.FromPartyID ?? DBNull.Value);
        //                        cmd.Parameters.AddWithValue("@ToBranchID", model.ToBranchID);
        //                        cmd.Parameters.AddWithValue("@PurposeID", model.PurposeID);
        //                        cmd.Parameters.AddWithValue("@Remarks", (object)model.Remarks ?? DBNull.Value);
        //                        cmd.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId);

        //                        using (SqlDataReader rdr = cmd.ExecuteReader())
        //                        {
        //                            if (!rdr.Read())
        //                            {
        //                                throw new Exception("No response from server while saving Rough Inward.");
        //                            }
        //                            object successVal = SafeGetColumn(rdr, "Success");
        //                            isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
        //                            object msgVal = SafeGetColumn(rdr, "Message");
        //                            message = msgVal != null ? Convert.ToString(msgVal) : "";
        //                            object idVal = SafeGetColumn(rdr, "InwardID");
        //                            inwardId = idVal != null ? Convert.ToInt32(idVal) : model.InwardID;
        //                        }
        //                    }

        //                    if (!isSuccess)
        //                    {
        //                        tran.Rollback();
        //                        return Json(new { success = false, message = message });
        //                    }

        //                    //if (isUpdate)
        //                    //{
        //                    //    using (SqlCommand delCmd = new SqlCommand("USP_TRN_RoughInwardDetail_DeleteByInward", con, tran))
        //                    //    {
        //                    //        delCmd.CommandType = CommandType.StoredProcedure;
        //                    //        delCmd.Parameters.AddWithValue("@InwardID", inwardId);
        //                    //        delCmd.ExecuteNonQuery();
        //                    //    }
        //                    //}

        //                    if (model.Details != null)
        //                    {
        //                        int srNo = 1;
        //                        foreach (var line in model.Details)
        //                        {
        //                            if (string.IsNullOrWhiteSpace(line.LotNo)) continue;


        //                            if (line.InwardDetailID != 0)
        //                            { 



        //                            }
        //                            else
        //                            {
        //                                using (SqlCommand lineCmd = new SqlCommand("USP_TRN_RoughInwardDetail_Insert", con, tran))
        //                                {
        //                                    lineCmd.CommandType = CommandType.StoredProcedure;
        //                                    lineCmd.Parameters.AddWithValue("@InwardID", inwardId);
        //                                    lineCmd.Parameters.AddWithValue("@SrNo", srNo);
        //                                    lineCmd.Parameters.AddWithValue("@LotNo", line.LotNo ?? "");
        //                                    lineCmd.Parameters.AddWithValue("@GrowthRate", (object)line.GrowthRate ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@AvgGrowthRate", (object)line.AvgGrowthRate ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@RecipeID", (object)line.RecipeID ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@GradeID", (object)line.GradeID ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@Pcs", line.Pcs);
        //                                    lineCmd.Parameters.AddWithValue("@Carat", line.Carat);
        //                                    lineCmd.Parameters.AddWithValue("@Rate", (object)line.Rate ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@Amount", (object)line.Amount ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@Remarks", (object)line.Remarks ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@MachineNo", (object)line.MachineNo ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@Size", (object)line.Size ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@FromHeight", (object)line.FromHeight ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@ToHeight", (object)line.ToHeight ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@Hours", (object)line.Hours ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@BoxNo", (object)line.BoxNo ?? DBNull.Value);
        //                                    string lineSizeCode = !string.IsNullOrWhiteSpace(line.SizeCode) ? line.SizeCode : model.SizeCode;
        //                                    string lineColorType = !string.IsNullOrWhiteSpace(line.ColorType) ? line.ColorType : model.ColorType;
        //                                    lineCmd.Parameters.AddWithValue("@SizeCode", (object)lineSizeCode ?? DBNull.Value);
        //                                    lineCmd.Parameters.AddWithValue("@ColorType", (object)lineColorType ?? DBNull.Value);
        //                                    lineCmd.ExecuteNonQuery();
        //                                }
        //                            }
        //                            srNo++;
        //                        }
        //                    }

        //                    using (SqlCommand recalcCmd = new SqlCommand("USP_TRN_RoughInward_RecalcTotals", con, tran))
        //                    {
        //                        recalcCmd.CommandType = CommandType.StoredProcedure;
        //                        recalcCmd.Parameters.AddWithValue("@InwardID", inwardId);
        //                        recalcCmd.ExecuteNonQuery();
        //                    }

        //                    tran.Commit();
        //                }
        //                catch
        //                {
        //                    tran.Rollback();
        //                    throw;
        //                }
        //            }
        //        }

        //        if (isSuccess && string.IsNullOrEmpty(message))
        //        {
        //            message = isUpdate ? "Rough Inward updated successfully." : "Rough Inward saved successfully.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.ErrorLog(ex);
        //        message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //        isSuccess = false;
        //    }
        //    return Json(new { success = isSuccess, message = message });
        //}

        [HttpPost]
        public JsonResult RoughInward_Save(TRN_RoughInward model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                bool isUpdate = string.Equals(Action, "UPDATE", StringComparison.OrdinalIgnoreCase) && model.InwardID > 0;

                if (string.Equals(model.SourceType, "BRANCH", StringComparison.OrdinalIgnoreCase)
                    && model.FromBranchID.HasValue && model.FromBranchID.Value == model.ToBranchID)
                {
                    return Json(new { success = false, message = "From Branch and To Branch cannot be the same." });
                }

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            int inwardId;
                            string spName = isUpdate ? "USP_TRN_RoughInward_Update" : "USP_TRN_RoughInward_Insert";

                            using (SqlCommand cmd = new SqlCommand(spName, con, tran))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                if (isUpdate)
                                {
                                    cmd.Parameters.AddWithValue("@InwardID", model.InwardID);
                                }

                                DateTime saveChallanDate = model.ChallanDate;
                                if (!string.IsNullOrWhiteSpace(model.ChallanDateStr))
                                {
                                    DateTime parsed;
                                    if (DateTime.TryParse(model.ChallanDateStr, out parsed) && parsed >= new DateTime(1753, 1, 1))
                                    {
                                        saveChallanDate = parsed;
                                    }
                                }
                                if (saveChallanDate < new DateTime(1753, 1, 1))
                                {
                                    saveChallanDate = DateTime.Today;
                                }

                                cmd.Parameters.AddWithValue("@ChallanNo", model.ChallanNo ?? "");
                                cmd.Parameters.AddWithValue("@ChallanDate", saveChallanDate);
                                cmd.Parameters.AddWithValue("@FactoryCodeID", (object)model.FactoryCodeID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@SourceType", model.SourceType ?? "");
                                cmd.Parameters.AddWithValue("@FromBranchID", (object)model.FromBranchID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@FromPartyID", (object)model.FromPartyID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@ToBranchID", model.ToBranchID);
                                cmd.Parameters.AddWithValue("@PurposeID", model.PurposeID);
                                cmd.Parameters.AddWithValue("@Remarks", (object)model.Remarks ?? DBNull.Value);
                                cmd.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId);

                                using (SqlDataReader rdr = cmd.ExecuteReader())
                                {
                                    if (!rdr.Read())
                                    {
                                        throw new Exception("No response from server while saving Rough Inward.");
                                    }
                                    object successVal = SafeGetColumn(rdr, "Success");
                                    isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                                    object msgVal = SafeGetColumn(rdr, "Message");
                                    message = msgVal != null ? Convert.ToString(msgVal) : "";
                                    object idVal = SafeGetColumn(rdr, "InwardID");
                                    inwardId = idVal != null ? Convert.ToInt32(idVal) : model.InwardID;
                                }
                            }

                            if (!isSuccess)
                            {
                                tran.Rollback();
                                return Json(new { success = false, message = message });
                            }

                            if (model.Details != null)
                            {
                                int srNo = 1;
                                foreach (var line in model.Details)
                                {
                                    using (SqlCommand lineCmd = new SqlCommand("USP_TRN_RoughInwardDetail_Save", con, tran))
                                    {
                                        lineCmd.CommandType = CommandType.StoredProcedure;
                                        lineCmd.Parameters.AddWithValue("@InwardDetailID", line.InwardDetailID);
                                        lineCmd.Parameters.AddWithValue("@InwardID", inwardId);
                                        lineCmd.Parameters.AddWithValue("@SrNo", srNo);
                                        lineCmd.Parameters.AddWithValue("@LotNo", line.LotNo ?? "");
                                        lineCmd.Parameters.AddWithValue("@GrowthRate", (object)line.GrowthRate ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@AvgGrowthRate", (object)line.AvgGrowthRate ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@RecipeID", (object)line.RecipeID ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@GradeID", (object)line.GradeID ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@Pcs", line.Pcs);
                                        lineCmd.Parameters.AddWithValue("@Carat", line.Carat);
                                        lineCmd.Parameters.AddWithValue("@Rate", (object)line.Rate ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@Amount", (object)line.Amount ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@Remarks", (object)line.Remarks ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@MachineNo", (object)line.MachineNo ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@Size", (object)line.Size ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@DimensionID", (object)line.DimensionID ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@FromHeight", (object)line.FromHeight ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@ToHeight", (object)line.ToHeight ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@Hours", (object)line.Hours ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@BoxNo", (object)line.BoxNo ?? DBNull.Value);
                                        string lineSizeCode = !string.IsNullOrWhiteSpace(line.SizeCode) ? line.SizeCode : model.SizeCode;
                                        int? lineSizeCodeId = line.SizeCodeId ?? model.SizeCodeID;
                                        string lineColorType = !string.IsNullOrWhiteSpace(line.ColorType) ? line.ColorType : model.ColorType;
                                        lineCmd.Parameters.AddWithValue("@SizeCode", (object)lineSizeCode ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@SizeCodeID", (object)lineSizeCodeId ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@ColorType", (object)lineColorType ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@IsDelete", line.IsDelete);
                                        lineCmd.ExecuteNonQuery();
                                    }
                                    srNo++;
                                }
                            }

                            using (SqlCommand recalcCmd = new SqlCommand("USP_TRN_RoughInward_RecalcTotals", con, tran))
                            {
                                recalcCmd.CommandType = CommandType.StoredProcedure;
                                recalcCmd.Parameters.AddWithValue("@InwardID", inwardId);
                                recalcCmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                if (isSuccess && string.IsNullOrEmpty(message))
                {
                    message = isUpdate ? "Rough Inward updated successfully." : "Rough Inward saved successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message });
        }


        [HttpPost]
        public JsonResult RoughInward_Delete(int? InwardID, TRN_RoughInward model)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int targetInwardId = InwardID.HasValue && InwardID.Value > 0 ? InwardID.Value : (model != null ? model.InwardID : 0);
                if (targetInwardId <= 0)
                {
                    string rawId = Request["InwardID"] ?? Request["inwardID"] ?? Request["id"];
                    int.TryParse(rawId, out targetInwardId);
                }

                if (targetInwardId <= 0)
                {
                    return Json(new { success = false, message = "Invalid Inward ID specified for deletion." }, JsonRequestBehavior.AllowGet);
                }

                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                List<Dictionary<string, object>> deleteRows = DbHelper.ExecuteReaderAsList("USP_TRN_RoughInward_Delete",
                    new SqlParameter("@InwardID", InwardID),
                    new SqlParameter("@ModifiedBy", currentUserId));

               
               
                if (!isSuccess && string.IsNullOrWhiteSpace(message))
                {
                    message = "Cannot delete Rough Inward.";
                }
            }

            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = !string.IsNullOrWhiteSpace(message) ? message : (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }

        
        

        [HttpPost]
        public JsonResult RoughInwardDetail_Delete(int? InwardDetailID, int? InwardID, TRN_RoughInward model)
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int targetDetailId = InwardDetailID.HasValue && InwardDetailID.Value > 0 ? InwardDetailID.Value : 0;
                if (targetDetailId <= 0 && InwardID.HasValue && InwardID.Value > 0)
                {
                    targetDetailId = InwardID.Value;
                }
                if (targetDetailId <= 0 && model != null)
                {
                    targetDetailId = model.InwardID;
                }
                if (targetDetailId <= 0)
                {
                    string rawId = Request["InwardID"] ?? Request["inwardID"] ?? Request["InwardDetailID"] ?? Request["id"] ?? Request["InwardDetailId"];
                    int.TryParse(rawId, out targetDetailId);
                }

                if (targetDetailId <= 0)
                {
                    return Json(new { success = false, message = "Invalid Inward Detail ID specified for deletion." }, JsonRequestBehavior.AllowGet);
                }



                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_TRN_RoughInwardDetail_DeleteByInwardDetail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InwardDetailID", targetDetailId);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            object successVal = SafeGetColumn(rdr, "Success");
                            if (successVal != null)
                            {
                                int sVal = 0;
                                if (int.TryParse(successVal.ToString(), out sVal))
                                {
                                    isSuccess = (sVal == 1);
                                }
                            }

                            object msgVal = SafeGetColumn(rdr, "Message");
                            if (msgVal != null)
                            {
                                message = msgVal.ToString().Trim();
                            }
                        }
                    }
                    con.Close();
                }

                if (!isSuccess && string.IsNullOrWhiteSpace(message))
                {
                    message = "Cannot delete this lot line.";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                isSuccess = false;
            }
            return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Master Book
        public ActionResult MasterBook()
        {
            SetModulePermissions("Master", "MasterBook");
            return View();
        }

        [HttpPost]
        public JsonResult Get_MasterBook_DataList(string FilterType, string FromDate, string ToDate)
        {
            try
            {
                List<object> list = new List<object>();

                DataSet ds = DbHelper.ExecuteDataSet("Get_MasterBook_Data",
                    new SqlParameter("@FilterType", FilterType ?? "All"),
                    new SqlParameter("@FromDate", FromDate ?? ""),
                    new SqlParameter("@ToDate", ToDate ?? ""));

                // Table 0: Data Rows
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            string colName = ds.Tables[0].Columns[j].ColumnName;
                            row[colName] = ds.Tables[0].Rows[i][j] != DBNull.Value ? ds.Tables[0].Rows[i][j] : null;
                        }
                        list.Add(row);
                    }
                }

                // Table 1: Dynamic Column Definitions
                List<object> dataField = new List<object>();
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    dataField = ds.Tables[1].AsEnumerable().Select(row => (object)new
                    {
                        dataField = row.Field<string>("dataField"),
                        caption = row.Field<string>("caption"),
                        dataType = row.Field<string>("dataType"),
                        format = row.Field<string>("format"),
                        alignment = row.Field<string>("alignment"),
                        visible = row.IsNull("visible") ? false : Convert.ToBoolean(row["visible"])
                    }).ToList();
                } 

                // Table 2: Dynamic Summary Footers
                List<object> summary = new List<object>();
                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    summary = ds.Tables[2].AsEnumerable().Select(row => (object)new
                    {
                        column = row.Field<string>("column"),
                        valueFormat = row.Field<string>("valueFormat"),
                        summaryType = row.Field<string>("summaryType"),
                        displayFormat = row.Field<string>("displayFormat")
                    }).ToList();
                }

                var jsonResult = Json(new
                {
                    success = true,
                    List = list,
                    DataField = dataField,
                    DataFieldSummary = summary
                }, JsonRequestBehavior.AllowGet);

                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, List = new List<object>(), DataField = new List<object>(), DataFieldSummary = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // Master Book row-menu "CLV Chart": LotCode is the Kapan key the
        // external report is keyed on (not RCode - multiple Lot splits can
        // share one RCode, but LotCode is unique per Kapan).
        [HttpGet]
        public ActionResult Get_ClvChartData(string LotCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LotCode))
                {
                    return Json(new { success = false, message = "LotCode is required." }, JsonRequestBehavior.AllowGet);
                }

                string rawJson = BranchApiClient.GetAssortmentChartJson(LotCode, "CLV");
                JObject data = JObject.Parse(rawJson);

                // Content(...) + JsonConvert, not the built-in Json() helper -
                // Json() serializes via JavaScriptSerializer, which doesn't
                // understand Newtonsoft JToken/JObject internals and dumps
                // their raw reflection shape instead of clean JSON. Newtonsoft
                // has to be the one serializing this end-to-end.
                string responseJson = JsonConvert.SerializeObject(new { success = true, data = data });
                return Content(responseJson, "application/json");
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public ActionResult QRGenerator()
        {
            return View();
        }

        public ActionResult QRPrint()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateMultipleQRs(string stoneIds)
        {
            try
            {
                if (string.IsNullOrEmpty(stoneIds))
                    return Json(new { success = false, message = "Please enter Stone IDs" });

                var ids = stoneIds.Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => s.Trim())
                                  .Where(s => !string.IsNullOrEmpty(s))
                                  .Distinct()
                                  .ToList();

                List<QRList> qrList = new List<QRList>();

                foreach (var id in ids)
                {
                    string QRPath = Server.MapPath("~/Upload/RQR/" + id + ".png");

                    BarcodeWriter writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Width = 100,
                            Height = 100,
                            Margin = 0
                        }
                    };

                    Bitmap qrCodeBitmap = writer.Write(id);
                    qrCodeBitmap.Save(QRPath, System.Drawing.Imaging.ImageFormat.Png);

                    qrList.Add(new QRList
                    {
                        SubStoneID = id,
                        QRUrl = "/Upload/RQR/" + id + ".png"
                    });
                }

                SessionFacade.QRGenerateList = qrList;

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult RoughDistributionQRPrint()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateRoughDistributionQRs(List<int> distributionIDs)
        {
            try
            {
                if (distributionIDs == null || distributionIDs.Count == 0)
                    return Json(new { success = false, message = "No distribution rows selected." });

                string folder = Server.MapPath("~/Upload/RDQR/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                List<RoughDistributionQRList> qrList = new List<RoughDistributionQRList>();

                foreach (var id in distributionIDs.Distinct())
                {
                    var rows = DbHelper.ExecuteReaderAsList("USP_MST_RoughDistribution_GetQRData",
                        new SqlParameter("@DistributionID", id));

                    if (rows == null || rows.Count == 0) continue;
                    var row = rows[0];

                    string Val(string key) => row.ContainsKey(key) && row[key] != null && row[key] != DBNull.Value ? row[key].ToString() : "";

                    string Text =
                         Val("Receipe") + "," +
                          Val("GrowthRate") + "," +
                       Val("RCode") + "," +
                       Val("LotNo") + "," +
                       Val("RoughPcs") + "," +
                       Val("RoughWeight") + "," +
                       Val("FromName") + "," +
                       Val("Rate") + "," +
                       Val("ChallanNo") + "," +
                       Val("MachineNo") + "," +
                       Val("Hours") + "," +
                       Val("Height") + "," +
                       Val("Size") + "," +
                       Val("SizeCode") + "," +
                       Val("FactoryCode") + "," +
                       Val("Grade") + "," +
                       Val("PurposeName") + "," +
                        Val("ColorType") ;

                    string label =
                         Val("RCode") + "\n" +
                         Val("RoughWeight") + " (" + Val("RoughPcs") + ")" + "\n" +
                         Val("ColorType") + " / " + Val("SizeCode") + " / " + Val("Grade") +  "\n" +
                         Val("Receipe")  + " (" + Val("FactoryCode") + ")";

                    string qrFileName = "RD" + id + ".png";
                    string QRPath = Path.Combine(folder, qrFileName);

                    BarcodeWriter writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Width = 100,
                            Height = 100,
                            Margin = 0
                        }
                    };

                    Bitmap qrCodeBitmap = writer.Write(Text);
                    qrCodeBitmap.Save(QRPath, System.Drawing.Imaging.ImageFormat.Png);

                    qrList.Add(new RoughDistributionQRList
                    {
                        QRUrl = "/Upload/RDQR/" + qrFileName,
                        LabelText = label
                    });
                }

                SessionFacade.RoughDistributionQRList = qrList;

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult LotQRPrint()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateLotQRs(List<int> lotIDs)
        {
            try
            {
                if (lotIDs == null || lotIDs.Count == 0)
                    return Json(new { success = false, message = "No lot rows selected." });

                string folder = Server.MapPath("~/Upload/LotQR/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                List<LotQRList> qrList = new List<LotQRList>();

                foreach (var id in lotIDs.Distinct())
                {
                    var rows = DbHelper.ExecuteReaderAsList("USP_MST_Lot_GetQRData",
                        new SqlParameter("@LotID", id));

                    if (rows == null || rows.Count == 0) continue;
                    var row = rows[0];

                    string Val(string key) => row.ContainsKey(key) && row[key] != null && row[key] != DBNull.Value ? row[key].ToString() : "";

                    string Text =
                        Val("LotCode") + "," +
                        Val("RCode") + "," +
                        Val("LotType") + "," +
                        Val("OGName") + "," +
                        Val("Lot_Pcs") + "," +
                        Val("Lot_Carat") + "," +
                        Val("ValuePerCT") + "," +
                        Val("ColorType") + "," +
                        Val("FactoryCode") + "," +
                        Val("SizeCodeType") + "," +
                        Val("MachineNo") + "," +
                        Val("Hours") + "," +
                        Val("Height") + "," +
                        Val("NewHeight") + "," +
                        Val("Size") + "," +
                        //Val("RejPcs") + "," +
                        //Val("RejCarat") + "," +
                        //Val("CakePcs") + "," +
                        //Val("CakeWeight") + "," +
                        //Val("OGPcs") + "," +
                        //Val("OGCarat") + "," +
                        Val("BoxNo") + "," +
                        Val("RoughRate") + "," +
                        Val("Receipe") + "," +
                        Val("GrowthRate") + "," +
                        Val("LotPurpose") + "," +
                        Val("LotPriority") + "," +
                        Val("CompareNo") + "," +
                        Val("DepartmentShortName") + "," +
                        Val("BranchName") + "," +
                        Val("Remarks");

                    string label =
                        Val("LotCode") + "\n" +
                        Val("Receipe") + "\n" +
                        Val("Lot_Carat") + " Cts (" + Val("Lot_Pcs") + " Pcs)" + "\n" +
                        Val("ColorType") + " / " + Val("SizeCodeType") + " / " + Val("FactoryCode") + "\n" +
                        Val("DepartmentShortName") + " - " + Val("LotPriority");

                    string qrFileName = "LOT" + id + ".png";
                    string QRPath = Path.Combine(folder, qrFileName);

                    // Updating a lot changes its field data, so drop the stale
                    // QR image before writing the regenerated one in its place.
                    if (System.IO.File.Exists(QRPath))
                    {
                        System.IO.File.Delete(QRPath);
                    }

                    BarcodeWriter writer = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Width = 100,
                            Height = 100,
                            Margin = 0
                        }
                    };

                    Bitmap qrCodeBitmap = writer.Write(Text);
                    qrCodeBitmap.Save(QRPath, System.Drawing.Imaging.ImageFormat.Png);

                    qrList.Add(new LotQRList
                    {
                        QRUrl = "/Upload/LotQR/" + qrFileName,
                        LabelText = label
                    });
                }

                SessionFacade.LotQRList = qrList;

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Upload_Lot_Images(int LotID)
        {
            try
            {
                if (LotID <= 0)
                {
                    return Json(new { success = false, message = "LotID is required." });
                }

                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No files selected for upload." });
                }

                string relativeDirectory = $"/Upload/LotImages/{LotID}/";
                string physicalDirectory = Server.MapPath("~" + relativeDirectory);
                if (!Directory.Exists(physicalDirectory))
                {
                    Directory.CreateDirectory(physicalDirectory);
                }

                int uploadBy = SessionFacade.UserSession != null && SessionFacade.UserSession.UserID > 0
                    ? SessionFacade.UserSession.UserID : 1;

                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                List<string> savedPaths = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);
                        if (string.IsNullOrEmpty(ext)) ext = ".jpg";

                        string fileName = $"LOT{LotID}_{timestamp}_{i}{ext}";
                        string fullPhysicalPath = Path.Combine(physicalDirectory, fileName);
                        file.SaveAs(fullPhysicalPath);

                        string webRelativePath = relativeDirectory + fileName;
                        savedPaths.Add(webRelativePath);

                        DbHelper.ExecuteNonQueryWithMessage("USP_MST_LotImage_Insert",
                            new SqlParameter("@LotID", LotID),
                            new SqlParameter("@ImagePath", webRelativePath),
                            new SqlParameter("@UploadBy", uploadBy));
                    }
                }

                return Json(new { success = true, message = "Images uploaded successfully", files = savedPaths });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "Upload failed: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Get_Lot_Images(int LotID)
        {
            try
            {
                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_MST_LotImage_GetList",
                    new SqlParameter("@LotID", LotID));

                List<object> imageList = new List<object>();
                foreach (var row in rows)
                {
                    string rawPath = row["ImagePath"] != null ? row["ImagePath"].ToString() : "";
                    string resolvedUrl = (!string.IsNullOrEmpty(rawPath) && !rawPath.StartsWith("http") && !rawPath.StartsWith("/"))
                        ? "/" + rawPath : rawPath;

                    imageList.Add(new
                    {
                        ID = row["ID"] != null ? Convert.ToInt32(row["ID"]) : 0,
                        LotID = row["LotID"] != null ? Convert.ToInt32(row["LotID"]) : 0,
                        ImagePath = resolvedUrl,
                        Created = row["Created"] != null && row["Created"] != DBNull.Value ? Convert.ToDateTime(row["Created"]).ToString("dd-MMM-yyyy HH:mm") : ""
                    });
                }

                var jsonResult = Json(new { success = true, list = imageList }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = "Error fetching images: " + ex.Message, list = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete_Lot_Image(int imageId)
        {
            try
            {
                string message = DbHelper.ExecuteNonQueryWithMessage("USP_MST_LotImage_Delete",
                    new SqlParameter("@ImageID", imageId));

                bool isSuccess = !string.IsNullOrEmpty(message) && message.ToLower().Contains("success");
                return Json(new { success = isSuccess, message = message });
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // MST RECIPE CRUD AND GET 

        public ActionResult RecipeMaster()
        {
            SetModulePermissions("Master", "RecipeMaster");
            return View();
        }

        public JsonResult Get_MST_ReceipeGridList()
        {
            try
            {
                List<MST_Receipe> list = new List<MST_Receipe>();
                List<Dictionary<string, object>> receipeRows = DbHelper.ExecuteReaderAsList("USP_MST_Receipe_GetList");
                foreach (var row in receipeRows)
                {
                    list.Add(new MST_Receipe
                    {
                        ReceipeID = Convert.ToInt32(row["ReceipeID"]),
                        Receipe = row["Receipe"] != null ? Convert.ToString(row["Receipe"]) : "",
                        Remarks = row.ContainsKey("Remarks") && row["Remarks"] != null ? Convert.ToString(row["Remarks"]) : "",
                        IsActive = row.ContainsKey("IsActive") && row["IsActive"] != null ? Convert.ToBoolean(row["IsActive"]) : true
                    });
                }
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_Receipe>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Receipe_Insert_Update_Delete(MST_Receipe model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                model.UserID = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : (int?)null;

                XmlDocument xmlData = CommonMethods.ConvertToXml(model);
                string xmlStr = "<DocumentElement><MST_Receipe>" + xmlData.DocumentElement.InnerXml + "</MST_Receipe></DocumentElement>";

                message = DbHelper.ExecuteNonQueryWithMessage("USP_MST_Receipe_Insert_Update_Delete",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@ACTION", Action));

                if (message != null && (message.ToLower().Contains("inserted") || message.ToLower().Contains("updated") || message.ToLower().Contains("deleted") || message.ToLower().Contains("success")))
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Json(new { success = isSuccess, message = message });
        }

        // MST COLOR DAYS (MASTERBOOK DAYS) CRUD AND GET 

        public ActionResult ColorDaysMaster()
        {
            SetModulePermissions("Master", "ColorDaysMaster");
            return View();
        }

        public JsonResult Get_MST_MasterbookDaysList()
        {
            try
            {
                List<MST_MasterbookDays> list = new List<MST_MasterbookDays>();
                List<Dictionary<string, object>> rows = DbHelper.ExecuteReaderAsList("USP_MST_MasterbookDays_GetList");
                foreach (var row in rows)
                {
                    list.Add(new MST_MasterbookDays
                    {
                        MasterbookDaysID = Convert.ToInt32(row["MasterbookDaysID"]),
                        StartDay = row.ContainsKey("StartDay") && row["StartDay"] != null ? Convert.ToInt32(row["StartDay"]) : 0,
                        EndDay = row.ContainsKey("EndDay") && row["EndDay"] != null ? Convert.ToInt32(row["EndDay"]) : 0,
                        ColorHex = row.ContainsKey("ColorHex") && row["ColorHex"] != null ? Convert.ToString(row["ColorHex"]) : "",
                        IsActive = row.ContainsKey("IsActive") && row["IsActive"] != null ? Convert.ToBoolean(row["IsActive"]) : true
                    });
                }
                var jsonResult = Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_MasterbookDays>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MasterbookDays_Insert_Update_Delete(MST_MasterbookDays model, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                message = DbHelper.ExecuteNonQueryWithMessage("USP_MST_MasterbookDays_Insert_Update_Delete",
                    new SqlParameter("@MasterbookDaysID", model.MasterbookDaysID),
                    new SqlParameter("@StartDay", model.StartDay),
                    new SqlParameter("@EndDay", model.EndDay),
                    new SqlParameter("@ColorHex", (object)model.ColorHex ?? DBNull.Value),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@UserID", currentUserId),
                    new SqlParameter("@ACTION", Action));

                if (message != null && (message.ToLower().Contains("inserted") || message.ToLower().Contains("updated") || message.ToLower().Contains("deleted") || message.ToLower().Contains("success")))
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Json(new { success = isSuccess, message = message });
        }

        // USER MASTER GET AND CRUD 

        public ActionResult UserMaster()
        {
            SetModulePermissions("Master", "UserMaster");
            return View();
        }

        public JsonResult Get_UserMaster_Data()
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("USP_MST_UserMaster_Getdata");

                List<Dictionary<string, object>> userList = new List<Dictionary<string, object>>();
                List<Dictionary<string, object>> employeeList = new List<Dictionary<string, object>>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            row.Add(ds.Tables[0].Columns[j].ColumnName, ds.Tables[0].Rows[i][j]);
                        }
                        userList.Add(row);
                    }
                }

                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int j = 0; j < ds.Tables[1].Columns.Count; j++)
                        {
                            row.Add(ds.Tables[1].Columns[j].ColumnName, ds.Tables[1].Rows[i][j]);
                        }
                        employeeList.Add(row);
                    }
                }

                var jsonResult = Json(new
                {
                    success = true,
                    UserList = userList,
                    EmployeeList = employeeList
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, UserList = new List<object>(), EmployeeList = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UserMaster_Insert_Update_Delete(int UserID = 0, int? EmployeeID = null, string UserName = "", string Password = "", bool IsAdmin = false, bool IsActive = true, string Action = "INSERT")
        {
            string message = "";
            bool isSuccess = false;
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

                message = DbHelper.ExecuteNonQueryWithMessage("USP_MST_UserMaster_Insert_Update_Delete",
                    new SqlParameter("@ACTION", Action),
                    new SqlParameter("@UserID", UserID),
                    new SqlParameter("@EmployeeID", (object)EmployeeID ?? DBNull.Value),
                    new SqlParameter("@UserName", (object)UserName ?? DBNull.Value),
                    new SqlParameter("@Password", (object)Password ?? DBNull.Value),
                    new SqlParameter("@IsAdmin", IsAdmin),
                    new SqlParameter("@IsActive", IsActive),
                    new SqlParameter("@CreatedByUserID", currentUserId));

                if (message != null && (message.ToLower().Contains("saved") || message.ToLower().Contains("updated") || message.ToLower().Contains("deleted") || message.ToLower().Contains("success")))
                {
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                message = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Json(new { success = isSuccess, message = message });
        }

        

    }
}