using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Recording
    {
        public Recording()
        {
            RecordingArtist = new HashSet<RecordingArtist>();
        }

        public long RecordingId { get; set; }
        public string RecordingTitle { get; set; }
        public string SubTitle { get; set; }
        public string LabelName { get; set; }
        public long AdditionalIdentifierId { get; set; }
        public DateTime? ReleaseEmbargoDate { get; set; }

        public virtual AdditionalIdentifier AdditionalIdentifier { get; set; }
        public virtual ICollection<RecordingArtist> RecordingArtist { get; set; }
    }
}