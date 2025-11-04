using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class HighWatermark
    {
        public string Name { get; set; }
        public DateTime Value { get; set; }
    }
}
