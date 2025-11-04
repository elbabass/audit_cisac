using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class NumberType
    {
        public NumberType()
        {
            AdditionalIdentifier = new HashSet<AdditionalIdentifier>();
        }

        public int NumberTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AdditionalIdentifier> AdditionalIdentifier { get; set; }
    }
}
