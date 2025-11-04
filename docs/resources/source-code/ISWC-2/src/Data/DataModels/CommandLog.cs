using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class CommandLog
    {
        public int Id { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string IndexName { get; set; }
        public byte? IndexType { get; set; }
        public string StatisticsName { get; set; }
        public int? PartitionNumber { get; set; }
        public string ExtendedInfo { get; set; }
        public string Command { get; set; }
        public string CommandType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}
