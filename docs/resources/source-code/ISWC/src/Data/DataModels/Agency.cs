using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Agency
    {
        public Agency()
        {
            Agreement = new HashSet<Agreement>();
            DeMergeRequest = new HashSet<DeMergeRequest>();
            InterestedParty = new HashSet<InterestedParty>();
            Iswc = new HashSet<Iswc>();
            MergeRequest = new HashSet<MergeRequest>();
            NameNavigation = new HashSet<Name>();
            WebUser = new HashSet<WebUser>();
            WorkInfo = new HashSet<WorkInfo>();
            WorkflowTask = new HashSet<WorkflowTask>();
        }

        public string AgencyId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public int Iswcstatus { get; set; }
        public string DisallowDisambiguateOverwrite { get; set; }
        public bool EnableChecksumValidation { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<Agreement> Agreement { get; set; }
        public virtual ICollection<DeMergeRequest> DeMergeRequest { get; set; }
        public virtual ICollection<InterestedParty> InterestedParty { get; set; }
        public virtual ICollection<Iswc> Iswc { get; set; }
        public virtual ICollection<MergeRequest> MergeRequest { get; set; }
        public virtual ICollection<Name> NameNavigation { get; set; }
        public virtual ICollection<WebUser> WebUser { get; set; }
        public virtual ICollection<WorkInfo> WorkInfo { get; set; }
        public virtual ICollection<WorkflowTask> WorkflowTask { get; set; }
    }
}
