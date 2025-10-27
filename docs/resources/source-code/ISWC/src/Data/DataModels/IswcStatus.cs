using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class IswcStatus
    {
        public IswcStatus()
        {
            Iswc = new HashSet<Iswc>();
        }

        public int IswcStatusId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Iswc> Iswc { get; set; }
    }
}