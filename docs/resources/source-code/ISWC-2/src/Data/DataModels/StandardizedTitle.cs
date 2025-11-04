using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class StandardizedTitle
    {
        public int StandardizedTitleId { get; set; }
        public string Society { get; set; }
        public string SearchPattern { get; set; }
        public string ReplacePattern { get; set; }
    }
}
