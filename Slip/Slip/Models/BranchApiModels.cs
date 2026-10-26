using Newtonsoft.Json;
using System;

namespace Slip.Models
{
    // Mirrors the external Branch API's RoughInsertRequest (POST /api/Rough on
    // AnjaliLabTech.API, https://localhost:7008/swagger). Field names/casing
    // must match the API's JSON exactly - JsonProperty pins that independent of
    // C# naming conventions here.
    public class BranchRoughSyncRequest
    {
        [JsonProperty("rCode")] public string RCode { get; set; }
        [JsonProperty("rName")] public string RName { get; set; }
        [JsonProperty("pcs")] public int? Pcs { get; set; }
        [JsonProperty("carat")] public double? Carat { get; set; }
        [JsonProperty("party")] public string Party { get; set; }
        [JsonProperty("rejPcs")] public int? RejPcs { get; set; }
        [JsonProperty("rejCarat")] public double? RejCarat { get; set; }
        [JsonProperty("rejDate")] public DateTime? RejDate { get; set; }
        [JsonProperty("rate")] public double? Rate { get; set; }
        [JsonProperty("isComplete")] public bool? IsComplete { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
        [JsonProperty("originalCarat")] public double? OriginalCarat { get; set; }
        [JsonProperty("dclWeight")] public double? DclWeight { get; set; }
        [JsonProperty("chabka")] public double? Chabka { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
        [JsonProperty("weightLoss")] public double? WeightLoss { get; set; }
        [JsonProperty("sopcs")] public int? Sopcs { get; set; }
        [JsonProperty("socarat")] public double? Socarat { get; set; }
        [JsonProperty("machineNo")] public string MachineNo { get; set; }
        [JsonProperty("hours")] public double? Hours { get; set; }
        [JsonProperty("height")] public double? Height { get; set; }
        [JsonProperty("size")] public string Size { get; set; }
        [JsonProperty("colorType")] public string ColorType { get; set; }
        [JsonProperty("factoryCode")] public string FactoryCode { get; set; }
        [JsonProperty("nakhWeight")] public double? NakhWeight { get; set; }
        [JsonProperty("roughTopsWeight")] public double? RoughTopsWeight { get; set; }
        [JsonProperty("pattaRejPcs")] public int? PattaRejPcs { get; set; }
        [JsonProperty("pattaRejWt")] public double? PattaRejWt { get; set; }
        [JsonProperty("lotCount")] public int? LotCount { get; set; }
        [JsonProperty("sizeCodes")] public string SizeCodes { get; set; }
        [JsonProperty("soremarks")] public string Soremarks { get; set; }
        [JsonProperty("bagId")] public int? BagId { get; set; }
        [JsonProperty("heightName")] public string HeightName { get; set; }
        [JsonProperty("receipe")] public string Receipe { get; set; }
        [JsonProperty("growthRate")] public string GrowthRate { get; set; }
        [JsonProperty("roughType")] public string RoughType { get; set; }
    }

    public class BranchLoginRequest
    {
        [JsonProperty("userName")] public string UserName { get; set; }
        [JsonProperty("password")] public string Password { get; set; }
    }

    public class BranchLoginResponse
    {
        [JsonProperty("token")] public string Token { get; set; }
        [JsonProperty("expiresAt")] public DateTime ExpiresAt { get; set; }
        [JsonProperty("userId")] public int UserId { get; set; }
        [JsonProperty("userName")] public string UserName { get; set; }
        [JsonProperty("isAdmin")] public bool IsAdmin { get; set; }
        [JsonProperty("roleName")] public string RoleName { get; set; }
    }
}
