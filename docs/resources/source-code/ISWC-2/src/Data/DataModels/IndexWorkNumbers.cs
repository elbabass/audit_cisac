using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class IndexWorkNumbers
    {
        public string GeneratedId { get; set; }
        public long WorkId { get; set; }
        public string TypeCode { get; set; }
        public string Number { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Concurrency { get; set; }
    }
}
