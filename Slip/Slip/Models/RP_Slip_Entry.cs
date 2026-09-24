using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class RP_Slip_Entry
    {
        public int ID { get; set; }
        public string ReportDate { get; set; }
        public string RCode { get; set; }
        public int RCodePcs { get; set; }
        public string TableNo { get; set; }
        public int BoilWeak { get; set; }
        public int OverDetaction { get; set; }
        public int MISSDetection { get; set; }
        public int INCDetection { get; set; }
        public int ScanningTotal { get; set; }
        public int QC_1 { get; set; }
        public int QC_2 { get; set; }
        public int QC_3 { get; set; }
        public int NoLayer { get; set; }
        public int NoLayer2 { get; set; }
        public int NOTSee { get; set; }
        public int QCTotal1 { get; set; }
        public int Cloud { get; set; }
        public int Crackl3 { get; set; }
        public int QCTotal2 { get; set; }
        public decimal LayerMM { get; set; }
        public decimal LayerMMTo { get; set; }
        public string Remarks { get; set; }
        public string JangadNo { get; set; }
        public string BrakePcs { get; set; }

        public string Name { get; set; }
        public string Shift { get; set; }
        public int OriginalPcs { get; set; }
        public int MakeblePcs { get; set; }
        public int LMakeblePcs { get; set; }
        public int Extract { get; set; }
        public int MinusPcs { get; set; }
        public int LSPcs { get; set; }
        public int MarkingPcs { get; set; }
        public int RemapPcs { get; set; }
        public int CrossSawingPcs { get; set; }
        public int Total { get; set; }

        public int ReCutPcs { get; set; }
        public int PIEPcs { get; set; }
        public int TopsPcs { get; set; }
        public int DamagePcs { get; set; }

        public int RoundPcs { get; set; }
        public int FancyPcs { get; set; }
        public int ShapeTotalPcs { get; set; }
        public int BreakPcs { get; set; }
        public int SarinWeak { get; set; }
        public int WrongMarkingPcs { get; set; }
        public int TotalPcs { get; set; }

        public int ROKPcs { get; set; }
        public decimal ROKPer { get; set; }

        public int RTBLKhadPcs { get; set; }
        public decimal RTBLKhadPer { get; set; }
        public int RTwoInOnePcs { get; set; }
        public decimal RTwoInOnePer { get; set; }
        public int RReFourpPcs { get; set; }
        public decimal RReFourpPer { get; set; }
        public int RReLSPcs { get; set; }
        public decimal RReLSPer { get; set; }
        public int RPattaPcs { get; set; }
        public decimal RPattaPer { get; set; }
        public int RBLOCPcs { get; set; }
        public decimal RBLOCPer { get; set; }
        public int ROnePcs { get; set; }
        public decimal ROnePer { get; set; }
        public int RTwoPcs { get; set; }
        public decimal RTwoPer { get; set; }
        public int RLSPel { get; set; }
        public decimal RLSPelPer { get; set; }
        public int RPIEGhisi { get; set; }
        public decimal RPIEGhisiPer { get; set; }
        public int RDLC { get; set; }
        public decimal RDLCPer { get; set; }
        public int RPcsProblem { get; set; }
        public decimal RPcsProblemPer { get; set; }
        public int RTotal { get; set; }
        public int FOKPcs { get; set; }
        public decimal FOKPer { get; set; }
        public int FTBLKhadPcs { get; set; }
        public decimal FTBLKhadPer { get; set; }
        public int FTwoInOnePcs { get; set; }
        public decimal FTwoInOnePer { get; set; }
        public int FReFourpPcs { get; set; }
        public decimal FReFourpPer { get; set; }
        public int FReLSPcs { get; set; }
        public decimal FReLSPer { get; set; }
        public int FPattaPcs { get; set; }
        public decimal FPattaPer { get; set; }
        public int FBLOCPcs { get; set; }
        public decimal FBLOCPer { get; set; }
        public int FOnePcs { get; set; }
        public decimal FOnePer { get; set; }
        public int FTwoPcs { get; set; }
        public decimal FTwoPer { get; set; }
        public int FLSPel { get; set; }
        public decimal FLSPelPer { get; set; }
        public int FPIEGhisi { get; set; }
        public decimal FPIEGhisiPer { get; set; }
        public int FDLC { get; set; }
        public decimal FDLCPer { get; set; }
        public int FPcsProblem { get; set; }
        public decimal FPcsProblemPer { get; set; }
        public int FTotal { get; set; }


        public int FDTotalPcs { get; set; }

        public int FDPattaPcs { get; set; }
        public decimal FDPer { get; set; }
        public int FUTotalPcs { get; set; }
        public int FUPattaPcs { get; set; }
        public decimal FUPer { get; set; }
        public int CTUTotalPcs { get; set; }
        public int CTUPattaPcs { get; set; }
        public decimal CTUPer { get; set; }
        public int RTotalPcs { get; set; }
        public int RTotalPattaPcs { get; set; }
        public decimal RTotalPer { get; set; }
        public decimal Manual { get; set; }
        public int OVLTotalPcs { get; set; }
        public int OVLPattaPcs { get; set; }
        public decimal OVLPer { get; set; }
        public int MQTotalPcs { get; set; }
        public int MQLPattaPcs { get; set; }
        public decimal MQLPer { get; set; }
        public int PRTotalPcs { get; set; }
        public int PRLPattaPcs { get; set; }
        public decimal PRLPer { get; set; }
        public int FTotalPcs { get; set; }
        public int FTotalPattaPcs { get; set; }
        public decimal FPer { get; set; }
        public decimal FManual { get; set; }

        public int OTotalPcs { get; set; }
        public int Ook { get; set; }
        public int Opav { get; set; }
        public int Ocrn { get; set; }
        public int OCuletRough { get; set; }
        public int OPOverCut { get; set; }
        public int OCOverCut { get; set; }
        public int ORoughSkin { get; set; }
        public int OOP { get; set; }
        public int Ouncomplete { get; set; }
        public int ORoughSkinC {get;set;}
        public int ORoughSkinP {get;set;}
        public int OStoneRemove {get;set;}
        public int OStoneMove {get;set;}
        public int OOvercut {get;set;}
        public int OHardStone {get;set;}
        public int OHeightDown {get;set;}
        public int OOverHeight {get;set;}
        public int OMachineProblem {get;set;}
        public int OCrownRough { get;set;}     
        public int OTotalRepair { get; set; }
        public decimal ORepairPer { get; set; }
        public int OBrk { get; set; }

        public int MQok { get; set; }
        public int MQpav { get; set; }
        public int MQcrn { get; set; }
        public int MQCuletRough { get; set; }
        public int MQPOverCut { get; set; }
        public int MQCOverCut { get; set; }
        public int MQRoughSkin { get; set; }
        public int MQOP { get; set; }
        public int MQuncomplete { get; set; }
        public int MQRoughSkinC { get; set; }
        public int MQRoughSkinP { get; set; }
        public int MQStoneRemove { get; set; }
        public int MQStoneMove { get; set; }
        public int MQOvercut { get; set; }
        public int MQHardStone { get; set; }
        public int MQHeightDown { get; set; }
        public int MQOverHeight { get; set; }
        public int MQMachineProblem { get; set; }
        public int MQCrownRough { get; set; }
        public int MQTotalRepair { get; set; }
        public decimal MQRepairPer { get; set; }
        public int MQBrk { get; set; }


        public decimal FTotalPer { get; set; }

        public int EDTotalPcs { get; set; }
        public int EDok { get; set; }
        public int EDpav { get; set; }
        public int EDcrn { get; set; }
        public int EDCuletRough { get; set; }
        public int EDPOverCut { get; set; }
        public int EDCOverCut { get; set; }
        public int EDRoughSkin { get; set; }
        public int EDOP { get; set; }
        public int EDuncomplete { get; set; }
        public int EDRoughSkinC { get; set; }
        public int EDRoughSkinP { get; set; }
        public int EDStoneRemove { get; set; }
        public int EDStoneMove { get; set; }
        public int EDOvercut { get; set; }
        public int EDHardStone { get; set; }
        public int EDHeightDown { get; set; }
        public int EDOverHeight { get; set; }
        public int EDMachineProblem { get; set; }
        public int EDCrownRough { get; set; }
        public int EDTotalRepair { get; set; }
        public decimal EDRepairPer { get; set; }
        public int EDBrk { get; set; }


        public int EUTotalPcs { get; set; }
        public int EUok { get; set; }
        public int EUpav { get; set; }
        public int EUcrn { get; set; }
        public int EUCuletRough { get; set; }
        public int EUPOverCut { get; set; }
        public int EUCOverCut { get; set; }
        public int EURoughSkin { get; set; }
        public int EUOP { get; set; }
        public int EUuncomplete { get; set; }
        public int EURoughSkinC { get; set; }
        public int EURoughSkinP { get; set; }
        public int EUStoneRemove { get; set; }
        public int EUStoneMove { get; set; }
        public int EUOvercut { get; set; }
        public int EUHardStone { get; set; }
        public int EUHeightDown { get; set; }
        public int EUOverHeight { get; set; }
        public int EUMachineProblem { get; set; }
        public int EUCrownRough { get; set; }
        public int EUTotalRepair { get; set; }
        public decimal EURepairPer { get; set; }
        public int EUBrk { get; set; }


        public int OCTUTotalPcs { get; set; }
        public int OCTUok { get; set; }
        public int OCTUpav { get; set; }
        public int OCTUcrn { get; set; }
        public int OCTUCuletRough { get; set; }
        public int OCTUPOverCut { get; set; }
        public int OCTUCOverCut { get; set; }
        public int OCTURoughSkin { get; set; }
        public int OCTUOP { get; set; }
        public int OCTUuncomplete { get; set; }
        public int OCTURoughSkinC { get; set; }
        public int OCTURoughSkinP { get; set; }
        public int OCTUStoneRemove { get; set; }
        public int OCTUStoneMove { get; set; }
        public int OCTUOvercut { get; set; }
        public int OCTUHardStone { get; set; }
        public int OCTUHeightDown { get; set; }
        public int OCTUOverHeight { get; set; }
        public int OCTUMachineProblem { get; set; }
        public int OCTUCrownRough { get; set; }
        public int OCTUTotalRepair { get; set; }
        public decimal OCTURepairPer { get; set; }
        public int OCTUBrk { get; set; }


        public int ok { get; set; }
        public int pav { get; set; }
        public int crn { get; set; }
        public int CuletRough { get; set; }
        public int POverCut { get; set; }
        public int COverCut { get; set; }
        public int RoughSkin { get; set; }
        public int OP { get; set; }
        public int uncomplete { get; set; }
        public int RoughSkinC { get; set; }
        public int RoughSkinP { get; set; }
        public int StoneRemove { get; set; }
        public int StoneMove { get; set; }
        public int Overcut { get; set; }
        public int HardStone { get; set; }
        public int HeightDown { get; set; }
        public int OverHeight { get; set; }
        public int MachineProblem { get; set; }
        public int CrownRough { get; set; }
        public int TotalRepair { get; set; }
        public decimal RepairPer { get; set; }
        public int Brk { get; set; }

        public int ReFourpPcs { get; set; }
        public int CrossPcs { get; set; }
        public decimal TotalCarat { get; set; }
        public int MainPcs { get; set; }
        public decimal IssueRPartWeight { get; set; }
        public decimal ReceiveRPartWeight { get; set; }
        public decimal RLossPer { get; set; }
        public decimal IssuePolishWeight { get; set; }
        public decimal ReceivePolishWeight { get; set; }
        public decimal PLossPer { get; set; }


        public string Shape { get; set; }
        public string Size { get; set; }
        public int WITHOUTFT { get; set; }
        public decimal WITHOUTFTPer { get; set; }
        public int WITHFT { get; set; }
        public decimal WITHFTPer { get; set; }
        public int WITHOUTFTPATTA { get; set; }
        public decimal WITHOUTFTPATTAPer { get; set; }
        public int WITHFTPATTA { get; set; }
        public decimal WITHFTPATTAPer { get; set; }
        public int TABLE { get; set; }
        public decimal TABLEPer { get; set; }
        public int TWOINONE { get; set; }
        public decimal TWOINONEPer { get; set; }
        public decimal MANUALPer { get; set; }
        public decimal TOTALPer { get; set; }

        public int Rok { get; set; }
        public int OLsPel { get; set; }
        public int MQLsPel { get; set; }
        public int ODLC { get; set; }
        public int MQDLC { get; set; }
        public int RPlanningMistake { get; set; }
        public int OPlanningMistake { get; set; }
        public int MQPlanningMistake { get; set; }
        public int RFourPMistake { get; set; }
        public int OFourPMistake { get; set; }
        public int MQFourPMistake { get; set; }
        public int RBreak { get; set; }
        public int OBreak { get; set; }
        public int MQBreak { get; set; }
        public int RDamage { get; set; }
        public int ODamage { get; set; }
        public int MQDamage { get; set; }
        public int Pok {get; set;}
        public int PLsPel {get; set;}
        public int PDLC {get; set;}
        public int PPlanningMistake {get; set;}
        public int PFourPMistake {get; set;}
        public int PBreak {get; set;}
        public int PDamage {get; set;}
        public int PTotalPcs { get; set;}

        public string  LotCode { get; set; }
        public int LotPcs { get; set; }
        public int  FPcs { get; set; }
        public decimal  FWeight { get; set; }
        public int  GPcs { get; set; }
        public decimal GWeight { get; set; }
        public decimal GPer { get; set; }
        public int  GPlusPcs { get; set; }
        public decimal GPlusWeight { get; set; }
        public decimal GPlusPer { get; set; }
        public int  HPcs { get; set; }
        public decimal HWeight { get; set; }
        public decimal HPer { get; set; }
        public int  IPcs { get; set; }
        public decimal IWeight { get; set; }
        public decimal IPer { get; set; }
        public int OgalelaPcs { get; set; }
        public decimal OgalelaWeight { get; set; }
        public decimal OgalelaPer { get; set; }
        public int  ColorPattaPcs { get; set; }
        public decimal ColorPattaWeight { get; set; }
        public decimal ColorPattaPer { get; set; }
        public decimal DamageWeight { get; set; }
        public decimal DamagePer { get; set; }
        public int  BlackPcs { get; set; }
        public decimal BlackWeight { get; set; }
        public decimal BlackPer { get; set; }
        public int  TotalLossPcs { get; set; }
        public decimal TotalLossWeight { get; set; }
        public decimal TotalLossPer { get; set; }
        public int  WeightLossPcs { get; set; }
        public decimal WeightLossWeight { get; set; }
        public decimal WeightLossPer { get; set; }
        public decimal TotalWeight { get; set; }
        public int ReceipeID { get; set; }
        public string RoughType { get; set; }
        public string ShapeType { get; set; }

        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal RWeight { get; set; }
        public string Power { get; set; }
        public string MachineNo { get; set; }
        public string OperatorName { get; set; }
    }

    public class RP_Sawing_Loss { 
        public int SrNo { get; set; }
        public string TableNo { get; set; }
        public string SizeCode { get; set; }
        public int? MainPcs { get; set; }
        public decimal? IssueRPartWeight { get; set; }
        public decimal? ReceiveRPartWeight { get; set; }
        public decimal? RLossPer { get; set; }
        public decimal? IssuePolishWeight { get; set; }
        public decimal? ReceivePolishWeight { get; set; }
        public decimal? PLossPer { get; set; }
    }
}