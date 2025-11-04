using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class DerivedFrom
    {
        public int DerivedFromId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Iswc { get; set; }
        public string Title { get; set; }
        public long WorkInfoId { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual WorkInfo WorkInfo { get; set; }
    }
}
