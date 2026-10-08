using Newtonsoft.Json;
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
        public JsonResult GetData_For_MST_Lot()
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<object> _list_Slip = new List<object>();


                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("GetData_For_MST_Lot", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                }

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_Slip.Add(Values);
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
               })
               .ToList();

                var DataFieldSummary = _DropDownList.Tables[2].AsEnumerable()
                .Select(row => new DataFields
                {
                    column = row.Field<string>("column"),
                    valueFormat = row.Field<string>("valueFormat"),
                    summaryType = row.Field<string>("summaryType"),
                    displayFormat = row.Field<string>("displayFormat")
                })
                .ToList();

                var jsonResult = Json(new
                {
                    List = _list_Slip,
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
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
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

                        SqlConnection consW = new SqlConnection(conn);
                        SqlCommand cmdsW = new SqlCommand("LotCode_Insert_Using_Excel", consW);
                        cmdsW.CommandType = CommandType.StoredProcedure;
                        cmdsW.Parameters.AddWithValue("@XML", xmlStr);
                        cmdsW.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
                        cmdsW.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 1000);
                        cmdsW.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;
                        consW.Open();

                        int k = cmdsW.ExecuteNonQuery();
                        Message = Convert.ToString(cmdsW.Parameters["@MESSAGE"].Value);

                        consW.Close();
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
        public JsonResult Getdata_For_DropDown()
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<object> _list_Process = new List<object>();
                List<object> _list_Receipe = new List<object>();
                List<object> _list_RCode = new List<object>();
                List<object> _list_LotCode = new List<object>();
                List<object> _list_User = new List<object>();
                List<object> _list_Branch = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Getdata_For_DropDown", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                }

                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list_Process.Add(Values);
                    }
                }
                if (_DropDownList.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[1].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[1].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[1].Columns[j].ToString(), _DropDownList.Tables[1].Rows[i][j].ToString());
                        }
                        _list_Receipe.Add(Values);
                    }
                }
                if (_DropDownList.Tables[2].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[2].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[2].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[2].Columns[j].ToString(), _DropDownList.Tables[2].Rows[i][j].ToString());
                        }
                        _list_RCode.Add(Values);
                    }
                }
                if (_DropDownList.Tables[3].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[3].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[3].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[3].Columns[j].ToString(), _DropDownList.Tables[3].Rows[i][j].ToString());
                        }
                        _list_LotCode.Add(Values);
                    }
                }
                if (_DropDownList.Tables.Count > 4 && _DropDownList.Tables[4].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[4].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[4].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[4].Columns[j].ToString(), _DropDownList.Tables[4].Rows[i][j].ToString());
                        }
                        _list_User.Add(Values);
                    }
                }
                if (_DropDownList.Tables.Count > 5 && _DropDownList.Tables[5].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[5].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[5].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[5].Columns[j].ToString(), _DropDownList.Tables[5].Rows[i][j].ToString());
                        }
                        _list_Branch.Add(Values);
                    }
                }
                var jsonResult = Json(new
                {
                    TableList = _list_Process,
                    ReceipeList = _list_Receipe,
                    RCodeList = _list_RCode,
                    LotCodeList = _list_LotCode,
                    UserList = _list_User,
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
                DataSet _DropDownList = new DataSet();
                List<object> _list__DIA_Rough = new List<object>();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Z_Getdata_R_Rough_Add", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                }

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

                SqlConnection con = new SqlConnection(conn);
                SqlCommand cmd = new SqlCommand("Z_R_Rough_Add_Insert_Update_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@XML", xmlStr);
                cmd.Parameters.AddWithValue("@ACTION", Action);
                cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 1000);
                cmd.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;
                con.Open();

                int j = cmd.ExecuteNonQuery();
                Message = Convert.ToString(cmd.Parameters["@MESSAGE"].Value);

                con.Close();
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

                        using (SqlConnection con = new SqlConnection(conn))
                        {
                            using (SqlCommand cmd = new SqlCommand("Jangad_Images_Insert", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@PreRoughID", (object)PreRoughID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@DistributionID", (object)DistributionID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@JangadNo", safeJangadNo);
                                cmd.Parameters.AddWithValue("@ImagePath", webRelativePath);
                                cmd.Parameters.AddWithValue("@UploadBy", uploadBy);
                                cmd.Parameters.AddWithValue("@Status", normalizedStatus);

                                SqlParameter outMsg = new SqlParameter("@MESSAGE", SqlDbType.VarChar, 500)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                cmd.Parameters.Add(outMsg);

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
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

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_Jangad_Images", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@JangadNo", string.IsNullOrEmpty(JangadNo) ? (object)DBNull.Value : JangadNo);
                        cmd.Parameters.AddWithValue("@PreRoughID", (object)PreRoughID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DistributionID", (object)DistributionID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", (object)normalizedStatus ?? DBNull.Value);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string rawPath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : "";
                                string resolvedUrl = rawPath;
                                if (!string.IsNullOrEmpty(rawPath) && !rawPath.StartsWith("http") && !rawPath.StartsWith("/"))
                                {
                                    resolvedUrl = "/" + rawPath;
                                }

                                imageList.Add(new
                                {
                                    ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    PreRoughID = reader["PreRoughID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["PreRoughID"]) : null,
                                    DistributionID = reader["DistributionID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["DistributionID"]) : null,
                                    JangadNo = reader["JangadNo"] != DBNull.Value ? reader["JangadNo"].ToString() : "",
                                    ImagePath = resolvedUrl,
                                    Created = reader["Created"] != DBNull.Value ? Convert.ToDateTime(reader["Created"]).ToString("dd-MMM-yyyy HH:mm") : "",
                                    UploadBy = reader["UploadBy"] != DBNull.Value ? Convert.ToInt32(reader["UploadBy"]) : 0,
                                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : ""
                                });
                            }
                        }
                        con.Close();
                    }
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

                string message = "";
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Jangad_Images_Insert", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ImageID", imageId);
                        cmd.Parameters.AddWithValue("@Action", "DELETE");
                        cmd.Parameters.AddWithValue("@UploadBy", userId);
                        SqlParameter outMsg = new SqlParameter("@MESSAGE", SqlDbType.VarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outMsg);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        message = Convert.ToString(cmd.Parameters["@MESSAGE"].Value);
                        con.Close();
                    }
                }

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

                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (string actName in possibleActions)
                        {
                            using (SqlCommand cmd = new SqlCommand("Get_UserModulePermission", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
                                cmd.Parameters.AddWithValue("@ControllerName", "Master");
                                cmd.Parameters.AddWithValue("@ActionName", actName);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        bool foundPerm = false;
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            string col = reader.GetName(i);
                                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canView = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canAdd = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canEdit = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canDelete = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canExport = true; foundPerm = true; }
                                            }
                                        }
                                        if (foundPerm) break;
                                    }
                                }
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
        public JsonResult Getdata_TRN_Labour()
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<object> _list__DIA_Rough = new List<object>();
                List<object> DIARoughList = new List<object>();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Getdata_TRN_Labour", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                }
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

                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (string actName in possibleActions)
                        {
                            using (SqlCommand cmd = new SqlCommand("Get_UserModulePermission", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
                                cmd.Parameters.AddWithValue("@ControllerName", "Master");
                                cmd.Parameters.AddWithValue("@ActionName", actName);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        bool foundPerm = false;
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            string col = reader.GetName(i);
                                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canView = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canAdd = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canEdit = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canDelete = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                if (Convert.ToBoolean(reader[i])) { canExport = true; foundPerm = true; }
                                            }
                                        }
                                        if (foundPerm) break;
                                    }
                                }
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
        public JsonResult GetUserFileSizeList()
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<object> _list = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("MST_UserFileSize_Get", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_DropDownList);
                        cmd.Dispose();
                    }
                    con.Close();
                }

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

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("MST_UserFileSize_Insert_Update_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@XML", xmlStr);
                        cmd.Parameters.AddWithValue("@ACTION", Action);
                        cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 1000);
                        cmd.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;
                        con.Open();

                        cmd.ExecuteNonQuery();
                        Message = Convert.ToString(cmd.Parameters["@MESSAGE"].Value);

                        if (Message.ToLower().Contains("successfully"))
                        {
                            isSuccess = true;
                        }

                        con.Close();
                    }
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

                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (string actName in possibleActions)
                        {
                            using (SqlCommand cmd = new SqlCommand("Get_UserModulePermission", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
                                cmd.Parameters.AddWithValue("@ControllerName", "Master");
                                cmd.Parameters.AddWithValue("@ActionName", actName);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        bool foundPerm = false;
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            string col = reader.GetName(i);
                                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canView = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canAdd = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canEdit = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canDelete = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
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
                DataSet ds = new DataSet();
                List<object> list = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_MST_RoughDistributionList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                        cmd.Dispose();
                    }
                    con.Close();
                }

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
        public JsonResult GetPreRoughDetails(int id = 0)
        {
            try
            {
                DataSet ds = new DataSet();
                Dictionary<string, object> masterDetails = new Dictionary<string, object>();
                List<Dictionary<string, object>> childList = new List<Dictionary<string, object>>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_PreRoughDetails_For_Distribution", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                        cmd.Dispose();
                    }
                    con.Close();
                }

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

        private static bool TryParseRCodeSequence(string rCode, out string prefix, out int seqNum)
        {
            prefix = "";
            seqNum = 0;
            if (string.IsNullOrWhiteSpace(rCode)) return false;

            var match = System.Text.RegularExpressions.Regex.Match(rCode.Trim(), @"^(.*?)(\d+)$");
            if (match.Success)
            {
                prefix = match.Groups[1].Value;
                seqNum = int.Parse(match.Groups[2].Value);
                return true;
            }
            return false;
        }

        [HttpPost]
        public JsonResult SaveRoughDistribution(List<MST_RoughDistribution> list, List<int> deletedIDs = null, string Action = "INSERT")
        {
            return RoughDistribution_Insert_Update_Delete(list, Action, deletedIDs);
        }

        [HttpPost]
        public JsonResult RoughDistribution_Insert_Update_Delete(List<MST_RoughDistribution> list, string Action = "INSERT", List<int> deletedIDs = null)
        {
            string Message = "";
            bool isSuccess = false;
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
                                int preRoughId = 0;
                                string rCode = "";

                                using (SqlCommand getCmd = new SqlCommand("SELECT BranchID, PreRoughID, RCode FROM MST_RoughDistribution WITH(NOLOCK) WHERE DistributionID = @DistributionID", con))
                                {
                                    getCmd.Parameters.AddWithValue("@DistributionID", delId);
                                    using (SqlDataReader rdr = getCmd.ExecuteReader())
                                    {
                                        if (rdr.Read())
                                        {
                                            branchId = Convert.ToInt32(rdr["BranchID"] != DBNull.Value ? rdr["BranchID"] : 0);
                                            preRoughId = Convert.ToInt32(rdr["PreRoughID"] != DBNull.Value ? rdr["PreRoughID"] : 0);
                                            rCode = Convert.ToString(rdr["RCode"] != DBNull.Value ? rdr["RCode"] : "");
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(rCode) && TryParseRCodeSequence(rCode, out string prefix, out int currentSeq))
                                {
                                    // Fetch sibling active records for this branch under the same Jangad/PreRough lot
                                    List<string> siblingRCodes = new List<string>();
                                    using (SqlCommand sibCmd = new SqlCommand("SELECT DistributionID, RCode FROM MST_RoughDistribution WITH(NOLOCK) WHERE BranchID = @BranchID AND PreRoughID = @PreRoughID AND IsActive = 1", con))
                                    {
                                        sibCmd.Parameters.AddWithValue("@BranchID", branchId);
                                        sibCmd.Parameters.AddWithValue("@PreRoughID", preRoughId);
                                        using (SqlDataReader sibRdr = sibCmd.ExecuteReader())
                                        {
                                            while (sibRdr.Read())
                                            {
                                                int sibId = Convert.ToInt32(sibRdr["DistributionID"]);
                                                string sibRCode = Convert.ToString(sibRdr["RCode"] ?? "");
                                                if (!deletedIDs.Contains(sibId) && !string.IsNullOrEmpty(sibRCode))
                                                {
                                                    siblingRCodes.Add(sibRCode);
                                                }
                                            }
                                        }
                                    }

                                    // Verify if any remaining sibling record has a higher sequence
                                    string higherRCode = null;
                                    int maxHigherSeq = currentSeq;
                                    foreach (string sCode in siblingRCodes)
                                    {
                                        if (TryParseRCodeSequence(sCode, out string sPrefix, out int sSeq))
                                        {
                                            if (sPrefix.Equals(prefix, StringComparison.OrdinalIgnoreCase) && sSeq > currentSeq)
                                            {
                                                if (sSeq > maxHigherSeq)
                                                {
                                                    maxHigherSeq = sSeq;
                                                    higherRCode = sCode;
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

                            using (SqlConnection con = new SqlConnection(conn))
                            {
                                using (SqlCommand cmd = new SqlCommand("RoughDistribution_Insert_Update_Delete", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@XML", delXmlBuilder.ToString());
                                    cmd.Parameters.AddWithValue("@ACTION", "DELETE");
                                    cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 100);
                                    cmd.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;

                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    Message = Convert.ToString(cmd.Parameters["@MESSAGE"].Value);
                                    con.Close();
                                }
                            }
                        }
                    }
                }

                // 2. Process Insertions / Staged Items (Only process items where DistributionID == 0 or newly added)
                var newItems = list != null ? list.Where(x => x.DistributionID == 0).ToList() : new List<MST_RoughDistribution>();

                if (newItems.Count > 0)
                {
                    // Intra-payload duplicate RCode check
                    var duplicateInPayload = newItems
                        .Where(x => !string.IsNullOrEmpty((x.RCode ?? "").Trim()))
                        .GroupBy(x => x.RCode.Trim(), StringComparer.OrdinalIgnoreCase)
                        .FirstOrDefault(g => g.Count() > 1);

                    if (duplicateInPayload != null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Duplicate RCode '" + duplicateInPayload.Key + "' detected within the entries list! Each distribution must have a unique R.Code."
                        });
                    }

                    // Strict Database Concurrency & Duplicate Check against MST_RoughDistribution
                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (var item in newItems)
                        {
                            string checkRCode = (item.RCode ?? "").Trim();
                            if (!string.IsNullOrEmpty(checkRCode))
                            {
                                using (SqlCommand chkCmd = new SqlCommand("SELECT COUNT(1) FROM MST_RoughDistribution WITH(NOLOCK) WHERE RCode = @RCode", con))
                                {
                                    chkCmd.Parameters.AddWithValue("@RCode", checkRCode);
                                    int count = Convert.ToInt32(chkCmd.ExecuteScalar() ?? 0);
                                    if (count > 0)
                                    {
                                        con.Close();
                                        return Json(new
                                        {
                                            success = false,
                                            message = "Duplicate RCode detected! Another user/tab has already used this code (" + checkRCode + "). Please refresh and try again."
                                        });
                                    }
                                }
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
                        xmlBuilder.Append("<PreRoughID>").Append(item.PreRoughID).Append("</PreRoughID>");
                        xmlBuilder.Append("<BranchID>").Append(item.BranchID).Append("</BranchID>");
                        xmlBuilder.Append("<RCode>").Append(System.Security.SecurityElement.Escape(item.RCode ?? "")).Append("</RCode>");
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

                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        using (SqlCommand cmd = new SqlCommand("RoughDistribution_Insert_Update_Delete", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@XML", xmlBuilder.ToString());
                            cmd.Parameters.AddWithValue("@ACTION", "INSERT");
                            cmd.Parameters.Add("@MESSAGE", SqlDbType.VarChar, 100);
                            cmd.Parameters["@MESSAGE"].Direction = ParameterDirection.Output;

                            con.Open();
                            cmd.ExecuteNonQuery();
                            Message = Convert.ToString(cmd.Parameters["@MESSAGE"].Value);
                            con.Close();
                        }
                    }

                    // Commit sequence counter in MST_BranchVoucherSequence for Automatic numbering branches
                    if (string.IsNullOrEmpty(Message) || Message.ToLower().Contains("inserted") || Message.ToLower().Contains("success") || Message.ToLower().Contains("record is"))
                    {
                        using (SqlConnection con = new SqlConnection(conn))
                        {
                            con.Open();
                            foreach (var item in newItems)
                            {
                                try
                                {
                                    using (SqlCommand chkCmd = new SqlCommand("SELECT NumericMethod FROM MST_VoucherDetails WITH(NOLOCK) WHERE BranchID = @BranchID AND VoucherType = 'Rough Distribution' AND IsActive = 1", con))
                                    {
                                        chkCmd.Parameters.AddWithValue("@BranchID", item.BranchID);
                                        object methodObj = chkCmd.ExecuteScalar();
                                        if (methodObj != null && Convert.ToString(methodObj).Trim().Equals("Automatic", StringComparison.OrdinalIgnoreCase))
                                        {
                                            using (SqlCommand commitCmd = new SqlCommand("Get_RoughCodeConfig", con))
                                            {
                                                commitCmd.CommandType = CommandType.StoredProcedure;
                                                commitCmd.Parameters.AddWithValue("@BranchID", item.BranchID);
                                                commitCmd.Parameters.AddWithValue("@VoucherType", "Rough Distribution");
                                                commitCmd.Parameters.AddWithValue("@ActionType", "COMMIT_AUTO");
                                                commitCmd.Parameters.AddWithValue("@ManualRCode", "");
                                                commitCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                catch (Exception exSeq)
                                {
                                    ErrorLogger.ErrorLog(exSeq);
                                }
                            }
                            con.Close();
                        }
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
            return Json(new { success = isSuccess, message = Message });
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

                    using (SqlConnection con = new SqlConnection(conn))
                    {
                        con.Open();
                        foreach (string actName in possibleActions)
                        {
                            using (SqlCommand cmd = new SqlCommand("Get_UserModulePermission", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@UserID", SessionFacade.UserSession.UserID);
                                cmd.Parameters.AddWithValue("@ControllerName", "Master");
                                cmd.Parameters.AddWithValue("@ActionName", actName);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        bool foundPerm = false;
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            string col = reader.GetName(i);
                                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canView = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canAdd = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canEdit = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Delete", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
                                                if (val) { canDelete = true; foundPerm = true; }
                                            }
                                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && reader[i] != DBNull.Value)
                                            {
                                                bool val = Convert.ToBoolean(reader[i]);
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

        private MST_PartyMaster MapPartyMaster(SqlDataReader rdr)
        {
            return new MST_PartyMaster
            {
                PartyId = rdr["PartyId"] != DBNull.Value ? (Guid)rdr["PartyId"] : Guid.Empty,
                PartyCode = rdr["PartyCode"] != DBNull.Value ? Convert.ToString(rdr["PartyCode"]) : "",
                PartyName = rdr["PartyName"] != DBNull.Value ? Convert.ToString(rdr["PartyName"]) : "",
                ContactPerson = rdr["ContactPerson"] != DBNull.Value ? Convert.ToString(rdr["ContactPerson"]) : "",
                IsOutSide = rdr["IsOutSide"] != DBNull.Value && Convert.ToBoolean(rdr["IsOutSide"]),
                MobileNo = rdr["MobileNo"] != DBNull.Value ? Convert.ToString(rdr["MobileNo"]) : "",
                Email = rdr["Email"] != DBNull.Value ? Convert.ToString(rdr["Email"]) : "",
                Address = rdr["Address"] != DBNull.Value ? Convert.ToString(rdr["Address"]) : "",
                CityName = rdr["CityName"] != DBNull.Value ? Convert.ToString(rdr["CityName"]) : "",
                StateName = rdr["StateName"] != DBNull.Value ? Convert.ToString(rdr["StateName"]) : "",
                CountryName = rdr["CountryName"] != DBNull.Value ? Convert.ToString(rdr["CountryName"]) : "",
                Pincode = rdr["Pincode"] != DBNull.Value ? Convert.ToString(rdr["Pincode"]) : "",
                GSTNo = rdr["GSTNo"] != DBNull.Value ? Convert.ToString(rdr["GSTNo"]) : "",
                PANNo = rdr["PANNo"] != DBNull.Value ? Convert.ToString(rdr["PANNo"]) : "",
                IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                Remarks = rdr["Remarks"] != DBNull.Value ? Convert.ToString(rdr["Remarks"]) : "",
                CreatedOn = rdr["CreatedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["CreatedOn"]) : null,
                CreatedBy = rdr["CreatedBy"] != DBNull.Value ? (Guid?)rdr["CreatedBy"] : null,
                ModifiedOn = rdr["ModifiedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["ModifiedOn"]) : null,
                ModifiedBy = rdr["ModifiedBy"] != DBNull.Value ? (Guid?)rdr["ModifiedBy"] : null
            };
        }

        public JsonResult Get_MST_PartyMasterList(string SearchText)
        {
            try
            {
                List<MST_PartyMaster> list = new List<MST_PartyMaster>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_PartyMaster_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", string.IsNullOrWhiteSpace(SearchText) ? (object)DBNull.Value : SearchText);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(MapPartyMaster(rdr));
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_PartyMaster_GetById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PartyId", PartyId);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            item = MapPartyMaster(rdr);
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string spName = "USP_MST_PartyMaster_Save";
                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (isUpdate)
                        {
                            cmd.Parameters.AddWithValue("@PartyId", model.PartyId);
                        }
                        cmd.Parameters.AddWithValue("@PartyCode", model.PartyCode ?? "");
                        cmd.Parameters.AddWithValue("@PartyName", model.PartyName ?? "");
                        cmd.Parameters.AddWithValue("@ContactPerson", (object)model.ContactPerson ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsOutSide", model.IsOutSide);
                        cmd.Parameters.AddWithValue("@MobileNo", (object)model.MobileNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CityName", (object)model.CityName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StateName", (object)model.StateName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CountryName", (object)model.CountryName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Pincode", (object)model.Pincode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GSTNo", (object)model.GSTNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PANNo", (object)model.PANNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Remarks", (object)model.Remarks ?? DBNull.Value);
                        cmd.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", DBNull.Value);

                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                object successVal = SafeGetColumn(rdr, "Success");
                                isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                                object msgVal = SafeGetColumn(rdr, "Message");
                                message = msgVal != null ? Convert.ToString(msgVal) : "";
                            }
                            else
                            {
                                isSuccess = true;
                            }
                        }
                        con.Close();
                    }
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
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_PartyMaster_Delete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PartyId", PartyId);
                    cmd.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            object successVal = SafeGetColumn(rdr, "Success");
                            isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                            object msgVal = SafeGetColumn(rdr, "Message");
                            message = msgVal != null ? Convert.ToString(msgVal) : "";
                        }
                    }
                    con.Close();
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
                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_PartyMaster_ActiveInactive", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PartyId", PartyId);
                    cmd.Parameters.AddWithValue("@IsActive", IsActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            object successVal = SafeGetColumn(rdr, "Success");
                            isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                            object msgVal = SafeGetColumn(rdr, "Message");
                            message = msgVal != null ? Convert.ToString(msgVal) : "";
                        }
                    }
                    con.Close();
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

        private MST_CompanyBranch MapBranch(SqlDataReader rdr)
        {
            return new MST_CompanyBranch
            {
                BranchID = rdr["BranchID"] != DBNull.Value ? Convert.ToInt32(rdr["BranchID"]) : 0,
                BranchCode = rdr["BranchCode"] != DBNull.Value ? Convert.ToString(rdr["BranchCode"]) : "",
                BranchName = rdr["BranchName"] != DBNull.Value ? Convert.ToString(rdr["BranchName"]) : "",
                BranchTypeID = rdr["CompanyBranchTypeID"] != DBNull.Value ? Convert.ToInt32(rdr["CompanyBranchTypeID"]) : 0,
                BranchTypeName = SafeGetColumn(rdr, "BranchTypeName") != null ? Convert.ToString(SafeGetColumn(rdr, "BranchTypeName")) : "",
                GSTIN = rdr["GSTIN"] != DBNull.Value ? Convert.ToString(rdr["GSTIN"]) : "",
                StateCode = rdr["StateCode"] != DBNull.Value ? Convert.ToString(rdr["StateCode"]) : "",
                Address = rdr["Address"] != DBNull.Value ? Convert.ToString(rdr["Address"]) : "",
                City = rdr["City"] != DBNull.Value ? Convert.ToString(rdr["City"]) : "",
                ContactPerson = rdr["ContactPerson"] != DBNull.Value ? Convert.ToString(rdr["ContactPerson"]) : "",
                Phone = rdr["Phone"] != DBNull.Value ? Convert.ToString(rdr["Phone"]) : "",
                IsActive = rdr["IsActive"] != DBNull.Value && Convert.ToBoolean(rdr["IsActive"]),
                CreatedBy = rdr["CreatedBy"] != DBNull.Value ? Convert.ToInt32(rdr["CreatedBy"]) : 0,
                CreatedOn = rdr["CreatedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["CreatedOn"]) : null,
                ModifiedBy = rdr["ModifiedBy"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["ModifiedBy"]) : null,
                ModifiedOn = rdr["ModifiedOn"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["ModifiedOn"]) : null
            };
        }

        public JsonResult Get_MST_BranchTypeList()
        {
            try
            {
                List<MST_BranchType> list = new List<MST_BranchType>();

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_CompanyBranchType_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new MST_BranchType
                            {
                                BranchTypeID = Convert.ToInt32(rdr["ID"]),
                                BranchTypeName = rdr["BranchTypeName"] != DBNull.Value ? Convert.ToString(rdr["BranchTypeName"]) : ""
                            });
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_CompanyBranch_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", string.IsNullOrWhiteSpace(SearchText) ? (object)DBNull.Value : SearchText);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(MapBranch(rdr));
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_CompanyBranch_GetById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BranchID", BranchID);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            item = MapBranch(rdr);
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                {
                    string spName = isUpdate ? "USP_MST_CompanyBranch_Update" : "USP_MST_CompanyBranch_Insert";
                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (isUpdate)
                        {
                            cmd.Parameters.AddWithValue("@BranchID", model.BranchID);
                        }
                        cmd.Parameters.AddWithValue("@BranchCode", model.BranchCode ?? "");
                        cmd.Parameters.AddWithValue("@BranchName", model.BranchName ?? "");
                        cmd.Parameters.AddWithValue("@CompanyBranchTypeID", model.BranchTypeID);
                        cmd.Parameters.AddWithValue("@GSTIN", (object)model.GSTIN ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StateCode", (object)model.StateCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)model.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@City", (object)model.City ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContactPerson", (object)model.ContactPerson ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Phone", (object)model.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId);

                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                object successVal = SafeGetColumn(rdr, "Success");
                                isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                                object msgVal = SafeGetColumn(rdr, "Message");
                                message = msgVal != null ? Convert.ToString(msgVal) : "";
                            }
                        }
                        con.Close();
                    }
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_CompanyBranch_Delete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    cmd.Parameters.AddWithValue("@ModifiedBy", currentUserId);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            object successVal = SafeGetColumn(rdr, "Success");
                            isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                            object msgVal = SafeGetColumn(rdr, "Message");
                            message = msgVal != null ? Convert.ToString(msgVal) : "";
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_CompanyBranch_ActiveInactive", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BranchID", BranchID);
                    cmd.Parameters.AddWithValue("@IsActive", IsActive);
                    cmd.Parameters.AddWithValue("@ModifiedBy", currentUserId);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            object successVal = SafeGetColumn(rdr, "Success");
                            isSuccess = successVal != null && Convert.ToInt32(successVal) == 1;
                            object msgVal = SafeGetColumn(rdr, "Message");
                            message = msgVal != null ? Convert.ToString(msgVal) : "";
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_Purpose_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new MST_Purpose
                            {
                                PurposeID = Convert.ToInt32(rdr["PurposeID"]),
                                PurposeName = rdr["PurposeName"] != DBNull.Value ? Convert.ToString(rdr["PurposeName"]) : ""
                            });
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_Receipe_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new MST_Receipe
                            {
                                ReceipeID = Convert.ToInt32(rdr["ReceipeID"]),
                                Receipe = rdr["Receipe"] != DBNull.Value ? Convert.ToString(rdr["Receipe"]) : ""
                            });
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_Grade_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new MST_Grade
                            {
                                GradeID = Convert.ToInt32(rdr["ID"]),
                                Grade = rdr["Grade"] != DBNull.Value ? Convert.ToString(rdr["Grade"]) : ""
                            });
                        }
                    }
                    con.Close();
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_MST_FactoryCode_GetList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            list.Add(new MST_FactoryCode
                            {
                                FactoryCodeID = Convert.ToInt32(rdr["ID"]),
                                FactoryCode = rdr["FactoryCode"] != DBNull.Value ? Convert.ToString(rdr["FactoryCode"]) : ""
                            });
                        }
                    }
                    con.Close();
                }

                return Json(new { success = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { success = false, message = ex.Message, list = new List<MST_FactoryCode>() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Get_UserBranch()
        {
            try
            {
                int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;
                int? branchId = null;
                string branchName = null;

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_SEC_User_GetBranch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            branchId = rdr["BranchID"] != DBNull.Value ? (int?)Convert.ToInt32(rdr["BranchID"]) : null;
                            branchName = rdr["BranchName"] != DBNull.Value ? Convert.ToString(rdr["BranchName"]) : null;
                        }
                    }
                    con.Close();
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
                                        FromHeight = SafeGetColumn(rdr, "FromHeight") != null ? (decimal?)Convert.ToDecimal(SafeGetColumn(rdr, "FromHeight")) : null,
                                        ToHeight = SafeGetColumn(rdr, "ToHeight") != null ? (decimal?)Convert.ToDecimal(SafeGetColumn(rdr, "ToHeight")) : null,
                                        Hours = SafeGetColumn(rdr, "Hours") != null ? (decimal?)Convert.ToDecimal(SafeGetColumn(rdr, "Hours")) : null,
                                        BoxNo = SafeGetColumn(rdr, "BoxNo") != null ? Convert.ToString(SafeGetColumn(rdr, "BoxNo")) : "",
                                        SizeCode = SafeGetColumn(rdr, "SizeCode") != null ? Convert.ToString(SafeGetColumn(rdr, "SizeCode")) : "",
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
                                        lineCmd.Parameters.AddWithValue("@FromHeight", (object)line.FromHeight ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@ToHeight", (object)line.ToHeight ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@Hours", (object)line.Hours ?? DBNull.Value);
                                        lineCmd.Parameters.AddWithValue("@BoxNo", (object)line.BoxNo ?? DBNull.Value);
                                        string lineSizeCode = !string.IsNullOrWhiteSpace(line.SizeCode) ? line.SizeCode : model.SizeCode;
                                        string lineColorType = !string.IsNullOrWhiteSpace(line.ColorType) ? line.ColorType : model.ColorType;
                                        lineCmd.Parameters.AddWithValue("@SizeCode", (object)lineSizeCode ?? DBNull.Value);
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

                using (SqlConnection con = new SqlConnection(conn))
                using (SqlCommand cmd = new SqlCommand("USP_TRN_RoughInward_Delete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InwardID", targetInwardId);
                    cmd.Parameters.AddWithValue("@ModifiedBy", currentUserId);

                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        do
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
                                    else if (bool.TryParse(successVal.ToString(), out bool bVal))
                                    {
                                        isSuccess = bVal;
                                    }
                                }

                                object msgVal = SafeGetColumn(rdr, "Message");
                                if (msgVal != null && !string.IsNullOrWhiteSpace(msgVal.ToString()))
                                {
                                    message = msgVal.ToString().Trim();
                                }
                            }
                        } while (rdr.NextResult());
                    }
                    con.Close();
                }

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


        //[HttpPost]
        //public JsonResult RoughInwardDetail_Delete(int? InwardDetailID, TRN_RoughInward model)
        //{
        //    string message = "";
        //    bool isSuccess = false;
        //    try
        //    {
        //        int targetInwardId = InwardDetailID.HasValue && InwardDetailID.Value > 0 ? InwardDetailID.Value : (model != null ? model.InwardID : 0);
        //        if (targetInwardId <= 0)
        //        {
        //            string rawId = Request["InwardID"] ?? Request["inwardID"] ?? Request["id"];
        //            int.TryParse(rawId, out targetInwardId);
        //        }

        //        if (targetInwardId <= 0)
        //        {
        //            return Json(new { success = false, message = "Invalid Inward ID specified for deletion." }, JsonRequestBehavior.AllowGet);
        //        }

        //        int currentUserId = SessionFacade.UserSession != null ? SessionFacade.UserSession.UserID : 0;

        //        using (SqlConnection con = new SqlConnection(conn))
        //        using (SqlCommand cmd = new SqlCommand("USP_TRN_RoughInwardDetail_DeleteByInwardDetail", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@InwardDetailID", targetInwardId);

        //            con.Open();
        //            using (SqlDataReader rdr = cmd.ExecuteReader())
        //            {
        //                do
        //                {
        //                    while (rdr.Read())
        //                    {
        //                        object successVal = SafeGetColumn(rdr, "Success");
        //                        if (successVal != null)
        //                        {
        //                            int sVal = 0;
        //                            if (int.TryParse(successVal.ToString(), out sVal))
        //                            {
        //                                isSuccess = (sVal == 1);
        //                            }
        //                            else if (bool.TryParse(successVal.ToString(), out bool bVal))
        //                            {
        //                                isSuccess = bVal;
        //                            }
        //                        }

        //                        object msgVal = SafeGetColumn(rdr, "Message");
        //                        if (msgVal != null && !string.IsNullOrWhiteSpace(msgVal.ToString()))
        //                        {
        //                            message = msgVal.ToString().Trim();
        //                        }
        //                    }
        //                } while (rdr.NextResult());
        //            }
        //            con.Close();
        //        }

        //        if (!isSuccess && string.IsNullOrWhiteSpace(message))
        //        {
        //            message = "Cannot delete Rough Inward.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.ErrorLog(ex);
        //        message = !string.IsNullOrWhiteSpace(message) ? message : (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
        //        isSuccess = false;
        //    }
        //    return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        //}


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
                DataSet ds = new DataSet();
                List<object> list = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_MasterBook_Data", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FilterType", FilterType ?? "Rough Entry");
                        cmd.Parameters.AddWithValue("@FromDate", FromDate ?? "");
                        cmd.Parameters.AddWithValue("@ToDate", ToDate ?? "");
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(ds);
                        cmd.Dispose();
                    }
                    con.Close();
                }

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
                        visible = row.Field<bool>("visible")
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
    }
}