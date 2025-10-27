using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_34 : IRule
    {
        private readonly IMessagingManager messagingManager;
        private readonly IRulesManager rulesManager;
        private readonly IWorkManager workManager;

        public IV_34(IRulesManager rulesManager, IWorkManager workManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.workManager = workManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_34);
        public string ParameterName => "ValidateModifiedVersions";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var paramValue = await rulesManager.GetParameterValue<string>(this.ParameterName);
            RuleConfiguration = paramValue;

            if (model.DerivedWorkType == DerivedWorkType.ModifiedVersion)
            {
                if (paramValue.Equals("basic"))
                {
                    foreach (var der in model.DerivedFrom)
                    {
                        if (!string.IsNullOrWhiteSpace(der.Iswc))
                        {
                            if (!der.Iswc.IsValidIswcPattern())
                            {
                                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._119);
                                return (false, submission);
                            }
                            else if (await workManager.FindAsync(der.Iswc) == null)
                            {
                                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._119);
                                return (false, submission);
                            }
                        }
                    }
                }
                else if (paramValue.Equals("full"))
                {
                    if (!model.DerivedFrom.Any())
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._118);
                        return (false, submission);
                    }

                    foreach (var der in model.DerivedFrom)
                    {
                        if (string.IsNullOrWhiteSpace(der.Title) && string.IsNullOrWhiteSpace(der.Iswc))
                        {
                            submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._120);
                            return (false, submission);
                        }
                        else if ((!string.IsNullOrWhiteSpace(der.Title) || string.IsNullOrWhiteSpace(der.Title)) && !string.IsNullOrWhiteSpace(der.Iswc))
                        {
                            if (!der.Iswc.IsValidIswcPattern() || (await workManager.FindAsync(der.Iswc) == null))
                            {
                                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._119);
                                return (false, submission);
                            }
                        }
                    }
                }
            }

            return (true, submission);
        }
    }
}
