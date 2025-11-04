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
    public class IV_PopulateIPsMERandCDR : IRule, IAlwaysOnRule
    {
        private readonly IWorkManager workManager;

        public IV_PopulateIPsMERandCDR(IWorkManager workManager)
        {
            this.workManager = workManager;
        }

        public string Identifier => nameof(IV_PopulateIPsMERandCDR);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.MER, TransactionType.DMR, TransactionType.CDR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (string.IsNullOrEmpty(submission.Model.WorkNumber.Type) && string.IsNullOrEmpty(submission.Model.WorkNumber.Number))
            {
                var verifiedSubmission = await workManager.FindVerifiedAsync(model.PreferredIswc, model.Agency);
                if (verifiedSubmission != null)
                    submission.Model.WorkNumber = verifiedSubmission.WorkNumber;
            }

            var workinfos = new List<VerifiedSubmissionModel>();
            if (submission.TransactionType == TransactionType.MER)
            {
                foreach (var number in model.WorkNumbersToMerge)
                {
                    var workinfo = await workManager.FindVerifiedAsync(number);
                    if(workinfo != null)
                        workinfos.Add(workinfo);
                }
                foreach (var iswc in model.IswcsToMerge)
                {
                    var workinfo = await workManager.FindVerifiedAsync(iswc, model.Agency);
                    if (workinfo != null)
                        workinfos.Add(workinfo);
                }
            }
            else
            {
                var workinfo = await workManager.FindVerifiedAsync(submission.Model.WorkNumber);

                if (workinfo != null)
                    workinfos.Add(workinfo);
            }

            if (workinfos.Count > 0 && workinfos.Any(x => x?.InterestedParties != null))
            {
                submission.Model.InterestedParties = workinfos.SelectMany(x => x.InterestedParties).ToList();
                submission.Model.Titles = workinfos.SelectMany(x => x.Titles).ToList();
            }

            return (true, submission);
        }
    }
}
