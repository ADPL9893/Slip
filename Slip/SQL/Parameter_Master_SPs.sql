USE [SLIP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================================
-- 1. SHAPE MASTER STORED PROCEDURES (NO DELETE)
-- =========================================================================================

-- 1.1 GET SHAPE TYPE LOOKUP LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_ShapeType_GetList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        [ShapeTypeID],
        [ShapeType]
    FROM [dbo].[mst_shapetype] WITH(NOLOCK)
    ORDER BY [ShapeTypeID];
END
GO

-- 1.2 GET SHAPE GROUP LOOKUP LIST (FROM mst_shapefinalgroup)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_ShapeGroup_GetList]
    @ShapeType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        [ID],
        [ShapeGroup],
        [ShapeType],
        [Sequence]
    FROM [dbo].[mst_shapefinalgroup] WITH(NOLOCK)
    WHERE ISNULL([IsActive], 1) = 1
      AND (@ShapeType IS NULL OR @ShapeType = '' OR [ShapeType] = @ShapeType)
    ORDER BY [Sequence], [ShapeGroup];
END
GO

-- 1.3 GET SHAPE LIST (WITH SHAPETYPE NAME JOIN)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Shape_GetList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        S.[ShapeID],
        ISNULL(S.[ShapeName], '')        AS [ShapeName],
        S.[ShapeTypeID],
        ISNULL(ST.[ShapeType], '')       AS [ShapeTypeName],
        ISNULL(S.[Remarks], '')          AS [Remarks],
        S.[Created],
        S.[Modified],
        ISNULL(S.[Sequence], 0)          AS [Sequence],
        ISNULL(S.[ShapeCategory], '')    AS [ShapeCategory],
        S.[UserID],
        S.[UpdateBy],
        ISNULL(S.[ShapeGroup], '')       AS [ShapeGroup],
        ISNULL(S.[ShortName], '')        AS [ShortName],
        S.[MaxNo],
        S.[MaxNoDate],
        ISNULL(S.[Type], '')             AS [Type],
        ISNULL(S.[DisplayType], '')      AS [DisplayType],
        ISNULL(S.[ShapePrdGroup], '')    AS [ShapePrdGroup],
        ISNULL(S.[ShapeFinalGroup], '')  AS [ShapeFinalGroup],
        S.[ShapeIDUserFile]
    FROM dbo.MST_Shape S WITH(NOLOCK)
    LEFT JOIN dbo.mst_shapetype ST WITH(NOLOCK) ON ST.ShapeTypeID = S.ShapeTypeID
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = ''
        OR S.[ShapeName]       LIKE '%' + @SearchText + '%'
        OR S.[ShortName]       LIKE '%' + @SearchText + '%'
        OR S.[ShapeGroup]      LIKE '%' + @SearchText + '%'
        OR S.[ShapeCategory]   LIKE '%' + @SearchText + '%'
        OR S.[ShapeFinalGroup] LIKE '%' + @SearchText + '%'
        OR S.[ShapePrdGroup]   LIKE '%' + @SearchText + '%'
        OR ST.[ShapeType]      LIKE '%' + @SearchText + '%'
    )
    ORDER BY ISNULL(S.[Sequence], 999), S.[ShapeName] ASC;
END
GO


-- 1.2 INSERT / UPDATE (ONLY INSERT & UPDATE)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Shape_Insert_Update]
    @ACTION          VARCHAR(20)   = 'INSERT',
    @ShapeID         INT           = 0,
    @ShapeName       VARCHAR(100)  = NULL,
    @ShortName       VARCHAR(50)   = NULL,
    @ShapeGroup      VARCHAR(100)  = NULL,
    @ShapeCategory   VARCHAR(100)  = NULL,
    @ShapeTypeID     INT           = NULL,
    @Sequence        INT           = NULL,
    @ShapeFinalGroup VARCHAR(100)  = NULL,
    @ShapePrdGroup   VARCHAR(100)  = NULL,
    @Type            VARCHAR(50)   = NULL,
    @DisplayType     VARCHAR(50)   = NULL,
    @Remarks         VARCHAR(250)  = NULL,
    @UserID          INT           = 1,
    @UpdateBy        INT           = 1,
    @ShapeIDUserFile INT           = NULL,
    @MESSAGE         VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @ShapeName = LTRIM(RTRIM(@ShapeName));

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@ShapeName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Shape Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Shape 
            WHERE LOWER(REPLACE(ShapeName, ' ', '')) = LOWER(REPLACE(@ShapeName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Shape Name already exists!'; 
            RETURN; 
        END

        IF @Sequence IS NULL OR @Sequence = 0
        BEGIN
            SELECT @Sequence = ISNULL(MAX(Sequence), 0) + 1 FROM dbo.MST_Shape;
        END

        INSERT INTO dbo.MST_Shape
        (
            ShapeName, ShortName, ShapeGroup, ShapeCategory, ShapeTypeID,
            Sequence, ShapeFinalGroup, ShapePrdGroup, Type, DisplayType,
            Remarks, Created, Modified, UserID, UpdateBy, ShapeIDUserFile
        )
        VALUES
        (
            @ShapeName, @ShortName, @ShapeGroup, @ShapeCategory, @ShapeTypeID,
            @Sequence, @ShapeFinalGroup, @ShapePrdGroup, @Type, @DisplayType,
            @Remarks, GETDATE(), NULL, @UserID, @UpdateBy, @ShapeIDUserFile
        );

        SET @MESSAGE = 'Shape saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@ShapeID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Shape ID!'; 
            RETURN; 
        END

        IF ISNULL(@ShapeName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Shape Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Shape 
            WHERE ShapeID <> @ShapeID 
              AND LOWER(REPLACE(ShapeName, ' ', '')) = LOWER(REPLACE(@ShapeName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Shape Name already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_Shape
        SET 
            ShapeName       = @ShapeName,
            ShortName       = @ShortName,
            ShapeGroup      = @ShapeGroup,
            ShapeCategory   = @ShapeCategory,
            ShapeTypeID     = @ShapeTypeID,
            Sequence        = @Sequence,
            ShapeFinalGroup = @ShapeFinalGroup,
            ShapePrdGroup   = @ShapePrdGroup,
            Type            = @Type,
            DisplayType     = @DisplayType,
            Remarks         = @Remarks,
            Modified        = GETDATE(),
            UpdateBy        = @UpdateBy,
            ShapeIDUserFile = @ShapeIDUserFile
        WHERE ShapeID = @ShapeID;

        SET @MESSAGE = 'Shape updated successfully!';
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
-- 2. COLOR MASTER SCHEMA UPDATE & STORED PROCEDURES (NO DELETE - ONLY INSERT & UPDATE)
-- =========================================================================================

-- 2.0 SCHEMA UPDATE: ADD ColorType COLUMN & UPDATE EXISTING RECORDS
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'dbo' 
      AND TABLE_NAME = 'mst_color' 
      AND COLUMN_NAME = 'ColorType'
)
BEGIN
    ALTER TABLE [dbo].[mst_color] ADD [ColorType] VARCHAR(50) NULL;
END
GO

-- Populate existing ColorType based on ColorTypeID (1 = Round, 2 = Fancy)
UPDATE [dbo].[mst_color]
SET [ColorType] = 'Round'
WHERE [ColorTypeID] = 1 AND ([ColorType] IS NULL OR [ColorType] = '');

UPDATE [dbo].[mst_color]
SET [ColorType] = 'Fancy'
WHERE [ColorTypeID] = 2 AND ([ColorType] IS NULL OR [ColorType] = '');
GO


-- 2.1 GET LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Color_GetList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        C.[ColorID],
        ISNULL(C.[ColorName], '')       AS [ColorName],
        C.[ColorTypeID],
        ISNULL(C.[ColorType], 
            CASE 
                WHEN C.[ColorTypeID] = 1 THEN 'Round'
                WHEN C.[ColorTypeID] = 2 THEN 'Fancy'
                ELSE ''
            END
        )                               AS [ColorType],
        ISNULL(C.[Remarks], '')         AS [Remarks],
        C.[Created],
        C.[Modified],
        ISNULL(C.[Sequence], 0)         AS [Sequence],
        C.[UserID],
        ISNULL(C.[IsActive], 0)         AS [IsActive],
        C.[ColorIDUserFile]
    FROM dbo.MST_Color C WITH(NOLOCK)
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = ''
        OR C.[ColorName] LIKE '%' + @SearchText + '%'
        OR C.[Remarks]   LIKE '%' + @SearchText + '%'
        OR C.[ColorType] LIKE '%' + @SearchText + '%'
    )
    ORDER BY ISNULL(C.[Sequence], 999), C.[ColorName] ASC;
END
GO


-- 2.2 INSERT / UPDATE (ONLY INSERT & UPDATE)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Color_Insert_Update]
    @ACTION          VARCHAR(20)   = 'INSERT',
    @ColorID         INT           = 0,
    @ColorName       VARCHAR(100)  = NULL,
    @ColorTypeID     INT           = NULL,
    @ColorType       VARCHAR(50)   = NULL,
    @Remarks         VARCHAR(250)  = NULL,
    @Sequence        INT           = NULL,
    @UserID          INT           = 1,
    @IsActive        BIT           = 1,
    @ColorIDUserFile INT           = NULL,
    @MESSAGE         VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @ColorName = LTRIM(RTRIM(@ColorName));

    -- Synchronize ColorType & ColorTypeID
    IF (@ColorTypeID IS NULL OR @ColorTypeID = 0)
    BEGIN
        IF UPPER(@ColorType) = 'ROUND' SET @ColorTypeID = 1;
        ELSE IF UPPER(@ColorType) = 'FANCY' SET @ColorTypeID = 2;
    END
    IF (@ColorType IS NULL OR @ColorType = '')
    BEGIN
        IF @ColorTypeID = 1 SET @ColorType = 'Round';
        ELSE IF @ColorTypeID = 2 SET @ColorType = 'Fancy';
    END

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@ColorName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Color Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Color
            WHERE LOWER(REPLACE(ColorName, ' ', '')) = LOWER(REPLACE(@ColorName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Color Name already exists!'; 
            RETURN; 
        END

        IF @Sequence IS NULL OR @Sequence = 0
        BEGIN
            SELECT @Sequence = ISNULL(MAX(Sequence), 0) + 1 FROM dbo.MST_Color;
        END

        INSERT INTO dbo.MST_Color
        (
            ColorName, ColorTypeID, ColorType, Remarks, Created, Modified, 
            Sequence, UserID, IsActive, ColorIDUserFile
        )
        VALUES
        (
            @ColorName, @ColorTypeID, @ColorType, @Remarks, GETDATE(), NULL,
            @Sequence, @UserID, ISNULL(@IsActive, 1), @ColorIDUserFile
        );

        SET @MESSAGE = 'Color saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@ColorID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Color ID!'; 
            RETURN; 
        END

        IF ISNULL(@ColorName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Color Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Color
            WHERE ColorID <> @ColorID 
              AND LOWER(REPLACE(ColorName, ' ', '')) = LOWER(REPLACE(@ColorName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Color Name already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_Color
        SET 
            ColorName       = @ColorName,
            ColorTypeID     = @ColorTypeID,
            ColorType       = @ColorType,
            Remarks         = @Remarks,
            Modified        = GETDATE(),
            Sequence        = @Sequence,
            UserID          = @UserID,
            IsActive        = ISNULL(@IsActive, 1),
            ColorIDUserFile = @ColorIDUserFile
        WHERE ColorID = @ColorID;

        SET @MESSAGE = 'Color updated successfully!';
        RETURN;
    END

    ELSE
    BEGIN
        SET @MESSAGE = 'Invalid Action specified!';
        RETURN;
    END
END
GO


-- 2.3 ACTIVE / INACTIVE TOGGLE (ONLY FOR COLOR)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Color_ActiveInactive]
    @ColorID   INT,
    @IsActive  BIT,
    @UpdatedBy INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.MST_Color WHERE ColorID = @ColorID)
    BEGIN
        SELECT 0 AS [Success], 'Color record not found.' AS [Message];
        RETURN;
    END

    UPDATE dbo.MST_Color
    SET 
        IsActive = @IsActive,
        Modified = GETDATE(),
        UserID   = @UpdatedBy
    WHERE ColorID = @ColorID;

    SELECT 
        1 AS [Success], 
        CASE WHEN @IsActive = 1 THEN 'Color activated successfully!' ELSE 'Color deactivated successfully!' END AS [Message];
END
GO


-- =========================================================================================
-- 3. CLARITY MASTER STORED PROCEDURES (NO DELETE - ONLY INSERT & UPDATE)
-- =========================================================================================

-- 3.1 GET LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Clarity_GetList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        CL.[ClarityID],
        ISNULL(CL.[ClarityName], '')      AS [ClarityName],
        ISNULL(CL.[SubClarity], '')       AS [SubClarity],
        ISNULL(CL.[Sequence], 0)          AS [Sequence],
        CL.[ClarityIDUserFile],
        ISNULL(CL.[Remarks], '')          AS [Remarks],
        CL.[Created],
        CL.[Modified]
    FROM dbo.MST_Clarity CL WITH(NOLOCK)
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = ''
        OR CL.[ClarityName] LIKE '%' + @SearchText + '%'
        OR CL.[SubClarity]   LIKE '%' + @SearchText + '%'
        OR CL.[Remarks]      LIKE '%' + @SearchText + '%'
    )
    ORDER BY ISNULL(CL.[Sequence], 999), CL.[ClarityName] ASC;
END
GO


-- 3.2 INSERT / UPDATE (ONLY INSERT & UPDATE)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Clarity_Insert_Update]
    @ACTION            VARCHAR(20)   = 'INSERT',
    @ClarityID         INT           = 0,
    @ClarityName       VARCHAR(100)  = NULL,
    @SubClarity        VARCHAR(50)   = NULL,
    @Sequence          INT           = NULL,
    @Remarks           VARCHAR(250)  = NULL,
    @UserID            INT           = 1,
    @ClarityIDUserFile INT           = NULL,
    @MESSAGE           VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @ClarityName = LTRIM(RTRIM(@ClarityName));

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@ClarityName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Clarity Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Clarity
            WHERE LOWER(REPLACE(ClarityName, ' ', '')) = LOWER(REPLACE(@ClarityName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Clarity Name already exists!'; 
            RETURN; 
        END

        IF @Sequence IS NULL OR @Sequence = 0
        BEGIN
            SELECT @Sequence = ISNULL(MAX(Sequence), 0) + 1 FROM dbo.MST_Clarity;
        END

        INSERT INTO dbo.MST_Clarity
        (
            ClarityName, SubClarity, Sequence, ClarityIDUserFile, 
            Remarks, Created, Modified
        )
        VALUES
        (
            @ClarityName, @SubClarity, @Sequence, @ClarityIDUserFile,
            @Remarks, GETDATE(), NULL
        );

        SET @MESSAGE = 'Clarity saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@ClarityID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Clarity ID!'; 
            RETURN; 
        END

        IF ISNULL(@ClarityName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Clarity Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Clarity
            WHERE ClarityID <> @ClarityID 
              AND LOWER(REPLACE(ClarityName, ' ', '')) = LOWER(REPLACE(@ClarityName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Clarity Name already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_Clarity
        SET 
            ClarityName       = @ClarityName,
            SubClarity        = @SubClarity,
            Sequence          = @Sequence,
            ClarityIDUserFile = @ClarityIDUserFile,
            Remarks           = @Remarks,
            Modified          = GETDATE()
        WHERE ClarityID = @ClarityID;

        SET @MESSAGE = 'Clarity updated successfully!';
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
-- 4. CUT MASTER STORED PROCEDURES (NO DELETE - ONLY INSERT & UPDATE)
-- =========================================================================================

-- 4.1 GET LIST
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Cut_GetList]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @SearchText = LTRIM(RTRIM(@SearchText));

    SELECT 
        CT.[CutID],
        ISNULL(CT.[CutName], '')   AS [CutName],
        ISNULL(CT.[CutGroup], '')  AS [CutGroup],
        ISNULL(CT.[Sequence], 0)   AS [Sequence],
        ISNULL(CT.[Remarks], '')   AS [Remarks],
        CT.[Created],
        CT.[Modified]
    FROM dbo.MST_Cut CT WITH(NOLOCK)
    WHERE (
        @SearchText IS NULL 
        OR @SearchText = ''
        OR CT.[CutName]  LIKE '%' + @SearchText + '%'
        OR CT.[CutGroup] LIKE '%' + @SearchText + '%'
        OR CT.[Remarks]  LIKE '%' + @SearchText + '%'
    )
    ORDER BY ISNULL(CT.[Sequence], 999), CT.[CutName] ASC;
END
GO


-- 4.2 INSERT / UPDATE (ONLY INSERT & UPDATE)
CREATE OR ALTER PROCEDURE [dbo].[USP_MST_Cut_Insert_Update]
    @ACTION   VARCHAR(20)   = 'INSERT',
    @CutID    INT           = 0,
    @CutName  VARCHAR(100)  = NULL,
    @CutGroup VARCHAR(100)  = NULL,
    @Sequence INT           = NULL,
    @Remarks  VARCHAR(250)  = NULL,
    @UserID   INT           = 1,
    @MESSAGE  VARCHAR(1000) = '' OUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @MESSAGE = 'ERROR';

    SET @ACTION = UPPER(LTRIM(RTRIM(ISNULL(@ACTION, 'INSERT'))));
    SET @CutName = LTRIM(RTRIM(@CutName));

    -- =============================================================
    -- INSERT
    -- =============================================================
    IF @ACTION = 'INSERT'
    BEGIN
        IF ISNULL(@CutName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Cut Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Cut
            WHERE LOWER(REPLACE(CutName, ' ', '')) = LOWER(REPLACE(@CutName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Cut Name already exists!'; 
            RETURN; 
        END

        IF @Sequence IS NULL OR @Sequence = 0
        BEGIN
            SELECT @Sequence = ISNULL(MAX(Sequence), 0) + 1 FROM dbo.MST_Cut;
        END

        INSERT INTO dbo.MST_Cut
        (
            CutName, CutGroup, Sequence, Remarks, Created, Modified
        )
        VALUES
        (
            @CutName, @CutGroup, @Sequence, @Remarks, GETDATE(), NULL
        );

        SET @MESSAGE = 'Cut saved successfully!';
        RETURN;
    END

    -- =============================================================
    -- UPDATE
    -- =============================================================
    ELSE IF @ACTION = 'UPDATE'
    BEGIN
        IF ISNULL(@CutID, 0) = 0
        BEGIN 
            SET @MESSAGE = 'Invalid Cut ID!'; 
            RETURN; 
        END

        IF ISNULL(@CutName, '') = ''
        BEGIN 
            SET @MESSAGE = 'Cut Name is required!'; 
            RETURN; 
        END

        IF EXISTS (
            SELECT 1 FROM dbo.MST_Cut
            WHERE CutID <> @CutID 
              AND LOWER(REPLACE(CutName, ' ', '')) = LOWER(REPLACE(@CutName, ' ', ''))
        )
        BEGIN 
            SET @MESSAGE = 'Cut Name already exists!'; 
            RETURN; 
        END

        UPDATE dbo.MST_Cut
        SET 
            CutName  = @CutName,
            CutGroup = @CutGroup,
            Sequence = @Sequence,
            Remarks  = @Remarks,
            Modified = GETDATE()
        WHERE CutID = @CutID;

        SET @MESSAGE = 'Cut updated successfully!';
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
-- 5. ENABLE PARAMETER MASTER IN SYS_MODULE
-- =========================================================================================
IF EXISTS (SELECT 1 FROM dbo.SYS_Module WHERE ModuleName LIKE '%Parameter%')
BEGIN
    UPDATE dbo.SYS_Module
    SET 
        StrictlyStop = 0,
        Controller   = 'Master',
        [Action]     = 'ParameterMaster',
        [URL]        = 'Master\ParameterMaster'
    WHERE ModuleName LIKE '%Parameter%';
END
GO
