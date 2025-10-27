using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class PV_03 : IRule, IAlwaysOnRule
    {
        private readonly IWorkManager workManager;

        public PV_03(IWorkManager workManager)
        {
            this.workManager = workManager;
        }

        public string Identifier => nameof(PV_03);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType>() { TransactionType.CUR };

        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {

            var workNumber = submission.Model.WorkNumber;

            if (workNumber != null && submission.ExistingWork == null && !(await workManager.IsDeleted(workNumber)))
            {
                if (!string.IsNullOrWhiteSpace(submission.Model.PreferredIswc) && submission.Model.AdditionalIdentifiers.Count() == 0)
                {
                    var result = await workManager.FindAllocatedWorkAsync(submission.Model.PreferredIswc);

                    if (result != null && !string.IsNullOrWhiteSpace(result.WorkNumber?.Number) && result.WorkNumber.Number.StartsWith("AS"))
                    {
                        submission.WorkNumberToReplaceIasWorkNumber = submission.Model.WorkNumber.Number;
                        submission.AgencyToReplaceIasAgency = submission.Model.Agency;
                        submission.Model.WorkNumber.Number = result.WorkNumber.Number;
                        submission.Model.WorkNumber.Type = result.WorkNumber.Type;
                        submission.Model.Agency = result.Agency;
                        submission.UpdateAllocatedIswc = true;
                        submission.ExistingWork = result;
                        submission.IsEligible = false;
                        if (result.InterestedParties.Any(x => x.CisacType == Bdo.Ipi.CisacInterestedPartyType.E))
                        {
                            foreach (var pub in result.InterestedParties.Where(x => x.CisacType == Bdo.Ipi.CisacInterestedPartyType.E))
                            {
                                if (submission.Model.InterestedParties.Any(x => x.IPNameNumber != pub.IPNameNumber))
                                    submission.Model.InterestedParties.Add(pub);
                            }
                        }
                    }
                    else
                    {
                        submission.ToBeProcessed = true;
                        submission.TransactionType = TransactionType.CAR;
                        submission.MatchedResult.Matches = new List<Bdo.MatchingEngine.MatchingWork>();
                    }
                }
                else
                {
                    submission.ToBeProcessed = true;
                    submission.TransactionType = TransactionType.CAR;
                    submission.MatchedResult.Matches = new List<Bdo.MatchingEngine.MatchingWork>();
                }
            }

            return (true, submission);
        }

    }
}
