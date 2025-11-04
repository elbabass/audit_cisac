using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Title
    {
        public long TitleId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public long IswcId { get; set; }
        public long WorkInfoId { get; set; }
        public string StandardizedTitle { get; set; }
        public string Title1 { get; set; }
        public int TitleTypeId { get; set; }

        public virtual Iswc Iswc { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual TitleType TitleType { get; set; }
        public virtual WorkInfo WorkInfo { get; set; }
    }
}
