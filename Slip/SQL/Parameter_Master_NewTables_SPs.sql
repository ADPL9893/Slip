USE [SLIP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================================
-- 1. DIMENSION MASTER (MST_Dimension) STORED PROCEDURES
-- =========================================================================================

-- 1.1 GET GRID LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Dimension_GetGridList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        D.[ID],
        ISNULL(D.[DimensionName], '') AS [DimensionName],
        ISNULL(D.[OrderBy], 0)        AS [OrderBy],
        ISNULL(D.[IsActive], 0)       AS [IsActive],
        D.[CreatedBy],
        D.[CreatedDate],
        D.[UpdatedBy],
        D.[UpdatedDate]
    FROM [dbo].[MST_Dimension] D WITH(NOLOCK)
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = '' 
        OR D.[DimensionName] LIKE '%' + @SearchText + '%'
    )
    ORDER BY ISNULL(D.[OrderBy], 999), D.[DimensionName] ASC;
END
GO

-- 1.2 INSERT / UPDATE (INDIVIDUAL PARAMETERS - NO XML)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Dimension_Insert_Update]
    @ACTION        VARCHAR(20)   = 'INSERT',
    @ID            INT           = 0,
    @DimensionName NVARCHAR(100) = NULL,
    @OrderBy       INT           = NULL,
    @IsActive      BIT           = 1,
    @UserID        INT           = 1,
    @MESSAGE       VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @DimensionName = LTRIM(RTRIM(@DimensionName));

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@DimensionName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Dimension Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Dimension
            WHERE LOWER(REPLACE(DimensionName, ' ', '')) = LOWER(REPLACE(@DimensionName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Dimension Name already exists!'; 
            RETURN; 
        END

        IF @OrderBy IS NULL OR @OrderBy = 0
        BEGIN
            SELECT @OrderBy = ISNULL(MAX(OrderBy), 0) + 1 FROM dbo.MST_Dimension;
        END

        INSERT INTO dbo.MST_Dimension
        (
            DimensionName, OrderBy, CreatedBy, CreatedDate, IsActive
        )
        VALUES
        (
            @DimensionName, @OrderBy, @UserID, GETDATE(), ISNULL(@IsActive, 1)
        );

        SET @MESSAGE = 'Dimension saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@ID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Dimension ID!'; 
            RETURN; 
        END

        IF ISNULL(@DimensionName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Dimension Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Dimension
            WHERE ID <> @ID 
              AND LOWER(REPLACE(DimensionName, ' ', '')) = LOWER(REPLACE(@DimensionName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Dimension Name already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_Dimension
        SET 
            DimensionName = @DimensionName,
            OrderBy       = @OrderBy,
            UpdatedBy     = @UserID,
            UpdatedDate   = GETDATE(),
            IsActive      = ISNULL(@IsActive, 1)
        WHERE ID = @ID;

        SET @MESSAGE = 'Dimension updated successfully!';
        RETURN;
    END

    ELSE
    BEGIN
        SET @MESSAGE = 'Invalid Action specified!';
        RETURN;
    END
END
GO

-- 1.3 ACTIVE / INACTIVE TOGGLE
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Dimension_ActiveInactive]
    @ID        INT,
    @IsActive  BIT,
    @UpdatedBy INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.MST_Dimension WHERE ID = @ID)
    BEGIN
        SELECT 0 AS [Success], 'Dimension record not found.' AS [Message];
        RETURN;
    END

    UPDATE dbo.MST_Dimension
    SET 
        IsActive    = @IsActive,
        UpdatedDate = GETDATE(),
        UpdatedBy   = @UpdatedBy
    WHERE ID = @ID;

    SELECT 
        1 AS [Success], 
        CASE WHEN @IsActive = 1 THEN 'Dimension activated successfully!' ELSE 'Dimension deactivated successfully!' END AS [Message];
END
GO


-- =========================================================================================
-- 2. FACTORY CODE MASTER (MST_FactoryCode) STORED PROCEDURES
-- =========================================================================================

-- 2.1 GET GRID LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_FactoryCode_GetGridList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        F.[ID],
        ISNULL(RTRIM(F.[FactoryCode]), '') AS [FactoryCode],
        ISNULL(F.[OrderBy], 0)             AS [OrderBy],
        F.[CreatedOn],
        F.[CreatedBy],
        F.[UpdatedOn],
        F.[UpdatedBy]
    FROM [dbo].[MST_FactoryCode] F WITH(NOLOCK)
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = '' 
        OR F.[FactoryCode] LIKE '%' + @SearchText + '%'
    )
    ORDER BY ISNULL(F.[OrderBy], 999), F.[FactoryCode] ASC;
END
GO

-- 2.2 INSERT / UPDATE (INDIVIDUAL PARAMETERS - NO XML)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_FactoryCode_Insert_Update]
    @ACTION      VARCHAR(20)  = 'INSERT',
    @ID          INT          = 0,
    @FactoryCode NVARCHAR(50) = NULL,
    @OrderBy     INT          = NULL,
    @UserID      INT          = 1,
    @MESSAGE     VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @FactoryCode = LTRIM(RTRIM(@FactoryCode));

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@FactoryCode, '') = ''
        BEGIN 
            SET @MESSAGE = 'Factory Code is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_FactoryCode
            WHERE LOWER(REPLACE(RTRIM(FactoryCode), ' ', '')) = LOWER(REPLACE(@FactoryCode, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Factory Code already exists!'; 
            RETURN; 
        END

        IF @OrderBy IS NULL OR @OrderBy = 0
        BEGIN
            SELECT @OrderBy = ISNULL(MAX(OrderBy), 0) + 1 FROM dbo.MST_FactoryCode;
        END

        INSERT INTO dbo.MST_FactoryCode
        (
            FactoryCode, OrderBy, CreatedOn, CreatedBy
        )
        VALUES
        (
            @FactoryCode, @OrderBy, GETDATE(), @UserID
        );

        SET @MESSAGE = 'Factory Code saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@ID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Factory Code ID!'; 
            RETURN; 
        END

        IF ISNULL(@FactoryCode, '') = ''
        BEGIN 
            SET @MESSAGE = 'Factory Code is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_FactoryCode
            WHERE ID <> @ID 
              AND LOWER(REPLACE(RTRIM(FactoryCode), ' ', '')) = LOWER(REPLACE(@FactoryCode, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Factory Code already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_FactoryCode
        SET 
            FactoryCode = @FactoryCode,
            OrderBy     = @OrderBy,
            UpdatedOn   = GETDATE(),
            UpdatedBy   = @UserID
        WHERE ID = @ID;

        SET @MESSAGE = 'Factory Code updated successfully!';
        RETURN;
    END

    ELSE
    BEGIN
        SET @MESSAGE = 'Invalid Action specified!';
        RETURN;
    END
END
GO


-- =========================================================================================
-- 3. SIZE CODE MASTER (MST_SizeCode) STORED PROCEDURES
-- =========================================================================================

-- 3.1 GET GRID LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_SizeCode_GetGridList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        S.[ID],
        ISNULL(S.[SizeName], '')  AS [SizeName],
        ISNULL(S.[ShortName], '') AS [ShortName],
        ISNULL(S.[IsActive], 0)   AS [IsActive],
        S.[CreatedBy],
        S.[CreatedDate],
        S.[UpdatedBy],
        S.[UpdatedDate]
    FROM [dbo].[MST_SizeCode] S WITH(NOLOCK)
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = '' 
        OR S.[SizeName]  LIKE '%' + @SearchText + '%'
        OR S.[ShortName] LIKE '%' + @SearchText + '%'
    )
    ORDER BY S.[ID] ASC;
END
GO

-- 3.2 INSERT / UPDATE (INDIVIDUAL PARAMETERS - NO XML)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_SizeCode_Insert_Update]
    @ACTION    VARCHAR(20)   = 'INSERT',
    @ID        INT           = 0,
    @SizeName  NVARCHAR(50)  = NULL,
    @ShortName NVARCHAR(50)  = NULL,
    @IsActive  BIT           = 1,
    @UserID    INT           = 1,
    @MESSAGE   VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @SizeName = LTRIM(RTRIM(@SizeName));
    SET @ShortName = LTRIM(RTRIM(@ShortName));

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@SizeName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Size Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_SizeCode
            WHERE LOWER(REPLACE(SizeName, ' ', '')) = LOWER(REPLACE(@SizeName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Size Name already exists!'; 
            RETURN; 
        END

        INSERT INTO dbo.MST_SizeCode
        (
            SizeName, ShortName, IsActive, CreatedBy, CreatedDate
        )
        VALUES
        (
            @SizeName, @ShortName, ISNULL(@IsActive, 1), @UserID, GETDATE()
        );

        SET @MESSAGE = 'Size Code saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@ID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Size Code ID!'; 
            RETURN; 
        END

        IF ISNULL(@SizeName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Size Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_SizeCode
            WHERE ID <> @ID 
              AND LOWER(REPLACE(SizeName, ' ', '')) = LOWER(REPLACE(@SizeName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Size Name already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_SizeCode
        SET 
            SizeName    = @SizeName,
            ShortName   = @ShortName,
            IsActive    = ISNULL(@IsActive, 1),
            UpdatedBy   = @UserID,
            UpdatedDate = GETDATE()
        WHERE ID = @ID;

        SET @MESSAGE = 'Size Code updated successfully!';
        RETURN;
    END

    ELSE
    BEGIN
        SET @MESSAGE = 'Invalid Action specified!';
        RETURN;
    END
END
GO

-- 3.3 ACTIVE / INACTIVE TOGGLE
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_SizeCode_ActiveInactive]
    @ID        INT,
    @IsActive  BIT,
    @UpdatedBy INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.MST_SizeCode WHERE ID = @ID)
    BEGIN
        SELECT 0 AS [Success], 'Size Code record not found.' AS [Message];
        RETURN;
    END

    UPDATE dbo.MST_SizeCode
    SET 
        IsActive    = @IsActive,
        UpdatedDate = GETDATE(),
        UpdatedBy   = @UpdatedBy
    WHERE ID = @ID;

    SELECT 
        1 AS [Success], 
        CASE WHEN @IsActive = 1 THEN 'Size Code activated successfully!' ELSE 'Size Code deactivated successfully!' END AS [Message];
END
GO

