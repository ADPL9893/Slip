using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Models
{
    public class Process_Wise_Issue_Receive_Loss
    {
        public int SrNo {  get; set; }
        public string Process {  get; set; }
        public string IdealCount { get; set; }
        public int? IssuePcs { get; set; }
        public decimal?  IssueRPartWeight {  get; set; }
        public decimal?  IssuePolishWeight {  get; set; }
        public decimal?  IssueModel {  get; set; }
        public int?  ReceivePcs {  get; set; }
        public decimal?  ReceiveRPartWeight {  get; set; }
        public decimal?  ReceivePolishWeight {  get; set; }
        public decimal?  ReceiveModel {  get; set; }
        public decimal?  LossRPartWeight {  get; set; }
        public decimal?  LossRPartWeightPer {  get; set; }
        public decimal?  LossPolishWeight {  get; set; }
        public decimal?  LossPolishWeightPer {  get; set; }
    }
}