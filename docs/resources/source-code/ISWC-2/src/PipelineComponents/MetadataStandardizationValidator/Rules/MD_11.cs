using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules
{
    public class MD_11 : IRule
    {
		private readonly IRulesManager rulesManager;
		public MD_11(IRulesManager rulesManager)
		{
			this.rulesManager = rulesManager;
		}

        public string Identifier => nameof(MD_11);
        public string ParameterName => "StandardizeTitleENPlurals";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
			var paramValue = await rulesManager.GetParameterValue<bool>("StandardizeTitleENPlurals");
			RuleConfiguration = paramValue.ToString();

			if(!paramValue)
				return (true, submission);

			foreach (var title in submission.Model.Titles)
            {
                var namestring = !string.IsNullOrEmpty(title.StandardizedName) ? title.StandardizedName : title.Name;
                if (namestring == null) continue;

                var wordArray = namestring.SplitWordOnSeparators();
                foreach ((var word, var idx) in wordArray.Select((w, i) => (w.ToUpper(), i)))
                {
                    if (word.Length >= 1)
                    {
                        wordArray[idx] = word.Last() == 'S' ? wordArray[idx].Remove(word.Length - 1) : wordArray[idx];
                    }
                }
                title.StandardizedName = string.Join("", wordArray);
            }
            return (true, submission);
        }
    }
}
