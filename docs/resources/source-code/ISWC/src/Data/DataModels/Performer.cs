using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Performer
    {
        public Performer()
        {
            RecordingArtist = new HashSet<RecordingArtist>();
            WorkInfoPerformer = new HashSet<WorkInfoPerformer>();
        }

        public long PerformerId { get; set; }
        public string Isni { get; set; }
        public int? Ipn { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<RecordingArtist> RecordingArtist { get; set; }
        public virtual ICollection<WorkInfoPerformer> WorkInfoPerformer { get; set; }
    }
}
