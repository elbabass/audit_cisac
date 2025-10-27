using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class MatchType
    {
        public MatchType()
        {
            WorkInfo = new HashSet<WorkInfo>();
        }

        public int MatchTypeId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Description { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<WorkInfo> WorkInfo { get; set; }
    }
}
