using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class AdditionalIdentifier
    {
        public string? WorkCode { get; set; }
        public string? SubmitterCode { get; set; }
        public long? NameNumber { get; set; }
        public int? NumberTypeId { get; set; }
        public string? SubmitterDPID { get; set; }
        public string? RecordingTitle { get; set; }
        public string? SubTitle { get; set; }
        public string? LabelName { get; set; }
        public DateTimeOffset? ReleaseEmbargoDate { get; set; }
        public IEnumerable<Performer>? Performers { get; set; }
    }
}
