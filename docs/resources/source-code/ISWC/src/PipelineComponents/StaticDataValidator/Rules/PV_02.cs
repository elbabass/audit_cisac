using IdentityServer4.Extensions;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class PV_02 : IRule, IAlwaysOnRule
    {
        private readonly IWorkManager workManager;
        private readonly IAdditionalIdentifierManager additionalIdentifierManager;
        private readonly INumberTypeManager numberTypeManager;
        private readonly IMessagingManager messagingManager;

        public PV_02(IWorkManager workManager, IAdditionalIdentifierManager additionalIdentifierManager, INumberTypeManager numberTypeManager, IMessagingManager messagingManager)
        {
            this.workManager = workManager;
            this.additionalIdentifierManager = additionalIdentifierManager;
            this.numberTypeManager = numberTypeManager;
            this.messagingManager = messagingManager;
        }
        public string Identifier => nameof(PV_02);

        public string ParameterName => string.Empty;

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };

        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var workNumber = submission.Model.WorkNumber;
            var labelIdentifiers = submission.Model.AdditionalIdentifiers.Where(ai => !ai.SubmitterDPID.IsNullOrEmpty());

            if (workNumber != null && await workManager.Exists(workNumber))
            {
                var work = await workManager.FindAsync(submission.Model.WorkNumber);

                if (work != null && work.WorkNumber != null && work.IsReplaced != true)
                {
                    if (submission.Model.DisableAddUpdateSwitching)
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._168);
                        return (false, submission);
                    }
                    submission.ToBeProcessed = true;
                    submission.TransactionType = TransactionType.CUR;
                    submission.Model.PreferredIswc = work.Iswc;
                }
            }
            else if (labelIdentifiers.Any())
            {
                if (labelIdentifiers.Count() > 1)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._248);
                    return (false, submission);
                }

                var workCode = labelIdentifiers.FirstOrDefault()?.WorkCode;
                var submitterCode = labelIdentifiers.FirstOrDefault()?.SubmitterDPID;

                if (workCode != null && await additionalIdentifierManager.Exists(workCode)
                    && submitterCode != null && await numberTypeManager.Exists(submitterCode))
                {
                    var numberTypeId = (await numberTypeManager.FindAsync(submitterCode))?.NumberTypeId;
                    var additionalIdentifiers = await additionalIdentifierManager.FindManyAsync(new string[] { workCode }, numberTypeId ?? 0);
                    var work = (await workManager.FindManyAsync(additionalIdentifiers))?.FirstOrDefault();
                    
                    if (work?.WorkNumber?.Number != null && !work.WorkNumber.Number.IsNullOrEmpty())
                    {
                        if (submission.Model.DisableAddUpdateSwitching)
                        {
                            submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._168);
                            return (false, submission);
                        }
                        submission.ToBeProcessed = true;
                        submission.TransactionType = TransactionType.CUR;
                        submission.Model.PreferredIswc = work.Iswc;
                        submission.Model.WorkNumber = work.WorkNumber;
                    }
                }
            }
          
            return (true, submission);
        }
    }
}
