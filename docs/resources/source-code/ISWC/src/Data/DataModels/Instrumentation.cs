using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Instrumentation
    {
        public Instrumentation()
        {
            WorkInfoInstrumentation = new HashSet<WorkInfoInstrumentation>();
        }

        public int InstrumentationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Family { get; set; }
        public string Note { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<WorkInfoInstrumentation> WorkInfoInstrumentation { get; set; }
    }
}
