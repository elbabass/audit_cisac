using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.Submissions
{
    public class SubmissionModel
    {
        public SubmissionModel()
        {
            DisambiguateFrom = new List<DisambiguateFrom>();
            DerivedFrom = new List<DerivedFrom>();
            Performers = new List<Performer>();
            Instrumentation = new List<Instrumentation>();
            Titles = new List<Title>();
            InterestedParties = new List<InterestedPartyModel>();
            IswcsToMerge = new List<string>();
            WorkNumbersToMerge = new List<WorkNumber>();
            WorkNumber = new WorkNumber();
            WorkflowTasks = new List<WorkflowTask>();
            Agency = string.Empty;
            PreferredIswc = string.Empty;
            Iswc = string.Empty;
            AdditionalAgencyWorkNumbers = new List<AdditionalAgencyWorkNumber>();
            Affiliations = new List<string>();
        }
        public string PreferredIswc { get; set; }
        public int SourceDb { get; set; }
        public WorkNumber WorkNumber { get; set; }
        public bool Disambiguation { get; set; }
        public DisambiguationReason? DisambiguationReason { get; set; }
        public ICollection<DisambiguateFrom> DisambiguateFrom { get; set; }
        public BVLTR? BVLTR { get; set; }
        public DerivedWorkType? DerivedWorkType { get; set; }
        public ICollection<DerivedFrom> DerivedFrom { get; set; }
        public ICollection<Performer> Performers { get; set; }
        public ICollection<Instrumentation> Instrumentation { get; set; }
        public DateTimeOffset? CisnetCreatedDate { get; set; }
        public DateTimeOffset? CisnetLastModifiedDate { get; set; }
        public string Iswc { get; set; }
        public string Agency { get; set; }
        public ICollection<Title> Titles { get; set; }
        public ICollection<InterestedPartyModel> InterestedParties { get; set; }
        public bool IsReplaced { get; set; }
        public string? ReasonCode { get; set; }
        public bool ApproveWorkflowTasks { get; set; }
        public Category Category { get; set; }
        public ICollection<string> IswcsToMerge { get; set; }
        public ICollection<WorkNumber> WorkNumbersToMerge { get; set; }
        public int? StartIndex { get; set; }
        public int? PageLength { get; set; }
        public IEnumerable<WorkflowTask> WorkflowTasks { get; set; }
        public bool PreviewDisambiguation { get; set; }
        public IEnumerable<AdditionalIdentifier>? AdditionalIdentifiers { get; set; }
        public bool RelatedSubmissionIncludedIswc { get; set; }
        public bool IsPublicRequest { get; set; }
        public bool DisableAddUpdateSwitching { get; set; }
        public bool AllowProvidedIswc { get; set; }
        public IEnumerable<AdditionalAgencyWorkNumber> AdditionalAgencyWorkNumbers { get; set; }
        public ICollection<string> Affiliations { get; set; }
        public string HashCode => GetHashValue();
        public string GetHashValue()
        {
            StringBuilder dataToHash = new StringBuilder();

            dataToHash.Append(PreferredIswc);

            foreach (var title in Titles)
            {
                dataToHash.Append(title.Name);
            }

            foreach (var ip in InterestedParties)
                if (ip.IsPublisherInterestedPartyType() || ip.IsCreatorInterestedPartyType())
                {
                    dataToHash.Append(ip.IPNameNumber);
                    dataToHash.Append(ip.Type?.ToFriendlyString());
                }

            dataToHash.Append(Disambiguation);
            dataToHash.Append(DisambiguationReason);
            dataToHash.Append(BVLTR);

            foreach (var iswc in DisambiguateFrom)
                dataToHash.Append(iswc.Iswc);
            foreach (var item in Instrumentation)
                dataToHash.Append(item.Code);

            foreach (var item in Performers)
            {
                dataToHash.Append(item.FirstName);
                dataToHash.Append(item.LastName);
            }

            return dataToHash.ToString().GetHashValue();
        }

        public SubmissionModel Copy() => (SubmissionModel)this.MemberwiseClone();
    }
}
