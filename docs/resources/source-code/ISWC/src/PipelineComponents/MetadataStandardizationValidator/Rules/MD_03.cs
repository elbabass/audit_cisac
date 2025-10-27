using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules
{
    public class MD_03 : IRule
    {
        private readonly IRulesManager rulesManager;
        public MD_03(IRulesManager rulesManager)
        {
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(MD_03);
        public string ParameterName => "ExcludeTitleTypes";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
		public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var titleExclusions = await rulesManager.GetParameterValueEnumerable<TitleType>(ParameterName);
			RuleConfiguration = await rulesManager.GetParameterValue<string>(ParameterName);

			if (submission.Model?.Titles != null && titleExclusions != null)
                submission.Model.Titles = submission.Model.Titles.Where(x => !titleExclusions.Contains(x.Type)).ToList();

            return (true, submission);
        }
    }
}
