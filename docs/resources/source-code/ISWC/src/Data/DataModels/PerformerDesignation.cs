using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class PerformerDesignation
    {
        public PerformerDesignation()
        {
            WorkInfoPerformer = new HashSet<WorkInfoPerformer>();
            RecordingArtist = new HashSet<RecordingArtist>();
        }

        public int PerformerDesignationId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WorkInfoPerformer> WorkInfoPerformer { get; set; }
        public virtual ICollection<RecordingArtist> RecordingArtist { get; set; }
    }
}