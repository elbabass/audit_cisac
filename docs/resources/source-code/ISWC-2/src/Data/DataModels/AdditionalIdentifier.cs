using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class AdditionalIdentifier
    {
        public long AdditionalIdentifierId { get; set; }
        public long WorkInfoId { get; set; }
        public int NumberTypeId { get; set; }
        public string WorkIdentifier { get; set; }

        public virtual NumberType NumberType { get; set; }
        public virtual Recording Recording { get; set; }
        public virtual WorkInfo WorkInfo { get; set; }
    }
}
