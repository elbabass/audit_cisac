using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class CsiAuditReqWork
    {
        public string Titles { get; set; }
        public string Ips { get; set; }
        public DateTime? CreatedDt { get; set; }
        public int? Soccde { get; set; }
        public string Socwrkcde { get; set; }
        public int? Srcdb { get; set; }
        public int? UpdateSeq { get; set; }
        public DateTime? LastUpdatedDt { get; set; }
        public string LastUpdatedUser { get; set; }
        public int ReqTxId { get; set; }
        public string ArchIswc { get; set; }
        public string PrefIswc { get; set; }
        public string Cat { get; set; }
        public string Deleted { get; set; }
        public DateTime? PostedDt { get; set; }
        public DateTime? LstUpdatedDt { get; set; }
    }
}
