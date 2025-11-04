using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_19 : IRule
    {
        private readonly IMessagingManager messagingManager;
        public IV_19(IMessagingManager messagingManager)
        {
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(IV_19);
        public string ParameterName => "ValidateIPNameNumberExists";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.CIQ, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
		public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;

            if (((model.InterestedParties == null) || (model.InterestedParties.SelectMany(x => x.Names ?? Enumerable.Empty<NameModel>()).Any(n => n.IpNameNumber <= 0 || n.IpNameNumber == 0))) && submission.RequestType != RequestType.Label)
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._102);
                return (false, submission);
            }
            else
                return (true, submission);
        }
    }
}

