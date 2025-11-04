using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.MatchingEngine;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Submissions
{
    public class Submission
    {
        public Submission()
        {
            ToBeProcessed = true;
            HasAlternateIswcMatches = false;
            RulesApplied = new List<RuleExecution>();
            AuditRequestId = Guid.NewGuid();
            Model = new SubmissionModel();
            MatchedResult = new MatchResult();
            IsrcMatchedResult = new MatchResult();
            SearchedIswcModels = new List<IswcModel>();
            WorkflowSearchModel = new WorkflowSearchModel();
            IswcModel = new IswcModel();
            MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes>();
            SearchWorks = new List<WorkModel>();
        }

        public int SubmissionId { get; set; }
        public Guid AuditId { get; set; }
        public Guid AuditRequestId { get; private set; }
        public SubmissionModel Model { get; set; }
        public IswcModel IswcModel { get; set; }
        public ICollection<IswcModel> SearchedIswcModels { get; set; }
        public Rejection? Rejection { get; set; }
        public MatchResult MatchedResult { get; set; }
        public MatchResult IsrcMatchedResult { get; set; }
        public bool HasAlternateIswcMatches { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool ToBeProcessed { get; set; }
        public ICollection<RuleExecution> RulesApplied { get; private set; }
        public bool IsEligible { get; set; }
        public bool IsEligibileOnlyForDeletingIps { get; set; }
        public bool IsProcessed { get; set; }
        public WorkflowSearchModel WorkflowSearchModel { get; set; }
        public SubmissionModel? ExistingWork { get; set; }
        public bool IsPublicRequest { get; set; }
        public bool IsPortalSubmissionFinalStep { get; set; }
        public TransactionSource TransactionSource { get; set; }
        public RequestSource RequestSource { get; set; }
        public DetailLevel DetailLevel { get; set; }
        public string? AgentVersion { get; set; }
        public RequestType RequestType { get; set; }
        public bool UpdateAllocatedIswc { get; set; }
        public string? WorkNumberToReplaceIasWorkNumber { get; set; }
        public string AgencyToReplaceIasAgency { get; set; } = string.Empty;

        public bool MultipleAgencyWorkCodesChild { get; set; }

        public int SubmissionParentId { get; set; }

        public ICollection<MultipleAgencyWorkCodes> MultipleAgencyWorkCodes { get; set; }
        public bool SkipProcessing { get; set; }
        public string MatchingSource { get; set; } = string.Empty;
        public ICollection<WorkModel> SearchWorks { get; set; }
        public bool SocietyWorkCodes { get; set; }
        public bool AdditionalIPNames { get; set; }
    }
}
