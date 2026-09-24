using Slip.Models;
using Slip.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using static Slip.Models.Filter.SessionExpireFilter;

namespace Slip.Controllers
{
    public class EmployeesController : BaseController
    {
        // GET: Employees
        public ActionResult Index(int id)
        {
            ViewBag.EmployeeID = id;
            return View();
        }
        public ActionResult EmployeeDetail()
        {
            return View();
        }
        #region :: Register ::
        public JsonResult CustomerImageUpload()
        {
            var file1 = "";

            try
            {
                var file = Request.Files[0];
                if (Request.Files.Count > 0)
                {
                    //string FolderPath = "~\\Images\\PhotoPath";
                    string FolderPath = "~/Images/PhotoPath";
                    int _custId = 0;
                    _custId = int.Parse(Request.Form.Get("party_code"));
                    if (_custId == 0)
                    {
                        DataSet dataset = new DataSet();
                        using (SqlConnection con = new SqlConnection(conn))
                        {
                            using (SqlCommand cmd = new SqlCommand("EMP_Employee_MaxIDDefine", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                con.Open();
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dataset);
                                cmd.Dispose();
                            }
                            con.Close();
                        }

                        var EmployeeID = dataset.Tables[0].Rows[0][0].ToString();
                        _custId = Convert.ToInt32(EmployeeID) + 1;
                    }
                    string Filename = _custId + "_ProfileImage";
                    var extension = file.FileName.Split('.')[1];
                    if (!Directory.Exists(Server.MapPath(FolderPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(FolderPath));
                    }
                    string CustomerFilePath = Server.MapPath(FolderPath + "\\" + Filename + "." + extension);

                    if (System.IO.File.Exists(CustomerFilePath))
                    {
                        System.IO.File.Delete(CustomerFilePath);
                    }
                    file.SaveAs(CustomerFilePath);
                    file1 = Filename + "." + extension;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }
            return Json(file1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CustomerProofImageUpload()
        {
            var file1 = "";

            try
            {
                var file = Request.Files[0];
                // string[] Extension = { "jpeg", "jpg", "png" };
                if (Request.Files.Count > 0)
                {
                    //string FolderPath = "~\\Images\\ProofPhotoPath";
                    string FolderPath = "~/Images/ProofPhotoPath";
                    int _custId = 0;
                    _custId = int.Parse(Request.Form.Get("Emp_ID"));
                    if (_custId == 0)
                    {
                        DataSet dataset = new DataSet();
                        using (SqlConnection con = new SqlConnection(conn))
                        {
                            using (SqlCommand cmd = new SqlCommand("EMP_Employee_MaxIDDefine", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                con.Open();
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dataset);
                                cmd.Dispose();
                            }
                            con.Close();
                        }

                        var EmployeeID = dataset.Tables[0].Rows[0][0].ToString();
                        _custId = Convert.ToInt32(EmployeeID) + 1;
                    }
                    string Filename = _custId + "_ProofImage";
                    var extension = file.FileName.Split('.')[1];
                    if (!Directory.Exists(Server.MapPath(FolderPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(FolderPath));
                    }
                    string CustomerFilePath = Server.MapPath(FolderPath + "\\" + Filename + "." + extension);


                    if (System.IO.File.Exists(CustomerFilePath))
                    {
                        System.IO.File.Delete(CustomerFilePath);
                    }
                    file.SaveAs(CustomerFilePath);
                    file1 = Filename + "." + extension;

                }
            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorLogger.ErrorLog(ex);
            }
            return Json(file1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CustomerPath2ImageUpload()
        {
            var file1 = "";

            try
            {
                var file = Request.Files[0];
                // string[] Extension = { "jpeg", "jpg", "png" };
                if (Request.Files.Count > 0)
                {
                    //string FolderPath = "~\\Images\\ProofPhotoPath";
                    string FolderPath = "~/Images/PhotoPath2";
                    int _custId = 0;
                    _custId = int.Parse(Request.Form.Get("Emp_ID"));
                    if (_custId == 0)
                    {
                        DataSet dataset = new DataSet();
                        using (SqlConnection con = new SqlConnection(conn))
                        {
                            using (SqlCommand cmd = new SqlCommand("EMP_Employee_MaxIDDefine", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                con.Open();
                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.Fill(dataset);
                                cmd.Dispose();
                            }
                            con.Close();
                        }

                        var EmployeeID = dataset.Tables[0].Rows[0][0].ToString();
                        _custId = Convert.ToInt32(EmployeeID) + 1;
                    }
                    string Filename = _custId + "_Path2Image";
                    var extension = file.FileName.Split('.')[1];
                    if (!Directory.Exists(Server.MapPath(FolderPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(FolderPath));
                    }
                    string CustomerFilePath = Server.MapPath(FolderPath + "\\" + Filename + "." + extension);


                    if (System.IO.File.Exists(CustomerFilePath))
                    {
                        System.IO.File.Delete(CustomerFilePath);
                    }
                    file.SaveAs(CustomerFilePath);
                    file1 = Filename + "." + extension;

                }
            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorLogger.ErrorLog(ex);
            }
            return Json(file1, JsonRequestBehavior.AllowGet);
        }
        public string InsertUpdateEmployee(EMP_Employees model, string Action)
        {

            string Message = "";
            try
            {

                model.Age = model.Age == null ? 1 : model.Age;

                model.IsActive = model.IsActive == null ? false : model.IsActive;
                model.UserID = SessionFacade.UserSession.UserID;
                XmlDocument Xmldata = CommonMethods.ConvertToXml(model);
                string xmlStr = "<DocumentElement><EMP_Employees>" + Xmldata.DocumentElement.InnerXml + "</EMP_Employees></DocumentElement>";
                //   ErrorLogger.ErrorLogStr("XML Create" + xmlStr);

                SqlConnection con = new SqlConnection(conn);
                SqlCommand cmd = new SqlCommand("EMP_Employee_Insert_Update_Delete", con);
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
                return "ERROR : " + ex.InnerException.Message;
            }
            return Message;
        }
        public JsonResult EMP_Employee_Getdata()
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<object> _list__EMP_Employee = new List<object>();


                List<object> EMPEmployeeList = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("EMP_Employee_Getdata", con))
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
                        _list__EMP_Employee.Add(Values);
                    }
                }

                var DataField = _DropDownList.Tables[1].AsEnumerable()
               .Select(row => new DataFields
               {
                   dataField = row.Field<string>("dataField"),
                   caption = row.Table.Columns.Contains("caption") && !row.IsNull("caption") ? row.Field<string>("caption") : null,
                   dataType = row.Field<string>("dataType"),
                   format = row.Field<string>("format"),
                   alignment = row.Field<string>("alignment"),
                   visible = row.Field<bool>("visible"),
                   @fixed = row.Field<bool>("fixed"),
               })
               .ToList();

                var jsonResult = Json(new
                {
                    EMPEmployeeList = _list__EMP_Employee,
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
        public JsonResult EMP_Employee_GetdataEdit(int EmployeeID)
        {
            List<object> _Party_List = new List<object>();
            try
            {
                DataSet _PartyEditData = new DataSet();
                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("EMP_Employee_GetdataEdit", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(_PartyEditData);
                        cmd.Dispose();
                    }
                    con.Close();
                }
                for (int i = 0; i < _PartyEditData.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _PartyEditData.Tables[0].Columns.Count; j++)
                    {
                        Values.Add(_PartyEditData.Tables[0].Columns[j].ToString(), _PartyEditData.Tables[0].Rows[i][j].ToString());
                    }
                    _Party_List.Add(Values);
                }

                var jsonResult = Json(new
                {
                    List = _Party_List

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
                return jsonResult;
            }
        }
        #endregion
        public JsonResult Get_DataList_For_Using_Common()
        {
            try
            {
                DataSet _DropDownList = new DataSet();

                List<object> _list = new List<object>();
                List<object> _list_Role = new List<object>();
                List<object> _list_Process = new List<object>();

                using (SqlConnection con = new SqlConnection(conn))
                {
                    using (SqlCommand cmd = new SqlCommand("Get_DataList_For_Using_Common", con))
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
                        _list.Add(Values);
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
                        _list_Role.Add(Values);
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
                        _list_Process.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    TableList = _list,
                    RoleList = _list_Role,
                    ProcessList = _list_Process,
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
    }
}