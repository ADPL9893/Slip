# Slip — System Architecture Document

> **Purpose:** Internal diamond manufacturing/processing management system.
> **Author (doc):** Antigravity AI — read-only analysis, no code was changed.

---

## 1. Technology Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET MVC 5 (.NET Framework 4.7.2) |
| Web API | ASP.NET Web API 2 (side-by-side with MVC) |
| Language | C# |
| Database | Microsoft SQL Server (integrated security) |
| ORM / Data access | Raw ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`) |
| UI / Client | Razor (`.cshtml`), DevExtreme UI components, jQuery, JavaScript |
| Bundling | ASP.NET Optimization (`BundleConfig`, `DevExtremeBundleConfig`) |
| Excel I/O | EPPlus (`OfficeOpenXml`), ExcelDataReader |
| Serialization | Newtonsoft.Json (v13), XML Serializer |
| Barcode / QR | Separate barcode generation controller |
| Auth | Forms Authentication (cookie-based, 30-min timeout) |
| Session | ASP.NET Session (30-min timeout) |
| Logging | Custom file-based logger (`~/Logs/*.txt`) |

---

## 2. Solution Structure

```
Slip.sln
└── Slip/                          ← Main MVC project (namespace: Slip)
    ├── Global.asax(.cs)           ← App bootstrap + scheduled job
    ├── Web.config                 ← Connection string, app settings, auth config
    ├── Web.Debug.config
    ├── Web.Release.config
    ├── favicon.ico
    ├── packages.config
    │
    ├── App_Start/                 ← MVC startup configuration
    │   ├── BundleConfig.cs
    │   ├── DevExtremeBundleConfig.cs
    │   ├── FilterConfig.cs
    │   ├── RouteConfig.cs
    │   └── WebApiConfig.cs
    │
    ├── Controllers/               ← MVC Controllers (12 files)
    ├── API/                       ← Web API Controller (1 file)
    ├── Models/                    ← POCOs / domain models (35+ files)
    │   └── Filter/                ← MVC action filters
    ├── Views/                     ← Razor views (13 sub-folders)
    │   └── Shared/                ← Layouts and error page
    ├── Utility/                   ← Cross-cutting helpers (7 files)
    ├── Content/                   ← CSS / static content
    ├── Scripts/                   ← JavaScript files
    ├── Images/
    ├── Upload/                    ← Runtime file storage (Excel, images, backups)
    │   ├── ExportExcel/
    │   ├── DiamondExcelFile/
    │   ├── PortalData/
    │   ├── BackUp/
    │   ├── CVD/
    │   ├── QCFileUpload/
    │   ├── CasseteFiles/
    │   └── KapanImages/
    ├── Logs/                      ← Error / debug log files (txt)
    └── bin/ / obj/
```

---

## 3. Application Bootstrap (`Global.asax.cs`)

- Registers bundles, Web API config, areas, global filters, routes.
- **Scheduled Job:** Launches a `System.Threading.Timer` on startup that fires daily at **7:05 PM IST**.
  - Calls `PricingController.TRN_Approve_Pricing_Insert(null, "", "Save")` automatically.
  - Self-reschedules on each fire (one-shot timer, not periodic).
  - Errors are swallowed silently (only `Debug.WriteLine`).

---

## 4. Routing

Default route (conventional MVC):

```
{controller}/{action}/{id}
Default: Login/Index
```

Web API route registered separately via `WebApiConfig`. The single API controller lives in `API/APIController.cs`.

---

## 5. Controllers

All MVC controllers (except `LoginController`) inherit from `BaseController`.

### 5.1 `BaseController`
- Inherits `Controller`.
- Decorated with `[SessionExpireFilterAttribute]` — enforces session on all derived controllers.
- Exposes a protected `conn` property that reads the connection string from `Web.config` (`StockDetailConnectionString`).

---

### 5.2 Controller Inventory

| Controller | View folder | Primary Responsibility |
|---|---|---|
| `LoginController` | `Login/` | Authentication (login, logout, password change). Does **not** inherit `BaseController` — handles unauthenticated requests. |
| `HomeController` | `Home/` | Landing page after login. |
| `DashboardController` | `Dashboard/` | KPI / summary data for the dashboard view. |
| `SlipController` | `Slip/` | **Core domain controller** — all slip entry operations (scanning, sawing, 4P, HPHT, auto-polish, Mumbai submit, etc.). Largest file (~161 KB / 3163 lines). |
| `MasterController` | `Master/` | Master data management (lot, pre-rough, kapan images, TRN labour, Excel upload). |
| `PricingController` | `Pricing/` | Pricing transactions and automated daily approval job target. |
| `ReportController` | `Report/` | All analytical reports (daily, DST, cleaving, MFG, 4P OK summary, loss, prediction, process timing, etc.). |
| `PrintController` | `Print/` | Print-ready views / print data retrieval. |
| `EmployeesController` | `Employees/` | Employee management. |
| `UserController` | `User/` | User/role/privilege management. |
| `BarcodeController` | `Barcode/` | Barcode generation. |
| **`APIController`** | *(API)* | REST Web API endpoint — exposes data for external/portal consumers. Uses `BasicAuthenticationAttribute` for auth. |

---

## 6. Domain (Business) Overview

The application manages the **diamond cutting and polishing manufacturing pipeline**. The domain terminology:

| Term | Meaning |
|---|---|
| **Rough** | Raw/uncut diamond |
| **Slip** | A production record/entry for a diamond stone or batch |
| **Scanning** | Initial quality scanning step |
| **4P / FourP** | Four-parameter diamond grading process |
| **Sawing** | Cutting/sawing process stage |
| **HPHT** | High Pressure High Temperature treatment |
| **Auto Polish** | Automated polishing stage (Round / Fancy shapes) |
| **Pre-Polish** | Pre-polishing step |
| **Table Check** | Polishing table quality check |
| **Jangad** | Consignment/lot identifier |
| **KapanImage** | Photo of a rough stone batch (kapan) |
| **CVD** | Chemical Vapour Deposition process |
| **Cleaving** | Stone splitting |
| **Mumbai Submit** | Dispatching stones to Mumbai office |
| **Process Timing** | Time tracking per process stage |
| **Loss %** | Weight/piece loss at each stage |

---

## 7. Models Layer

### 7.1 Security Models (`SEC_*`)
| Model | Purpose |
|---|---|
| `SEC_User` | Authenticated user (UserID, RoleID, IsAdmin, TableNo, IsDashBoardShow) |
| `SEC_Role` (MST_Role) | Role definition |
| `SEC_RolePrivileges` | Role-level permissions |
| `SEC_UserPermission` | User-level permission overrides |
| `SEC_UserPrivileges` | User privileges |
| `SEC_LoginHistory` | Login audit trail |

### 7.2 Core Domain Models
| Model | Purpose |
|---|---|
| `RP_Slip_Entry` | Master slip entry model — 390+ properties covering all process stages |
| `MST_Lot` | Lot/batch master |
| `EMP_Employees` | Employee data |
| `TRN_Labour` | Labour transaction |
| `TRN_Pricing` | Pricing transaction |
| `TRN_PreProcess` | Pre-process transaction |
| `TRN_Process_Timing` | Process timing per stage |
| `Slip_Sawing_Machine` | Sawing machine slip |
| `Slip_Scanning_STN` | Scanning station slip |
| `FourP_Pridiction` | 4P prediction model |
| `Pridiction` | General prediction |
| `R_Rough_Add` | Rough stone add |
| `QRList` | QR code list |
| `PriceXML` | XML-based price data |

### 7.3 Report Models
| Model | Purpose |
|---|---|
| `DailyReport` / `DailyReportModels` | Daily production report |
| `MainRoughSummary` | Rough stone summary |
| `Rp_Prd_Summary` | Production summary |
| `Daily_RP_Rough_Polish` | Daily rough/polish summary |
| `ProcessWiseTiming` | Process-wise time tracking |
| `Process_Wise_Issue_Receive_Loss` | Issue/receive/loss per process |
| `After4POk_Loss` / `Api_After4POk_Loss` | Post-4P OK loss |
| `RoughTo4POk_Loss` / `Api_RoughTo4POk_Loss` | Rough to 4P OK loss |

### 7.4 Infrastructure Models
| Model | Purpose |
|---|---|
| `DataFields` | Dynamic column definition for DevExtreme grids (dataField, dataType, format, alignment, visible) |
| `CheckPermission_User` | Flattened user permission record (ModuleID, Action, Controller, GroupID, SubMenu) |
| `Api_Response` | Standard API response wrapper |
| `Api_TRN_Process_Timing` | API-specific process timing |

### 7.5 Filter Models (`Models/Filter/`)
| Class | Purpose |
|---|---|
| `SessionExpireFilter` > `SessionExpireFilterAttribute` | Redirects unauthenticated page requests to `~/Login`; returns HTTP 401 on unauthenticated AJAX calls |
| `SessionExpireFilter` > `ValidateJsonAntiForgeryTokenAttribute` | AntiForgery token validation for JSON requests |
| `BasicAuthenticationAttribute` | HTTP Basic Auth for the Web API |

---

## 8. Utility Layer (`Utility/`)

| File | Key Responsibilities |
|---|---|
| `CommonMethods.cs` | Pivot DataTable, generic `ConvertDataTable<T>`, `ConvertToXml` (serialize object → XmlDocument), Excel read (EPPlus + OleDb), `Encrypt`/`Decrypt` (TripleDES/MD5), number-to-words, year/month→alphabet mapping, SMS OTP sender |
| `SessionFacade.cs` | Typed wrappers around `HttpContext.Current.Session` for `UserSession` (`SEC_User`), `FormPermissions`, `FormPermissionsGroup`, `QRGenerateList` |
| `ErrorLogger.cs` | Writes timestamped `.txt` log files to `~/Logs/` — three overloads: `ErrorLog(Exception)`, `ErrorLog(string)`, `ErrorLogStr(string)` |
| `ExcelExport.cs` | Large (~147 KB) utility for generating Excel exports |
| `FormPermissionHelper.cs` | Permission check helpers used in views/controllers |
| `RequestHelpers.cs` | Config value readers (`GetConfigValue`), server path helpers |
| `jQueryDataTableParamModel.cs` | Server-side DataTables parameters model |

---

## 9. Security & Authentication

```
Request
  │
  ├─ LoginController (no session filter)
  │     └─ Check_Login SP → populate SEC_User → Session["UserLogin"]
  │     └─ Get_CheckPermission_User SP → populate FormPermissions in session
  │
  └─ All other MVC controllers (inherit BaseController)
        └─ [SessionExpireFilterAttribute] on every action
              ├─ AJAX: return HTTP 401 if no session
              └─ Page: redirect to ~/Login if no session

API/APIController
  └─ [BasicAuthenticationAttribute] — HTTP Basic Auth
```

**Session data stored:**
- `"UserLogin"` → `SEC_User` (UserID, RoleID, IsAdmin, TableNo, …)
- `"FormPermissions"` → `List<CheckPermission_User>` (form-level access)
- `"FormPermissionsGroup"` → `List<CheckPermission_User>` (group-level menu)
- `"QRGenerateNew"` → `List<QRList>` (QR temp data)

> ⚠️ Passwords appear to be stored/transmitted in plain text (no hashing observed in `LoginController`).

---

## 10. Data Access Pattern

**No ORM is used.** All data access is raw ADO.NET following a consistent pattern:

```csharp
// Read
using (SqlConnection con = new SqlConnection(conn))
using (SqlCommand cmd = new SqlCommand("StoredProcedureName", con))
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@Param", value);
    con.Open();
    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    adapter.Fill(dataset);
}

// Write (with XML payload)
XmlDocument xml = CommonMethods.ConvertToXml(model);
string xmlStr = "<DocumentElement><Entity>" + xml.DocumentElement.InnerXml + "</Entity></DocumentElement>";
cmd.Parameters.AddWithValue("@XML", xmlStr);
SqlParameter msg = new SqlParameter("@MESSAGE", SqlDbType.VarChar, 1000) { Direction = ParameterDirection.Output };
cmd.Parameters.Add(msg);
cmd.ExecuteNonQuery();
string result = cmd.Parameters["@MESSAGE"].Value.ToString();
```

**All writes pass model data serialized as XML** to stored procedures. The stored procedure returns a `@MESSAGE` output parameter with success/error text.

**Multi-result sets:** Many read operations fill a `DataSet` with multiple tables (columns metadata, summary config, data rows) that are consumed separately.

---

## 11. Views Layer

| Folder | Key Views |
|---|---|
| `Shared/` | `_Layout.cshtml` (main shell), `_LoginLayout.cshtml`, `Error.cshtml` |
| `Login/` | Login page |
| `Home/` | Post-login home |
| `Dashboard/` | `Dashboard.cshtml` — KPI charts |
| `Slip/` | `Slip_Entry.cshtml` (~278 KB — main data entry form), `SawingMachineSlip`, `FourPDailySlip`, `ScanningSTNSlip`, `Process_Timing`, `MumbaiSubmit`, `AddReports`, `Vipulbhai` |
| `Master/` | `Lot`, `PreRough`, `KapanImg`, `TRNLabour`, `SlipExcelUpload` |
| `Report/` | 11 report views (Daily, DST, Cleaving, MFG, HPHT, Prediction, ProcessWiseTiming, SlipReport, FourPOkLotSummary, DateWiseLossJumbo, ProcessWiseIssueReceiveLoss) |
| `Pricing/` | Pricing entry/approval views |
| `Print/` | Print-ready templates |
| `Employees/` | Employee CRUD |
| `User/` | User/role management |
| `Barcode/` | Barcode generation UI |
| `Base/` | Base layout fragments |

---

## 12. Configuration (`Web.config`)

| Key | Value / Purpose |
|---|---|
| `StockDetailConnectionString` | SQL Server `SLIP` database on `DESKTOP-C70BOOF`, Integrated Security |
| `ExcelFiles` | `~/Upload/ExportExcel` |
| `ExcelFilesSetUp` | `~/Upload/DiamondExcelFile` |
| `ExcelFilesPortal` | `~/Upload/PortalData` |
| `DBBackUp` | `~/Upload/BackUp` |
| `CVDExcel` | `~/Upload/CVD` |
| `ExcelFilesQC` | `~/Upload/QCFileUpload` |
| `XMLCasseteFiles` | `~/Upload/CasseteFiles` |
| `KapanImages` | `~/Upload/KapanImages` |
| `AccessKey` / `SecKey` | AWS credentials (hardcoded — likely S3 integration) |
| `aspnet:MaxJsonDeserializerMembers` | `2147483647` (max JSON size) |
| Forms auth | Login: `~/Login/Index`, timeout: 30 min |
| Session timeout | 30 min |
| `executionTimeout` | 5,000,000 seconds (very large — supports long-running operations) |
| `maxRequestLength` | 1 GB (large file uploads) |

---

## 13. External Integrations

| Integration | Location | Notes |
|---|---|---|
| **AWS S3** | `Web.config` (`AccessKey`, `SecKey`) | Keys present; usage in code not confirmed in this review |
| **SMS OTP** | `CommonMethods.SendOTPCode()` | Calls `redsms.in` API with hardcoded credentials |
| **DevExtreme** | `DevExtremeBundleConfig.cs`, all views | DevExpress UI grid/chart library |

---

## 14. Architecture Diagram

```
Browser (DevExtreme + jQuery)
        │  HTTP / AJAX
        ▼
┌──────────────────────────────────────────┐
│           ASP.NET MVC 5 / Web API        │
│                                          │
│  LoginController   ──► Session Auth      │
│  BaseController    ◄── [SessionFilter]   │
│    ├── DashboardController               │
│    ├── SlipController  (core)            │
│    ├── MasterController                  │
│    ├── PricingController ◄── Timer Job   │
│    ├── ReportController                  │
│    ├── PrintController                   │
│    ├── EmployeesController               │
│    ├── UserController                    │
│    └── BarcodeController                 │
│                                          │
│  APIController  ◄── [BasicAuth]          │
└──────────┬───────────────────────────────┘
           │  ADO.NET (Raw SQL / Stored Procs)
           │  Data passed as XML strings
           ▼
┌──────────────────────┐    ┌─────────────┐
│  SQL Server (SLIP DB)│    │  ~/Logs/    │
│  Stored Procedures   │    │  (txt files)│
└──────────────────────┘    └─────────────┘
           │
┌──────────────────────┐
│  ~/Upload/           │
│  (Excel, Images, etc)│
└──────────────────────┘
```

---

## 15. Notable Observations (No Changes Made)

> These are architectural observations for awareness. All are read-only findings.

1. **Monolithic Controller** — `SlipController.cs` is 3,163 lines / 161 KB. It handles all slip entry variants without sub-controllers or services.
2. **No Repository / Service Layer** — Data access SQL is directly embedded in controller action methods. No abstraction between controller and database.
3. **XML as DB payload** — Every write serializes the entire model object to XML and passes it to a stored procedure. This is a deliberate pattern (not an ORM substitute).
4. **Plain-text passwords** — `LoginController` passes username/password directly to `Check_Login` SP with no client or server-side hashing.
5. **Hardcoded credentials in config** — AWS keys and SMS API credentials are stored in `Web.config` in plain text.
6. **Scheduled job in Global.asax** — Background `Timer` runs in the IIS app pool process. App pool recycling will reset the timer; no persistent job queue.
7. **Large model** — `RP_Slip_Entry` has ~390 properties covering all possible process-stage metrics in a single flat class.
8. **MaxJsonLength overrides** — Multiple places set `jsonResult.MaxJsonLength = Int32.MaxValue` to handle large data grid payloads.
9. **Dead code** — Several commented-out code blocks exist in `Global.asax.cs`, `SessionFacade.cs`, `CommonMethods.cs` that were previous implementations.
10. **Connection string target** — Points to `DESKTOP-C70BOOF` with Integrated Security — a local developer machine name. Production deploy would require config transform.

---

*Document generated: 2026-05-24 | Read-only analysis — no source files were modified.*
