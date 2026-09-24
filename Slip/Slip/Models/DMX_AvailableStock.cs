using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class DMX_AvailableStock
    {
        public int ID { get; set; }
        public string Stone_NO { get; set; }
        public string Shape { get; set; }
        public string Weight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symm { get; set; }
        public string FlrIntens { get; set; }
        public string FlrColor { get; set; }
        public string FColor { get; set; }
        public string FCIntens { get; set; }
        public string FCOverton { get; set; }
        public string Measurement { get; set; }
        public string Diameter_Min { get; set; }
        public string Diameter_Max { get; set; }
        public string Total_Depth { get; set; }
        public string Lab { get; set; }
        public string LabLocation { get; set; }
        public string Lab_Report_No { get; set; }
        public string LabLink { get; set; }
        public string Lab_Report_Date { get; set; }
        public string Laser_Inscription { get; set; }
        public string Treatment { get; set; }
        public string Location { get; set; }
        public string LiveRAP { get; set; }
        public string SaleDis { get; set; }
        public string SaleRate { get; set; }
        public string SaleAmt { get; set; }
        public string Total_Depth_Per { get; set; }
        public string Table_Diameter_Per { get; set; }
        public string GirdleName { get; set; }
        public string GirdleThin_ID { get; set; }
        public string GirdleThick_ID { get; set; }
        public string Girdle_Per { get; set; }
        public string GirdleCon { get; set; }
        public string CuletSize { get; set; }
        public string CuletCon { get; set; }
        public string CrownHeight { get; set; }
        public string CrownAngle { get; set; }
        public string PavillionHeight { get; set; }
        public string PavillionAngle { get; set; }
        public string PairStock_No { get; set; }
        public string Parcels_Stone { get; set; }
        public string Comment { get; set; }
        public string KeyToSymbols { get; set; }
        public string Shade { get; set; }
        public string StarLength { get; set; }
        public string LowerHalve { get; set; }
        public string Table_Inclusion { get; set; }
        public string Black_Inclusion { get; set; }
        public string Side_Inclusion { get; set; }
        public string Open_Inclusion { get; set; }
        public string Brand { get; set; }
        public string HnA { get; set; }
        public string Ratio { get; set; }
        public string Eyeclean { get; set; }
        public string Tinge { get; set; }
        public string Feather_Inclusion { get; set; }
        public string Milkey { get; set; }
        public string Luster { get; set; }
        public string BIS { get; set; }
        public string BIC { get; set; }
        public string WIS { get; set; }
        public string WIC { get; set; }
        public string Internal_Graining { get; set; }
        public string Surface_Graining { get; set; }
        public string Natural_Type { get; set; }
        public string ExtraFacet { get; set; }
        public string Fancy_Color_Description { get; set; }
        public string Stone_Comment { get; set; }
        public string NewArrival { get; set; }
        public string StockStatus { get; set; }
        public string Is_VedioExist { get; set; }
        public string Is_imgExist { get; set; }
        public string Is_CertyExist { get; set; }
        public string Certificate_file_url { get; set; }
        public string Stone_Img_url { get; set; }
        public string Video_url { get; set; }
        public string Lab_Report_Comment { get; set; }
        public string Remarks { get; set; }
        public string Control_No { get; set; }
        public string CVD_HPHT { get; set; }
        public string Diamond_Type { get; set; }
        public string Rough_Origin { get; set; }
        public string COP { get; set; }
        public string Lab_Assortment_Result { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }

    public class DiamxApiResponse
    {
        public string ApiStatus { get; set; }
        public List<DMX_AvailableStock> StoneList { get; set; }
    }

    public class DmxStockSummary_Size
    {
        public string SizeGroup { get; set; }
        public int Pcs { get; set; }
        public decimal PcsPer { get; set; }
        public decimal Weight { get; set; }
        public decimal WeightPer { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalAmtPer { get; set; }
        public decimal PerCt { get; set; }
    }

    public class DmxStockSummary_Color
    {
        public string Color { get; set; }
        public int Pcs { get; set; }
        public decimal PcsPer { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalAmtPer { get; set; }
    }

    public class DmxStockSummary_Clarity
    {
        public string Clarity { get; set; }
        public int Pcs { get; set; }
        public decimal PcsPer { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalAmtPer { get; set; }
    }

    public class DmxStockSummary_Pivot
    {
        public string SizeBucket { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public int Pcs { get; set; }
        public decimal PcsPer { get; set; }
        public decimal Weight { get; set; }
        public decimal WeightPer { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalAmtPer { get; set; }
    }

    public class DMX_HOLDStock
    {
        public int ID { get; set; }
        public string Stone_NO { get; set; }
        public string Shape { get; set; }
        public decimal? Weight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symm { get; set; }
        public string FlrIntens { get; set; }
        public string FlrColor { get; set; }
        public string FColor { get; set; }
        public string FCIntens { get; set; }
        public string FCOverton { get; set; }
        public string Lab { get; set; }
        public string Lab_Report_No { get; set; }
        public string Location { get; set; }
        public decimal? LiveRAP { get; set; }
        public decimal? SaleDis { get; set; }
        public decimal? SaleRate { get; set; }
        public decimal? SaleAmt { get; set; }
        public int? Trans_No { get; set; }
        public string Trans_Date { get; set; }
        public int? Trans_Due_Day { get; set; }
        public string Trans_Due_Date { get; set; }
        public decimal? Trans_Rap { get; set; }
        public decimal? Trans_Dis { get; set; }
        public decimal? Trans_Rate { get; set; }
        public decimal? Trans_Amt { get; set; }
        public string NewArrival { get; set; }
        public string StockStatus { get; set; }
        public string CVD_HPHT { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Seller_Name { get; set; }
        public string Branch { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string Party { get; set; }
        public string PartyCountry { get; set; }
        public decimal? ExRate { get; set; }
        public int? PacketPcs { get; set; }
    }

    public class DiamxHoldApiResponse
    {
        public string ApiStatus { get; set; }
        public List<DMX_HOLDStock> Transaction_List { get; set; }
    }

    public class DMX_MEMOStock
    {
        public int ID { get; set; }
        public string Stone_NO { get; set; }
        public string Shape { get; set; }
        public decimal? Weight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symm { get; set; }
        public string FlrIntens { get; set; }
        public string FlrColor { get; set; }
        public string FColor { get; set; }
        public string FCIntens { get; set; }
        public string FCOverton { get; set; }
        public string Lab { get; set; }
        public string Lab_Report_No { get; set; }
        public string Location { get; set; }
        public decimal? LiveRAP { get; set; }
        public decimal? SaleDis { get; set; }
        public decimal? SaleRate { get; set; }
        public decimal? SaleAmt { get; set; }
        public string Trans_No { get; set; }
        public string Trans_Date { get; set; }
        public int? Trans_Due_Day { get; set; }
        public string Trans_Due_Date { get; set; }
        public decimal? Trans_Rap { get; set; }
        public decimal? Trans_Dis { get; set; }
        public decimal? Trans_Rate { get; set; }
        public decimal? Trans_Amt { get; set; }
        public string NewArrival { get; set; }
        public string StockStatus { get; set; }
        public string CVD_HPHT { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Seller_Name { get; set; }
        public string Branch { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string Party { get; set; }
        public string PartyCountry { get; set; }
        public decimal? ExRate { get; set; }
        public int? PacketPcs { get; set; }
    }

    public class DiamxMemoApiResponse
    {
        public string ApiStatus { get; set; }
        public List<DMX_MEMOStock> Transaction_List { get; set; }
    }

    public class DmxMemoHoldSummaryItem
    {
        public string Status { get; set; }
        public int PacketPcs { get; set; }
        public decimal Weight { get; set; }
        public decimal SaleAmt { get; set; }
        public string Branch { get; set; }
        public string Sellar { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? Trans_Due_Date { get; set; }
    }

    public class DMX_SALESStock
    {
        public int ID { get; set; }
        public string Stone_NO { get; set; }
        public string Shape { get; set; }
        public decimal? Weight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public string Cut { get; set; }
        public string Polish { get; set; }
        public string Symm { get; set; }
        public string FlrIntens { get; set; }
        public string FlrColor { get; set; }
        public string FColor { get; set; }
        public string FCIntens { get; set; }
        public string FCOverton { get; set; }
        public string Lab { get; set; }
        public string Lab_Report_No { get; set; }
        public string Location { get; set; }
        public decimal? LiveRAP { get; set; }
        public decimal? SaleDis { get; set; }
        public decimal? SaleRate { get; set; }
        public decimal? SaleAmt { get; set; }
        public string Trans_No { get; set; }
        public string Trans_Date { get; set; }
        public int? Trans_Due_Day { get; set; }
        public string Trans_Due_Date { get; set; }
        public decimal? Trans_Rap { get; set; }
        public decimal? Trans_Dis { get; set; }
        public decimal? Trans_Rate { get; set; }
        public decimal? Trans_Amt { get; set; }
        public string NewArrival { get; set; }
        public string StockStatus { get; set; }
        public string CVD_HPHT { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Seller_Name { get; set; }
        public string Branch { get; set; }
        public string EntryBy { get; set; }
        public string EntryDate { get; set; }
        public string Party { get; set; }
        public string PartyCountry { get; set; }
        public decimal? ExRate { get; set; }
        public int? PacketPcs { get; set; }
        public string BillType { get; set; }
        public decimal? INRRate { get; set; }
        public decimal? INRAmt { get; set; }
        public decimal? FinalRate { get; set; }
        public decimal? CRate { get; set; }
        public decimal? Tax_Amt { get; set; }
        public decimal? UsdRate { get; set; }
        public string Purchase_Party { get; set; }
        public string Purchase_No { get; set; }
        public decimal? Purchase_Rate { get; set; }
        public string Purchaser_By { get; set; }
    }

    public class DiamxSalesApiResponse
    {
        public string ApiStatus { get; set; }
        public List<DMX_SALESStock> Transaction_List { get; set; }
    }

    public class SalesSummaryReportItem
    {
        public decimal Weight { get; set; }
        public string Color { get; set; }
        public string Clarity { get; set; }
        public decimal Trans_Amt { get; set; }
    }

    public class SizeMasterItem
    {
        public decimal FromSize { get; set; }
        public decimal ToSize { get; set; }
        public int Sequence { get; set; }
    }

    public class SalesSellerWiseReportItem
    {
        public string Seller_Name { get; set; }
        public string Branch { get; set; }
        public int Mum_SAP_Pcs { get; set; }
        public decimal Mum_SAP_Weight { get; set; }
        public decimal Mum_SAP_Amt { get; set; }
        public int Mum_Sale_Pcs { get; set; }
        public decimal Mum_Sale_Weight { get; set; }
        public decimal Mum_Sale_Amt { get; set; }
        public int NY_SAP_Pcs { get; set; }
        public decimal NY_SAP_Weight { get; set; }
        public decimal NY_SAP_Amt { get; set; }
        public int NY_Sale_Pcs { get; set; }
        public decimal NY_Sale_Weight { get; set; }
        public decimal NY_Sale_Amt { get; set; }
        public int Sur_SAP_Pcs { get; set; }
        public decimal Sur_SAP_Weight { get; set; }
        public decimal Sur_SAP_Amt { get; set; }
        public int Sur_Sale_Pcs { get; set; }
        public decimal Sur_Sale_Weight { get; set; }
        public decimal Sur_Sale_Amt { get; set; }
        public int Inc_Transfer_Pcs { get; set; }
        public decimal Inc_Transfer_Weight { get; set; }
        public decimal Inc_Transfer_Amt { get; set; }
    }
}




