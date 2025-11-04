using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_50 : IRule
    {
        public string Identifier => nameof(IV_50);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var interestedPartyTypesToRemove = new List<InterestedPartyType?> { InterestedPartyType.AM, InterestedPartyType.E };

            foreach (var ip in model.InterestedParties.ToList())
            {
                if (interestedPartyTypesToRemove.Contains(ip.Type) && ip.IPNameNumber == null && submission.RequestType != RequestType.Label)
                    model.InterestedParties.Remove(ip);
            }

            return await Task.FromResult((true, submission));
        }
    }
}
