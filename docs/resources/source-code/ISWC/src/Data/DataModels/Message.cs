using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public string Header { get; set; }
        public string MessageBody { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Portal { get; set; }
        public int LanguageTypeId { get; set; }

        public virtual Language LanguageType { get; set; }
    }
}
