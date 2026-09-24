CREATE PROCEDURE [dbo].[DMX_StockAndSalesReport_Export]
    @ReportDate VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #TmpStock (
        Weight DECIMAL(18,3),
        Color VARCHAR(50),
        Clarity VARCHAR(50),
        Location VARCHAR(100),
        StockPcs INT,
        SalePcs INT
    )

    -- Insert Available Stock
    IF @ReportDate IS NULL
    BEGIN
        INSERT INTO #TmpStock (Weight, Color, Clarity, Location, StockPcs, SalePcs)
        SELECT Weight, Color, Clarity,
               CASE WHEN Location = 'NY' OR Location = 'NEWYORK' THEN 'NY' WHEN Location = 'SURAT' THEN 'SURAT' ELSE 'MUMBAI' END,
               1, 0
        FROM DMX_AvailableStock
        WHERE IsActive = 1
    END
    ELSE
    BEGIN
        INSERT INTO #TmpStock (Weight, Color, Clarity, Location, StockPcs, SalePcs)
        SELECT Weight, Color, Clarity,
               CASE WHEN Location = 'NY' OR Location = 'NEWYORK' THEN 'NY' WHEN Location = 'SURAT' THEN 'SURAT' ELSE 'MUMBAI' END,
               1, 0
        FROM DMX_AvailableStock
        WHERE IsActive = 1 AND CAST(Created AS DATE) = CAST(@ReportDate AS DATE)
    END

    -- Insert Sales Stock
    IF @ReportDate IS NULL
    BEGIN
        INSERT INTO #TmpStock (Weight, Color, Clarity, Location, StockPcs, SalePcs)
        SELECT Weight, Color, Clarity,
               CASE WHEN Location = 'NY' OR Location = 'NEWYORK' THEN 'NY' WHEN Location = 'SURAT' THEN 'SURAT' ELSE 'MUMBAI' END,
               0, ISNULL(PacketPcs, 1)
        FROM DMX_SALESStock
        WHERE IsActive = 1
    END
    ELSE
    BEGIN
        INSERT INTO #TmpStock (Weight, Color, Clarity, Location, StockPcs, SalePcs)
        SELECT Weight, Color, Clarity,
               CASE WHEN Location = 'NY' OR Location = 'NEWYORK' THEN 'NY' WHEN Location = 'SURAT' THEN 'SURAT' ELSE 'MUMBAI' END,
               0, ISNULL(PacketPcs, 1)
        FROM DMX_SALESStock
        WHERE IsActive = 1 AND (CAST(EntryDate AS DATE) = CAST(@ReportDate AS DATE) OR TRY_CONVERT(DATE, Trans_Date, 103) = CAST(@ReportDate AS DATE) OR CAST(Created AS DATE) = CAST(@ReportDate AS DATE))
    END

    -- Group and Pivot data
    SELECT 
        t.Location,
        s.SizeBucket,
        t.Color,
        t.Clarity,
        SUM(t.StockPcs) AS StockPcs,
        SUM(t.SalePcs) AS SalePcs
    FROM #TmpStock t
    INNER JOIN MST_Size_DMX m ON m.IsActive = 1 
                         AND t.Weight >= m.FromSize AND t.Weight <= m.ToSize
    CROSS APPLY (
        SELECT CONVERT(VARCHAR, CAST(m.FromSize AS DECIMAL(18,2))) + '-' + CONVERT(VARCHAR, CAST(ROUND(m.ToSize, 2, 1) AS DECIMAL(18,2))) AS SizeBucket
    ) s
    WHERE t.Color IS NOT NULL AND t.Color <> ''
      AND t.Clarity IS NOT NULL AND t.Clarity <> ''
    GROUP BY t.Location, s.SizeBucket, t.Color, t.Clarity
    ORDER BY t.Location, MIN(t.Weight), t.Color, t.Clarity

    DROP TABLE #TmpStock
END
