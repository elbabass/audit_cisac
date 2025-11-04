using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class User
    {
        public User()
        {
            Agency = new HashSet<Agency>();
            CisacRoleMapping = new HashSet<CisacRoleMapping>();
            Creator = new HashSet<Creator>();
            DeMergeRequest = new HashSet<DeMergeRequest>();
            DerivedFrom = new HashSet<DerivedFrom>();
            DerivedWorkType = new HashSet<DerivedWorkType>();
            DisambiguationIswc = new HashSet<DisambiguationIswc>();
            DisambiguationReason = new HashSet<DisambiguationReason>();
            Instrumentation = new HashSet<Instrumentation>();
            InverseLastModifiedUser = new HashSet<User>();
            Iswc = new HashSet<Iswc>();
            IswclinkedTo = new HashSet<IswclinkedTo>();
            MatchType = new HashSet<MatchType>();
            MergeRequest = new HashSet<MergeRequest>();
            MergeStatus = new HashSet<MergeStatus>();
            Performer = new HashSet<Performer>();
            Publisher = new HashSet<Publisher>();
            RecordingArtist = new HashSet<RecordingArtist>();
            RoleType = new HashSet<RoleType>();
            SubmissionSource = new HashSet<SubmissionSource>();
            Title = new HashSet<Title>();
            TitleType = new HashSet<TitleType>();
            WorkInfo = new HashSet<WorkInfo>();
            WorkInfoInstrumentation = new HashSet<WorkInfoInstrumentation>();
            WorkInfoPerformer = new HashSet<WorkInfoPerformer>();
            WorkflowInstance = new HashSet<WorkflowInstance>();
            WorkflowStatus = new HashSet<WorkflowStatus>();
            WorkflowTask = new HashSet<WorkflowTask>();
            WorkflowTaskStatus = new HashSet<WorkflowTaskStatus>();
            WorkflowType = new HashSet<WorkflowType>();
        }

        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<Agency> Agency { get; set; }
        public virtual ICollection<CisacRoleMapping> CisacRoleMapping { get; set; }
        public virtual ICollection<Creator> Creator { get; set; }
        public virtual ICollection<DeMergeRequest> DeMergeRequest { get; set; }
        public virtual ICollection<DerivedFrom> DerivedFrom { get; set; }
        public virtual ICollection<DerivedWorkType> DerivedWorkType { get; set; }
        public virtual ICollection<DisambiguationIswc> DisambiguationIswc { get; set; }
        public virtual ICollection<DisambiguationReason> DisambiguationReason { get; set; }
        public virtual ICollection<Instrumentation> Instrumentation { get; set; }
        public virtual ICollection<User> InverseLastModifiedUser { get; set; }
        public virtual ICollection<Iswc> Iswc { get; set; }
        public virtual ICollection<IswclinkedTo> IswclinkedTo { get; set; }
        public virtual ICollection<MatchType> MatchType { get; set; }
        public virtual ICollection<MergeRequest> MergeRequest { get; set; }
        public virtual ICollection<MergeStatus> MergeStatus { get; set; }
        public virtual ICollection<Performer> Performer { get; set; }
        public virtual ICollection<Publisher> Publisher { get; set; }
        public virtual ICollection<RecordingArtist> RecordingArtist { get; set; }
        public virtual ICollection<RoleType> RoleType { get; set; }
        public virtual ICollection<SubmissionSource> SubmissionSource { get; set; }
        public virtual ICollection<Title> Title { get; set; }
        public virtual ICollection<TitleType> TitleType { get; set; }
        public virtual ICollection<WorkInfo> WorkInfo { get; set; }
        public virtual ICollection<WorkInfoInstrumentation> WorkInfoInstrumentation { get; set; }
        public virtual ICollection<WorkInfoPerformer> WorkInfoPerformer { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstance { get; set; }
        public virtual ICollection<WorkflowStatus> WorkflowStatus { get; set; }
        public virtual ICollection<WorkflowTask> WorkflowTask { get; set; }
        public virtual ICollection<WorkflowTaskStatus> WorkflowTaskStatus { get; set; }
        public virtual ICollection<WorkflowType> WorkflowType { get; set; }
    }
}
