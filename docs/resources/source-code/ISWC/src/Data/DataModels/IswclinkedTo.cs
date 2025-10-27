using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class IswclinkedTo
    {
        public long IswcLinkedToId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public long IswcId { get; set; }
        public string LinkedToIswc { get; set; }
        public long? MergeRequest { get; set; }
        public long? DeMergeRequest { get; set; }

        public virtual DeMergeRequest DeMergeRequestNavigation { get; set; }
        public virtual Iswc Iswc { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual MergeRequest MergeRequestNavigation { get; set; }
    }
}
