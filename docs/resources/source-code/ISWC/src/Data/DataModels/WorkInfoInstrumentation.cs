using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WorkInfoInstrumentation
    {
        public long WorkInfoId { get; set; }
        public int InstrumentationId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }

        public virtual Instrumentation Instrumentation { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual WorkInfo WorkInfo { get; set; }
    }
}
