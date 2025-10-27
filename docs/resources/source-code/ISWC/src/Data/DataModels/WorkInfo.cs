using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WorkInfo
    {
        public WorkInfo()
        {
            AdditionalIdentifier = new HashSet<AdditionalIdentifier>();
            Creator = new HashSet<Creator>();
            DerivedFrom = new HashSet<DerivedFrom>();
            DisambiguationIswc = new HashSet<DisambiguationIswc>();
            Publisher = new HashSet<Publisher>();
            Title = new HashSet<Title>();
            WorkInfoInstrumentation = new HashSet<WorkInfoInstrumentation>();
            WorkInfoPerformer = new HashSet<WorkInfoPerformer>();
            WorkflowInstance = new HashSet<WorkflowInstance>();
        }

        public virtual long WorkInfoId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public long IswcId { get; set; }
        public string ArchivedIswc { get; set; }
        public DateTime? CisnetLastModifiedDate { get; set; }
        public DateTime? CisnetCreatedDate { get; set; }
        public int Ipcount { get; set; }
        public bool IsReplaced { get; set; }
        public bool IswcEligible { get; set; }
        public int? MatchTypeId { get; set; }
        public string MwiCategory { get; set; }
        public string AgencyId { get; set; }
        public string AgencyWorkCode { get; set; }
        public int SourceDatabase { get; set; }
        public bool? Disambiguation { get; set; }
        public int? DisambiguationReasonId { get; set; }
        public string Bvltr { get; set; }
        public int? DerivedWorkTypeId { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual DerivedWorkType DerivedWorkType { get; set; }
        public virtual DisambiguationReason DisambiguationReason { get; set; }
        public virtual Iswc Iswc { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual MatchType MatchType { get; set; }
        public virtual ICollection<AdditionalIdentifier> AdditionalIdentifier { get; set; }
        public virtual ICollection<Creator> Creator { get; set; }
        public virtual ICollection<DerivedFrom> DerivedFrom { get; set; }
        public virtual ICollection<DisambiguationIswc> DisambiguationIswc { get; set; }
        public virtual ICollection<Publisher> Publisher { get; set; }
        public virtual ICollection<Title> Title { get; set; }
        public virtual ICollection<WorkInfoInstrumentation> WorkInfoInstrumentation { get; set; }
        public virtual ICollection<WorkInfoPerformer> WorkInfoPerformer { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstance { get; set; }
    }
}
