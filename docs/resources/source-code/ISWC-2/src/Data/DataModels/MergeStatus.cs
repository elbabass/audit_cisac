using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class MergeStatus
    {
        public MergeStatus()
        {
            DeMergeRequest = new HashSet<DeMergeRequest>();
            MergeRequest = new HashSet<MergeRequest>();
        }

        public int MergeStatusId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Description { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<DeMergeRequest> DeMergeRequest { get; set; }
        public virtual ICollection<MergeRequest> MergeRequest { get; set; }
    }
}
