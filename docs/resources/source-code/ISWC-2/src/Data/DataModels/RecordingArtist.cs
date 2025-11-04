using System;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class RecordingArtist
    {
        public long RecordingId { get; set; }
        public long PerformerId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public int? PerformerDesignationId { get; set; }

        public virtual PerformerDesignation PerformerDesignation { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual Performer Performer { get; set; }
        public virtual Recording Recording { get; set; }
    }
}
