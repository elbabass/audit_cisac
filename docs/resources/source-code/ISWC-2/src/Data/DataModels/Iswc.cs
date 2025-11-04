using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Iswc
    {
        public Iswc()
        {
            Creator = new HashSet<Creator>();
            IswclinkedTo = new HashSet<IswclinkedTo>();
            Publisher = new HashSet<Publisher>();
            Title = new HashSet<Title>();
            WorkInfo = new HashSet<WorkInfo>();
        }

        public long IswcId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Iswc1 { get; set; }
        public string AgencyId { get; set; }
        public int? IswcStatusId { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual IswcStatus IswcStatus { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<Creator> Creator { get; set; }
        public virtual ICollection<IswclinkedTo> IswclinkedTo { get; set; }
        public virtual ICollection<Publisher> Publisher { get; set; }
        public virtual ICollection<Title> Title { get; set; }
        public virtual ICollection<WorkInfo> WorkInfo { get; set; }
    }
}
