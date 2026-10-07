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
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static Slip.Models.Filter.SessionExpireFilter;


namespace Slip.Controllers
{
    public class PricingController : BaseController
    {
        // GET: Pricing
        public ActionResult Pricing_Old()
        {
            return View();
        }
        public ActionResult Pricing()
        {
            SetModulePermissions("Pricing", "Pricing");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "Pricing"),
                        new SqlParameter("@ActionName", "Pricing"));

                    if (permissionRows.Count > 0)
                    {
                        var row = permissionRows[0];
                        foreach (var kvp in row)
                        {
                            if (kvp.Key.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }
        public ActionResult PricingDetails()
        {
            SetModulePermissions("Pricing", "PricingDetails");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "Pricing"),
                        new SqlParameter("@ActionName", "PricingDetails"));

                    if (permissionRows.Count > 0)
                    {
                        var row = permissionRows[0];
                        foreach (var kvp in row)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canAdd = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }
        public JsonResult Get_DataList_DIAMaster()
        {
            try
            {
                DataSet _DropDownList = new DataSet();
                List<object> _list__DIA_Size = new List<object>();
                List<object> _list__DIA_Shape = new List<object>();
                List<object> _list__DIA_Clarity = new List<object>();
                List<object> _list__DIA_Color = new List<object>();
                List<object> _list__DIA_Cut = new List<object>();
                List<object> _list__DIA_ShapeType = new List<object>();
                List<object> _list__MST_ClarityWithColor = new List<object>();

                List<object> DIASizeList = new List<object>();
                List<object> DIAShapeList = new List<object>();
                List<object> DIAClarityList = new List<object>();
                List<object> DIAColorList = new List<object>();
                List<object> DIACutList = new List<object>();
                List<object> DIAShapeTypeList = new List<object>();
                List<object> MSTLabourList = new List<object>();
                List<object> MSTClarityWithColorList = new List<object>();
                _DropDownList = DbHelper.ExecuteDataSet("Get_DataList_DIAMaster");
                if (_DropDownList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[0].Columns[j].ToString(), _DropDownList.Tables[0].Rows[i][j].ToString());
                        }
                        _list__DIA_Size.Add(Values);
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
                        _list__DIA_Shape.Add(Values);
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
                        _list__DIA_Clarity.Add(Values);
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
                        _list__DIA_Color.Add(Values);
                    }
                }
                if (_DropDownList.Tables[4].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[4].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[4].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[4].Columns[j].ToString(), _DropDownList.Tables[4].Rows[i][j].ToString());
                        }
                        _list__DIA_Cut.Add(Values);
                    }
                }
                if (_DropDownList.Tables[5].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[5].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[5].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[5].Columns[j].ToString(), _DropDownList.Tables[5].Rows[i][j].ToString());
                        }
                        _list__DIA_ShapeType.Add(Values);
                    }
                }
                if (_DropDownList.Tables[6].Rows.Count > 0)
                {
                    for (int i = 0; i < _DropDownList.Tables[6].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DropDownList.Tables[6].Columns.Count; j++)
                        {
                            Values.Add(_DropDownList.Tables[6].Columns[j].ToString(), _DropDownList.Tables[6].Rows[i][j].ToString());
                        }
                        _list__MST_ClarityWithColor.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    DIASizeList = _list__DIA_Size,
                    DIAShapeList = _list__DIA_Shape,
                    DIAClarityList = _list__DIA_Clarity,
                    DIAColorList = _list__DIA_Color,
                    DIACutList = _list__DIA_Cut,
                    DIAShapeTypeList = _list__DIA_ShapeType,
                    MSTClarityWithColorList = _list__MST_ClarityWithColor
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
        public JsonResult Getdata_TRN_Pricing(string Date, string SizeType, List<TRN_Pricing_Filter> SizeList, string Shape, string GradType, string Action)
        {
            List<object> _TRN_Rap = new List<object>();
            try
            {
                string xmlStr = "";
                if (SizeList != null)
                {
                    XmlDocument Xmldata = CommonMethods.ConvertToXml(SizeList);
                    xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                }

                DataSet _DiamondEditData = DbHelper.ExecuteDataSet("Getdata_TRN_Pricing",
                    new SqlParameter("@SizeXML", SqlDbType.Xml) { Value = xmlStr },
                    new SqlParameter("@Date", SqlDbType.VarChar) { Value = Date },
                    new SqlParameter("@SizeType", SqlDbType.VarChar) { Value = SizeType },
                    new SqlParameter("@Shape", SqlDbType.VarChar) { Value = Shape },
                    new SqlParameter("@GradType", SqlDbType.VarChar) { Value = GradType },
                    new SqlParameter("@Action", SqlDbType.VarChar) { Value = Action });

                for (int i = 0; i < _DiamondEditData.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _DiamondEditData.Tables[0].Columns.Count; j++)
                    {
                        Values.Add(_DiamondEditData.Tables[0].Columns[j].ToString(), _DiamondEditData.Tables[0].Rows[i][j].ToString());
                    }
                    _TRN_Rap.Add(Values);
                }

                var jsonResult = Json(new
                {
                    List = _TRN_Rap

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

        public JsonResult GetData_TRN_Pricing_History(string Shape, string GradType, string SizeID, string ColorID, string ClarityID)
        {
            List<object> _History = new List<object>();
            try
            {
                DataSet _DataSet = DbHelper.ExecuteDataSet("GetData_TRN_Pricing_History",
                    new SqlParameter("@Shape", SqlDbType.VarChar) { Value = Shape ?? "" },
                    new SqlParameter("@GradType", SqlDbType.VarChar) { Value = GradType ?? "" },
                    new SqlParameter("@SizeID", SqlDbType.Int) { Value = Convert.ToInt32(SizeID) },
                    new SqlParameter("@ColorID", SqlDbType.Int) { Value = Convert.ToInt32(ColorID) },
                    new SqlParameter("@ClarityID", SqlDbType.Int) { Value = Convert.ToInt32(ClarityID) });

                if (_DataSet.Tables.Count > 0 && _DataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < _DataSet.Tables[0].Rows.Count; i++)
                    {
                        Dictionary<string, string> Values = new Dictionary<string, string>();
                        for (int j = 0; j < _DataSet.Tables[0].Columns.Count; j++)
                        {
                            Values.Add(_DataSet.Tables[0].Columns[j].ToString(), _DataSet.Tables[0].Rows[i][j].ToString());
                        }
                        _History.Add(Values);
                    }
                }

                var jsonResult = Json(new
                {
                    List = _History
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { List = new List<object>() }, JsonRequestBehavior.AllowGet);
            }
        }
        public string TRN_Pricing_Insert_Update_Delete(List<TRN_Pricing> PricingDetails)
        {

            string Message = "";
            try
            {
                XmlDocument Xmldata = CommonMethods.ConvertToXml(PricingDetails);
                string xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";

                Message = DbHelper.ExecuteNonQueryWithMessage("TRN_Pricing_Insert_Update_Delete",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));

            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return "ERROR : " + ex.InnerException.Message;
            }
            return Message;
        }


        public ActionResult Create_XML()
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("GetData_For_XML", 300);

                string xml = CreateXMLFromDataSet(ds);
                return Content(xml, "application/xml");

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

        public string CreateXMLFromDataSet(DataSet ds)
        {
            DataTable dtProperties = ds.Tables[0];
            DataTable dtProperty = ds.Tables[1];
            DataTable dtItem = ds.Tables[2];
            DataTable dtShape = ds.Tables[3];
            DataTable dtInstitute = ds.Tables[4];
            DataTable dtGrade = ds.Tables[5];
            DataTable dtRange = ds.Tables[6];
            DataTable dtPriceUnits = ds.Tables[7];

            XDocument xml = new XDocument(
                new XElement("Sarin",
                    new XElement("PropertiesSets",
                         new XAttribute("NumOfPropertiesSets", dtProperties.Rows.Count),

                        dtProperties.AsEnumerable().Select(Pr =>
                            new XElement("Properties",
                                new XAttribute("ID", Pr["MPRID"]),
                                new XAttribute("Name", Pr["MPRName"]),
                                new XAttribute("NumOfProperties",
                                    dtProperty.AsEnumerable()
                                    .Count(g => g.Field<int>("MPRID") == Pr.Field<int>("MPRID"))),

                                dtProperty.AsEnumerable()
                                .Where(g => g.Field<int>("MPRID") == Pr.Field<int>("MPRID"))
                                .Select(Property =>
                                    new XElement("Property",
                                        new XAttribute("ID", Property["SPRID"]),
                                        new XAttribute("Name", Property["SPRName"]),
                                        new XAttribute("NumOfItems",
                                                dtItem.AsEnumerable()
                                                .Count(i => i.Field<int>("MPRID") == Pr.Field<int>("MPRID") && i.Field<int>("SPRID") == Property.Field<int>("SPRID"))),

                                        dtItem.AsEnumerable()
                                        .Where(g => g.Field<int>("MPRID") == Pr.Field<int>("MPRID") && g.Field<int>("SPRID") == Property.Field<int>("SPRID"))
                                        .Select(Item =>
                                            new XElement("Item",
                                                new XAttribute("ID", Item["ItemID"]),
                                                new XAttribute("Name", Item["ItemName"])
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),

                    new XElement("PriceLists",
                        new XAttribute("NumOfPriceLists", "1"),

                        new XElement("PriceList",
                            new XAttribute("DiscountType", "1"),
                            new XAttribute("ID", "9009"),
                            new XAttribute("Name", "ADPL"),
                            new XAttribute("StoneAttributesCollectionID", "2"),

                            new XElement("Shapes",
                                new XAttribute("NumOfShapes", dtShape.Rows.Count),

                                dtShape.AsEnumerable().Select(shape =>
                                    new XElement("Shape",
                                        new XAttribute("ID", shape["ShapeIDUserFile"]),
                                        new XAttribute("Name", shape["ShapeName"]),

                                        new XElement("Institutes",
                                            new XAttribute("NumOfInstitutes",
                                                dtInstitute.AsEnumerable()
                                                .Count(i => i.Field<int>("ShapeID") == shape.Field<int>("ShapeIDUserFile"))),

                                            dtInstitute.AsEnumerable()
                                            .Where(i => i.Field<int>("ShapeID") == shape.Field<int>("ShapeIDUserFile"))
                                            .Select(inst =>
                                                new XElement("Institute",
                                                    new XAttribute("ID", inst["InstituteID"]),
                                                    new XAttribute("Name", inst["InstituteName"]),

                                                    new XElement("Grades",
                                                        new XAttribute("NumOfGrades",
                                                            dtGrade.AsEnumerable()
                                                            .Count(g =>
                                                                g.Field<int>("ShapeID") == shape.Field<int>("ShapeIDUserFile") &&
                                                                g.Field<int>("InstituteID") == inst.Field<int>("InstituteID"))),

                                                        dtGrade.AsEnumerable()
                                                        .Where(g =>
                                                            g.Field<int>("ShapeID") == shape.Field<int>("ShapeIDUserFile") &&
                                                            g.Field<int>("InstituteID") == inst.Field<int>("InstituteID"))
                                                        .Select(grd =>
                                                            new XElement("Grade",
                                                                new XAttribute("ID", grd["GradeID"]),
                                                                new XAttribute("Name", grd["GradeName"]),

                                                                new XElement("Ranges",
                                                                    new XAttribute("NumOfRanges",
                                                                        dtRange.AsEnumerable()
                                                                        .Count(g =>
                                                                            g.Field<string>("Shape") == shape.Field<string>("ShapeGroup"))),
                                                                            new XAttribute("Type", grd["Ranges"]),

                                                                    dtRange.AsEnumerable()
                                                                    .Where(g => g.Field<string>("Shape") == shape.Field<string>("ShapeGroup") && g.Field<string>("Cut") == grd.Field<string>("Cut"))
                                                                    .Select(Rng =>
                                                                        new XElement("Range",
                                                                            new XAttribute("ID", Rng["SizeIDUserFile"]),
                                                                            new XAttribute("LowerBound", Rng["FromSize"]),
                                                                            new XAttribute("UpperBound", Rng["ToSize"]),

                                                                            new XElement("PriceTable",
                                                                                new XElement("GroupedColors",
                                                                                    new XAttribute("NumOfGroups", "0")
                                                                                ),
                                                                                new XElement("GroupedClarities",
                                                                                    new XAttribute("NumOfGroups", "0")
                                                                                ),
                                                                                new XElement("CustomDiscountGroups",
                                                                                    new XAttribute("NumOfGroups", "0")
                                                                                ),

                                                                                new XElement("PriceUnits",
                                                                                    new XAttribute("Items", "Color,Clarity,Price"),
                                                                                    dtPriceUnits.AsEnumerable()
                                                                                    .Where(g =>
                                                                                        g.Field<int>("ShapeIDUserFile") == shape.Field<int>("ShapeIDUserFile") &&
                                                                                        g.Field<int>("SizeIDUserFile") == Rng.Field<int>("SizeIDUserFile") &&
                                                                                        g.Field<string>("Cut") == grd.Field<string>("Cut"))
                                                                                    .Select(Price =>
                                                                                        new XAttribute("Prices", Price["Data"])
                                                                                    )
                                                                                    .FirstOrDefault() ?? new XAttribute("Prices", "")
                                                                                )
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),

                    new XElement("DiscountLists",
                        new XAttribute("NumOfDiscountLists", "2"),

                        new XElement("DiscountList",
                            new XAttribute("DiscountType", "1"),
                            new XAttribute("ID", "0"),
                            new XAttribute("Name", "ADPL"),
                            new XAttribute("StoneAttributesCollectionID", "2"),

                            new XElement("Shapes",
                                new XAttribute("NumOfShapes", "6"),

                                CreateShape("1", "ROUND",
                                    CreateGrade("3", "IDEAL", "Weight"),
                                    CreateGrade("4", "AT", "Diameter")
                                ),

                                CreateShape("1028", "LEO MQ",
                                    CreateGrade("1", "EX", "Weight")
                                ),

                                CreateShape("1003", "LONG CUSHION",
                                    CreateGrade("1", "EX", "Weight")
                                ),

                                CreateShape("1002", "RADIANT BF",
                                    CreateGrade("2", "EX mm", "Weight")
                                ),

                                CreateShape("1069", "SQ CUSHION",
                                    CreateGrade("1", "EX", "Weight")
                                ),

                                CreateShape("37", "EMERALD 4STEP",
                                    CreateGrade("3", "SQUARE", "Weight")
                                )
                            )
                        ),

                        new XElement("DiscountList",
                            new XAttribute("DiscountType", "1"),
                            new XAttribute("ID", "1"),
                            new XAttribute("Name", "J-0310"),
                            new XAttribute("StoneAttributesCollectionID", "2"),
                            new XElement("Shapes", new XAttribute("NumOfShapes", "0"))
                        )
                    )
                )
            );

            return xml.ToString();
        }

        XElement CreateShape(string id, string name, params XElement[] grades)
        {
            return new XElement("Shape",
                new XAttribute("ID", id),
                new XAttribute("Name", name),

                new XElement("Institutes",
                    new XAttribute("NumOfInstitutes", "1"),

                    new XElement("Institute",
                        new XAttribute("ID", "5005"),
                        new XAttribute("Name", "AD"),

                        new XElement("Grades",
                            new XAttribute("NumOfGrades", grades.Length),
                            grades
                        )
                    )
                )
            );
        }

        XElement CreateGrade(string id, string name, string type)
        {
            return new XElement("Grade",
                new XAttribute("ID", id),
                new XAttribute("Name", name),
                new XElement("Ranges",
                    new XAttribute("NumOfRanges", "0"),
                    new XAttribute("Type", type)
                )
            );
        }



        public ActionResult ApprovePricing()
        {
            SetModulePermissions("Pricing", "ApprovePricing");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canAdd = isAdmin || (ViewBag.CanAdd == true);
            bool canEdit = isAdmin || (ViewBag.CanEdit == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "Pricing"),
                        new SqlParameter("@ActionName", "ApprovePricing"));

                    if (permissionRows.Count > 0)
                    {
                        var row = permissionRows[0];
                        foreach (var kvp in row)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Add", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canAdd = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Edit", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canEdit = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        public JsonResult GetData_Price_Approved_Date_List()
        {
            List<object> _List = new List<object>();
            try
            {
                DataSet _DiamondEditData = DbHelper.ExecuteDataSet("GetData_Price_Approved_Date_List");

                for (int i = 0; i < _DiamondEditData.Tables[0].Rows.Count; i++)
                {
                    Dictionary<string, string> Values = new Dictionary<string, string>();
                    for (int j = 0; j < _DiamondEditData.Tables[0].Columns.Count; j++)
                    {
                        Values.Add(_DiamondEditData.Tables[0].Columns[j].ToString(), _DiamondEditData.Tables[0].Rows[i][j].ToString());
                    }
                    _List.Add(Values);
                }

                var jsonResult = Json(new
                {
                    List = _List
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

        public string TRN_Approve_Pricing_Insert(List<TRN_Pricing> PricingDetails,string ApproveDate, string Action)
        {
            string Message = "";
            try
            {
                string xmlStr = "";
                if (PricingDetails != null)
                {
                    XmlDocument Xmldata = CommonMethods.ConvertToXml(PricingDetails);
                    xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                }
                int userId = SessionFacade.UserSession?.UserID ?? 0;

                Message = DbHelper.ExecuteNonQueryWithMessage("TRN_Approve_Pricing_Insert",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@ApproveDate", ApproveDate),
                    new SqlParameter("@Action", Action),
                    new SqlParameter("@UserID", userId));

            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR : " + ex.InnerException.Message;
                return Message;
            }
            return Message;
        }

        public JsonResult Getdata_TRN_Pricing_Export(string SizeType, List<TRN_Pricing_Filter> SizeList, string Shape, string GradType, string Action)
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("Get_DataList_DIAMaster");

                var ColorList = ds.Tables[3].AsEnumerable().AsEnumerable()
                .Select(row => new colorList
                {
                    Color = row.Field<string>("ColorName"),
                })
                .ToList();

                var ClarityList = ds.Tables[6].AsEnumerable().AsEnumerable()
                .Select(row => new clarityList
                {
                    Clarity = row.Field<string>("MasterName"),
                })
                .ToList();

                string Data = "";
                string headername = "Pricing_" + GradType;

                string xmlStr = "";
                if (SizeList != null)
                {
                    XmlDocument Xmldata = CommonMethods.ConvertToXml(SizeList);
                    xmlStr = "<DocumentElement>" + Xmldata.DocumentElement.InnerXml + "</DocumentElement>";
                }


                DataSet _DropDownList = DbHelper.ExecuteDataSet("Getdata_TRN_Pricing_Export",
                    new SqlParameter("@SizeXML", SqlDbType.Xml) { Value = xmlStr },
                    new SqlParameter("@SizeType", SqlDbType.VarChar) { Value = SizeType },
                    new SqlParameter("@Shape", SqlDbType.VarChar) { Value = Shape },
                    new SqlParameter("@GradType", SqlDbType.VarChar) { Value = GradType },
                    new SqlParameter("@Action", SqlDbType.VarChar) { Value = Action });

                DataSet _DiamondEditData = new DataSet();

                var Sizes = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new sizeList
                {
                    Size = row.Field<string>("Size"),
                })
                .ToList();

                var List = _DropDownList.Tables[1].AsEnumerable().AsEnumerable()
                .Select(row => new TRN_Pricing
                {
                    Size = row.Field<string>("Size"),
                    Color = row.Field<string>("ColorName"),
                    Clarity = row.Field<string>("ClarityName"),
                    FinalRate = row.Field<string>("FinalRate"),
                })
                .ToList();

                if (Action == "Export")
                {
                    Data = ExcelExport.ExportPricingPivot(ColorList, ClarityList, Sizes, List, Shape, headername);
                }
                else { 
                    Data = ExcelExport.ExportPricing(ColorList, ClarityList, Sizes, List, Shape, headername);
                }

                var jsonResult = Json(new
                {
                    Data = Data
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
        public JsonResult Getdata_TRN_Approve_Pricing_Export(string Date,string GradType)
        {
            try
            {
                string Data = "";
                string headername = "Pricing" + "_" + Date;

                DataSet _DropDownList = DbHelper.ExecuteDataSet("Getdata_TRN_Approve_Pricing_Export",
                    new SqlParameter("@Date", SqlDbType.VarChar) { Value = Date },
                    new SqlParameter("@GradType", SqlDbType.VarChar) { Value = GradType });

                var ClarityList = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new clarityList
                {
                    Clarity = row.Field<string>("ClarityName"),
                })
                .ToList();

                var List = _DropDownList.Tables[1].AsEnumerable().AsEnumerable()
                .Select(row => new TRN_Pricing
                {
                    GradType = row.Field<string>("GradType"),
                    Shape = row.Field<string>("Shape"),
                    Size = row.Field<string>("Size"),
                    Color = row.Field<string>("ColorName"),
                    Clarity = row.Field<string>("ClarityName"),
                    Rate = row.Field<decimal>("Rate"),
                })
                .ToList();

                Data = ExcelExport.ExportApprovePricing( ClarityList, List, headername);

                var jsonResult = Json(new
                {
                    Data = Data
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

        public JsonResult Getdata_TRN_Approve_Pricing_Export_V2(string Date, string SizeType, string Shape, string GradType, string GridType)
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("Get_DataList_DIAMaster");

                var ColorList = ds.Tables[3].AsEnumerable().AsEnumerable()
                .Select(row => new colorList
                {
                    Color = row.Field<string>("ColorName"),
                })
                .ToList();

                var ClarityList = ds.Tables[6].AsEnumerable().AsEnumerable()
                .Select(row => new clarityList
                {
                    Clarity = row.Field<string>("MasterName"),
                })
                .ToList();

                string Data = "";
                string headername = "Pricing_" + Date + "_" + GradType + "_" + Shape;

                DataSet _DropDownList = DbHelper.ExecuteDataSet("Getdata_TRN_Approve_Pricing_Export_V2",
                    new SqlParameter("@Date", SqlDbType.VarChar) { Value = Date },
                    new SqlParameter("@SizeType", SqlDbType.VarChar) { Value = SizeType },
                    new SqlParameter("@Shape", SqlDbType.VarChar) { Value = Shape },
                    new SqlParameter("@GradType", SqlDbType.VarChar) { Value = GradType });
                DataSet _DiamondEditData = new DataSet();

                var Sizes = _DropDownList.Tables[0].AsEnumerable().AsEnumerable()
                .Select(row => new sizeList
                {
                    Size = row.Field<string>("Size"),
                })
                .ToList();

                var List = _DropDownList.Tables[1].AsEnumerable().AsEnumerable()
                .Select(row => new TRN_Pricing
                {
                    Size = row.Field<string>("Size"),
                    Color = row.Field<string>("ColorName"),
                    Clarity = row.Field<string>("ClarityName"),
                    FinalRate = row.Field<string>("FinalRate"),
                })
                .ToList();

                Data = ExcelExport.ExportApprovePricingV2(ColorList, ClarityList, Sizes, List, Shape, GradType, GridType, Date, headername);

                var jsonResult = Json(new
                {
                    Data = Data
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

        public JsonResult Pricing_Insert_Using_Excel(HttpPostedFileBase excelFile, string Shape, string GradType)
        {
            ExcelPackage.License.SetNonCommercialPersonal("ANJALI LABTECH");

            string Message = "";
            try
            {
                if (excelFile == null || excelFile.ContentLength == 0)
                    return Json(new { Message = "Please upload an Excel file" }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrEmpty(Shape) || string.IsNullOrEmpty(GradType))
                    return Json(new { Message = "Shape and GradType are required." }, JsonRequestBehavior.AllowGet);

                XDocument doc = new XDocument(new XElement("DocumentElement"));

                using (var package = new OfficeOpenXml.ExcelPackage(excelFile.InputStream))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[0];
                    if (ws == null) return Json(new { Message = "Uploaded Excel has no worksheets." }, JsonRequestBehavior.AllowGet);

                    int rowCount = ws.Dimension.End.Row;
                    int colCount = ws.Dimension.End.Column;

                    // Row 1 should have GroupName (Col 1), Color (Col 2), then Clarities (Col 3 onwards)
                    List<string> clarities = new List<string>();
                    for (int col = 3; col <= colCount; col++)
                    {
                        var cellVal = ws.Cells[1, col].Value;
                        if (cellVal != null && !string.IsNullOrWhiteSpace(cellVal.ToString()))
                        {
                            clarities.Add(cellVal.ToString().Trim());
                        }
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var sizeVal = ws.Cells[row, 1].Value;
                        var colorVal = ws.Cells[row, 2].Value;

                        if (sizeVal == null || colorVal == null) continue; // Skip blank rows

                        string sizeName = sizeVal.ToString().Trim();
                        string colorName = colorVal.ToString().Trim();

                        for (int col = 3; col <= 2 + clarities.Count; col++)
                        {
                            string clarityName = clarities[col - 3];
                            var rateVal = ws.Cells[row, col].Value;

                            string rateStr = rateVal != null ? rateVal.ToString().Trim() : "0";
                            if (string.IsNullOrWhiteSpace(rateStr)) rateStr = "0";

                            // Add to XML
                            XElement trnPricing = new XElement("TRN_Pricing",
                                new XElement("Shape", Shape),
                                new XElement("GradType", GradType),
                                new XElement("SizeName", sizeName),
                                new XElement("ColorName", colorName),
                                new XElement("ClarityName", clarityName),
                                new XElement("Rate", rateStr)
                            );

                            doc.Root.Add(trnPricing);
                        }
                    }
                }

                string xmlStr = doc.ToString();

                Message = DbHelper.ExecuteNonQueryWithMessage("TRN_Pricing_Insert_Using_Excel",
                    new SqlParameter("@XML", xmlStr),
                    new SqlParameter("@UserID", SessionFacade.UserSession.UserID));
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                Message = "ERROR: " + ex.Message;
            }

            return Json(new { Message = Message }, JsonRequestBehavior.AllowGet);
        }

        #region :: Price Comparison ::
        public ActionResult PriceComparison()
        {
            SetModulePermissions("Pricing", "PriceComparison");

            bool isAdmin = SessionFacade.UserSession != null && SessionFacade.UserSession.IsAdmin;
            bool canView = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true || ViewBag.CanDelete == true);
            bool canExport = isAdmin || (ViewBag.CanAdd == true || ViewBag.CanEdit == true);

            try
            {
                if (SessionFacade.UserSession != null && !isAdmin)
                {
                    List<Dictionary<string, object>> permissionRows = DbHelper.ExecuteReaderAsList("Get_UserModulePermission",
                        new SqlParameter("@UserID", SessionFacade.UserSession.UserID),
                        new SqlParameter("@ControllerName", "Pricing"),
                        new SqlParameter("@ActionName", "PriceComparison"));

                    if (permissionRows.Count > 0)
                    {
                        var row = permissionRows[0];
                        foreach (var kvp in row)
                        {
                            string col = kvp.Key;
                            if (col.Equals("View", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canView = Convert.ToBoolean(kvp.Value);
                            }
                            else if (col.Equals("Export", StringComparison.OrdinalIgnoreCase) && kvp.Value != null)
                            {
                                canExport = Convert.ToBoolean(kvp.Value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
            }

            ViewBag.CanView = canView;
            ViewBag.CanExport = canExport;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }

        [HttpPost]
        public JsonResult GetPriceComparisonData(string Shape, string GradType)
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("Getdata_MST_Pricing_Diff",
                    new SqlParameter("@Shape", string.IsNullOrEmpty(Shape) ? "Round" : Shape),
                    new SqlParameter("@GradType", string.IsNullOrEmpty(GradType) ? "GR" : GradType));

                List<object> colors = new List<object>();
                List<object> clarities = new List<object>();
                List<string> sizeGroups = new List<string>();
                List<object> comparisonData = new List<object>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        colors.Add(new { ID = dr["ColorID"], Name = dr["Color"].ToString() });
                    }
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        clarities.Add(new { ID = dr["ClarityID"], Name = dr["Clarity"].ToString() });
                    }
                }

                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        sizeGroups.Add(dr["SizeGroup"].ToString());
                    }
                }

                if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        comparisonData.Add(new
                        {
                            ShapeCategory = dr["ShapeCategory"].ToString(),
                            Color = dr["Color"].ToString(),
                            Clarity = dr["Clarity"].ToString(),
                            SizeGroup = dr["SizeGroup"].ToString(),
                            NewPrice = dr["NewPrice"],
                            OldPrice = dr["OldPrice"],
                            DiffAmt = dr["DiffAmt"]
                        });
                    }
                }

                return Json(new
                {
                    Colors = colors,
                    Clarities = clarities,
                    SizeGroups = sizeGroups,
                    ComparisonData = comparisonData
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { Status = "Error", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ExportPriceComparisonExcel(string Shape, string GradType)
        {
            try
            {
                DataSet ds = DbHelper.ExecuteDataSet("Getdata_MST_Pricing_Diff",
                    new SqlParameter("@Shape", string.IsNullOrEmpty(Shape) ? "Round" : Shape),
                    new SqlParameter("@GradType", string.IsNullOrEmpty(GradType) ? "GR" : GradType));

                string filePath = ExcelExport.ExportPriceComparisonExcel(ds, Shape, GradType);
                if (string.IsNullOrEmpty(filePath))
                {
                    return Json(new { Status = "Error", Message = "Failed to generate Excel" }, JsonRequestBehavior.AllowGet);
                }

                string fileName = Path.GetFileName(filePath);
                return Json(new { Status = "Success", FileName = fileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.ErrorLog(ex);
                return Json(new { Status = "Error", Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion :: Price Comparison ::
    }
}