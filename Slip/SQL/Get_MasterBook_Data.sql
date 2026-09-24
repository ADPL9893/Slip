USE [SLIP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[Get_MasterBook_Data]
    @FilterType VARCHAR(100) = 'Rough Entry',
    @FromDate VARCHAR(50) = '',
    @ToDate VARCHAR(50) = ''
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #OutputMasterBook
    (
        ID INT,
        JangadNo VARCHAR(50),
        Priority VARCHAR(50),
        RName VARCHAR(50),
        RCode VARCHAR(50),
        BranchName VARCHAR(50),
        RoughPcs INT,
        RoughWeight DECIMAL(18,3),
        OriginalCarat DECIMAL(18,3),
        Size VARCHAR(50),
        SizeCode VARCHAR(50),
        ColorType VARCHAR(50),
        FactoryCode VARCHAR(50),
        BoxNo VARCHAR(50),
        Party VARCHAR(100),
        InvoiceNo VARCHAR(100),
        InvoiceDate VARCHAR(50),
        PurchaserName VARCHAR(100),
        StageStatus VARCHAR(100),
        StageDate VARCHAR(50),
        StageBy VARCHAR(100),
        Remarks VARCHAR(MAX),
        IsActive BIT,
        CreatedDate VARCHAR(50),
        -- Fixed Rear Columns
        MainRate DECIMAL(18,2),
        Rate DECIMAL(18,2),
        MainGrad VARCHAR(50),
        Grade VARCHAR(50),
        MainHeight VARCHAR(100),
        Height VARCHAR(100)
    );

    -- 1. ROUGH ENTRY: Z_R_Rough_Add records not yet assigned in MST_RoughDistribution
    IF @FilterType = 'Rough Entry'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            p.ID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            'REGULAR' AS Priority,
            ISNULL(p.RName, '') AS RName,
            '' AS RCode,
            'Not Assigned' AS BranchName,
            ISNULL(p.RoughPcs, 0) AS RoughPcs,
            ISNULL(p.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'Rough Entry' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), p.Created, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(p.Remarks, '') AS Remarks,
            ISNULL(p.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), p.Created, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            0.00 AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            '' AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            '' AS Height
        FROM Z_R_Rough_Add p WITH(NOLOCK)
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = p.UserID
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, p.Created) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, p.Created) <= CONVERT(DATE, @ToDate))
          AND NOT EXISTS (SELECT 1 FROM MST_RoughDistribution rd WITH(NOLOCK) WHERE rd.PreRoughID = p.ID);
    END

    -- 2. BRANCH ASSIGN: Allocated in MST_RoughDistribution, not moved to Planning
    ELSE IF @FilterType = 'Branch Assign'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'Branch Assigned' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.CreatedByUserID
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.CreatedDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.CreatedDate) <= CONVERT(DATE, @ToDate))
          AND (d.IsPlanning = 0 OR d.IsPlanning IS NULL);
    END

    -- 3. PLANNING
    ELSE IF @FilterType = 'Planning'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'In Planning' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.PlanningDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.PlanningEntryBy
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.PlanningDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.PlanningDate) <= CONVERT(DATE, @ToDate))
          AND d.IsPlanning = 1
          AND (d.IsCLVRunning = 0 OR d.IsCLVRunning IS NULL);
    END

    -- 4. CLV RUNNING
    ELSE IF @FilterType = 'CLV Running'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'CLV Running' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.CLVRunningDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.CLVRunningEntryBy
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.CLVRunningDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.CLVRunningDate) <= CONVERT(DATE, @ToDate))
          AND d.IsCLVRunning = 1
          AND (d.IsCLVComplete = 0 OR d.IsCLVComplete IS NULL);
    END

    -- 5. CLV COMPLETE
    ELSE IF @FilterType = 'CLV Complte'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'CLV Complete' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.CLVCompleteDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.CLVCompleteEntryBy
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.CLVCompleteDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.CLVCompleteDate) <= CONVERT(DATE, @ToDate))
          AND d.IsCLVComplete = 1
          AND (d.IsMFGRunning = 0 OR d.IsMFGRunning IS NULL);
    END

    -- 6. MFG RUNNING
    ELSE IF @FilterType = 'MFG Running'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'MFG Running' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.MFGRunningDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.MFGRunningEntryBy
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.MFGRunningDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.MFGRunningDate) <= CONVERT(DATE, @ToDate))
          AND d.IsMFGRunning = 1
          AND (d.IsMFGComplete = 0 OR d.IsMFGComplete IS NULL);
    END

    -- 7. MFG COMPLETE
    ELSE IF @FilterType = 'MFG Complte'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'MFG Complete' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.MFGCompleteDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.MFGCompleteEntryBy
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.MFGCompleteDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.MFGCompleteDate) <= CONVERT(DATE, @ToDate))
          AND d.IsMFGComplete = 1
          AND (d.IsMumbaiSubmit = 0 OR d.IsMumbaiSubmit IS NULL);
    END

    -- 8. MUMBAI SUBMIT
    ELSE IF @FilterType = 'Mumbai Submit'
    BEGIN
        INSERT INTO #OutputMasterBook
        SELECT 
            d.DistributionID AS ID,
            ISNULL(p.JangadNo, '') AS JangadNo,
            ISNULL(d.Priority, 'REGULAR') AS Priority,
            ISNULL(p.RName, '') AS RName,
            ISNULL(d.RCode, '') AS RCode,
            ISNULL(b.Value, 'Branch #' + CAST(d.BranchID AS VARCHAR)) AS BranchName,
            ISNULL(d.RoughPcs, 0) AS RoughPcs,
            ISNULL(d.RoughWeight, 0) AS RoughWeight,
            ISNULL(p.OriginalCarate, 0) AS OriginalCarat,
            ISNULL(p.Size, '') AS Size,
            ISNULL(p.SizeCode, '') AS SizeCode,
            ISNULL(p.ColorType, '') AS ColorType,
            ISNULL(p.FactoryCode, '') AS FactoryCode,
            ISNULL(p.BoxNo, '') AS BoxNo,
            ISNULL(p.Party, '') AS Party,
            ISNULL(p.InvoiceNo, '') AS InvoiceNo,
            ISNULL(CONVERT(VARCHAR(20), p.InvoiceDate, 106), '') AS InvoiceDate,
            ISNULL(pu.UserName, '') AS PurchaserName,
            'Mumbai Submitted' AS StageStatus,
            ISNULL(CONVERT(VARCHAR(20), d.MumbaiSubmitDate, 106), '') AS StageDate,
            ISNULL(u.UserName, '') AS StageBy,
            ISNULL(d.Remarks, '') AS Remarks,
            ISNULL(d.IsActive, 1) AS IsActive,
            ISNULL(CONVERT(VARCHAR(20), d.CreatedDate, 106), '') AS CreatedDate,
            -- Rear Columns
            ISNULL(p.Rate, 0.00) AS MainRate,
            ISNULL(d.Rate, 0.00) AS Rate,
            ISNULL(p.GradNo, '') AS MainGrad,
            ISNULL(d.GradNo, '') AS Grade,
            ISNULL(CONVERT(VARCHAR(30), p.FromHeight) + ' - ' + CONVERT(VARCHAR(30), p.ToHeight), '') AS MainHeight,
            ISNULL(CONVERT(VARCHAR(30), d.FromHeight) + ' - ' + CONVERT(VARCHAR(30), d.ToHeight), '') AS Height
        FROM MST_RoughDistribution d WITH(NOLOCK)
        INNER JOIN Z_R_Rough_Add p WITH(NOLOCK) ON p.ID = d.PreRoughID
        LEFT JOIN MST_GeneralSetting b WITH(NOLOCK) ON b.ID = d.BranchID AND b.MasterType = 'Branch'
        LEFT JOIN SEC_User u WITH(NOLOCK) ON u.UserID = d.MumbaiSubmitEntryBy
        LEFT JOIN SEC_User pu WITH(NOLOCK) ON pu.UserID = p.PurchaseByUserID
        WHERE (@FromDate = '' OR CONVERT(DATE, d.MumbaiSubmitDate) >= CONVERT(DATE, @FromDate))
          AND (@ToDate = '' OR CONVERT(DATE, d.MumbaiSubmitDate) <= CONVERT(DATE, @ToDate))
          AND d.IsMumbaiSubmit = 1;
    END

    -- TABLE 0: Primary Grid Dataset
    SELECT * FROM #OutputMasterBook ORDER BY ID DESC;

    -- TABLE 1: Column Definitions Ordered with the specified rear columns
    SELECT 
        'JangadNo' AS dataField, 'Jangad No' AS caption, 'string' AS dataType, '' AS [format], CAST(1 AS BIT) AS visible, 'center' AS alignment, 1 AS Seq UNION ALL
    SELECT 'Priority', 'Priority', 'string', '', CAST(1 AS BIT), 'center', 2 UNION ALL
    SELECT 'RName', 'R. Name', 'string', '', CAST(1 AS BIT), 'left', 3 UNION ALL
    SELECT 'RCode', 'R. Code', 'string', '', CAST(1 AS BIT), 'center', 4 UNION ALL
    SELECT 'BranchName', 'Branch', 'string', '', CAST(1 AS BIT), 'left', 5 UNION ALL
    SELECT 'RoughPcs', 'Rough Pcs', 'number', '#,##0', CAST(1 AS BIT), 'right', 6 UNION ALL
    SELECT 'RoughWeight', 'Rough Wt (Cts)', 'number', '#,##0.000', CAST(1 AS BIT), 'right', 7 UNION ALL
    SELECT 'OriginalCarat', 'Orig Carat', 'number', '#,##0.000', CAST(1 AS BIT), 'right', 8 UNION ALL
    SELECT 'Size', 'Size', 'string', '', CAST(1 AS BIT), 'center', 9 UNION ALL
    SELECT 'SizeCode', 'Size Code', 'string', '', CAST(1 AS BIT), 'center', 10 UNION ALL
    SELECT 'ColorType', 'Color Type', 'string', '', CAST(1 AS BIT), 'center', 11 UNION ALL
    SELECT 'FactoryCode', 'Factory Code', 'string', '', CAST(1 AS BIT), 'center', 12 UNION ALL
    SELECT 'BoxNo', 'Box No', 'string', '', CAST(1 AS BIT), 'center', 13 UNION ALL
    SELECT 'Party', 'Party', 'string', '', CAST(1 AS BIT), 'left', 14 UNION ALL
    SELECT 'InvoiceNo', 'Invoice No', 'string', '', CAST(1 AS BIT), 'left', 15 UNION ALL
    SELECT 'InvoiceDate', 'Invoice Date', 'string', '', CAST(1 AS BIT), 'center', 16 UNION ALL
    SELECT 'PurchaserName', 'Purchaser', 'string', '', CAST(1 AS BIT), 'left', 17 UNION ALL
    SELECT 'StageStatus', 'Stage', 'string', '', CAST(1 AS BIT), 'center', 18 UNION ALL
    SELECT 'StageDate', 'Stage Date', 'string', '', CAST(1 AS BIT), 'center', 19 UNION ALL
    SELECT 'StageBy', 'Stage By', 'string', '', CAST(1 AS BIT), 'left', 20 UNION ALL
    SELECT 'Remarks', 'Remarks', 'string', '', CAST(1 AS BIT), 'left', 21 UNION ALL
    SELECT 'IsActive', 'Status', 'boolean', '', CAST(1 AS BIT), 'center', 22 UNION ALL
    -- Ending Sequence strictly enforced:
    SELECT 'MainRate', 'Main Rate', 'number', '#,##0.00', CAST(1 AS BIT), 'right', 23 UNION ALL
    SELECT 'Rate', 'Rate', 'number', '#,##0.00', CAST(1 AS BIT), 'right', 24 UNION ALL
    SELECT 'MainGrad', 'Main Grad', 'string', '', CAST(1 AS BIT), 'center', 25 UNION ALL
    SELECT 'Grade', 'Grade', 'string', '', CAST(1 AS BIT), 'center', 26 UNION ALL
    SELECT 'MainHeight', 'Main Height', 'string', '', CAST(1 AS BIT), 'center', 27 UNION ALL
    SELECT 'Height', 'Height', 'string', '', CAST(1 AS BIT), 'center', 28
    ORDER BY Seq;

    -- TABLE 2: Dynamic Summary Footers
    SELECT 'RoughPcs' AS [column], 'sum' AS summaryType, '#,##0' AS valueFormat, 'Total: {0} Pcs' AS displayFormat UNION ALL
    SELECT 'RoughWeight', 'sum', '#,##0.000', 'Total: {0} Cts' UNION ALL
    SELECT 'OriginalCarat', 'sum', '#,##0.000', 'Total: {0} Cts';

    DROP TABLE #OutputMasterBook;
END
GO
