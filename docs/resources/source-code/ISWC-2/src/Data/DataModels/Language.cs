using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Language
    {
        public Language()
        {
            Message = new HashSet<Message>();
        }

        public int LanguageTypeId { get; set; }
        public string LanguageCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Message> Message { get; set; }
    }
}
