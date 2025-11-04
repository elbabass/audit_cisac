using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class DerivedWorkType
    {
        public DerivedWorkType()
        {
            WorkInfo = new HashSet<WorkInfo>();
        }

        public int DerivedWorkTypeId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Description { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<WorkInfo> WorkInfo { get; set; }
    }
}
