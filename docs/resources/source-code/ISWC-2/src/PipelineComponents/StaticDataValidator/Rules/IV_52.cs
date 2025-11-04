using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_52 : IBatchRule
    {
        private readonly IMessagingManager messagingManager;

        public string Identifier => nameof(IV_52);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public IV_52(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public async Task<IEnumerable<Submission>> IsValid(IEnumerable<Submission> submissions)
        {
            var duplicateSubmissions = submissions
                .Where(x => ValidTransactionTypes.Contains(x.TransactionType))
                .GroupBy(x => new { x.Model.WorkNumber.Number })
                .SelectMany(x => x.Skip(1));

            if (duplicateSubmissions.Any())
            {
                foreach (var submission in submissions.Where(s => duplicateSubmissions.Any(x => x.SubmissionId == s.SubmissionId)))
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._170);
                }
            }

            return submissions;
        }
    }
}
