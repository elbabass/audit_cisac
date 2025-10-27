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
    public class MD_09 : IRule
    {
        private readonly IStandardizedTitleManager standardizedTitleManager;
        private readonly IRulesManager rulesManager;

        public MD_09(IStandardizedTitleManager standardizedTitleManager, IRulesManager rulesManager)
        {
            this.standardizedTitleManager = standardizedTitleManager;
            this.rulesManager = rulesManager;
        }
        public string Identifier => nameof(MD_09);
        public string ParameterName => "StandardizeTitleWordSpelling";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
            RuleConfiguration = paramValue.ToString();

            if (!paramValue) return (true, submission);

            foreach (var title in submission.Model.Titles)
            {
                var namestring = !string.IsNullOrEmpty(title.StandardizedName) ? title.StandardizedName : title.Name;
                if (namestring == null)
                    continue;

                var wordArray = Regex.Split(namestring.ToUpperInvariant(), @"[^\w\d]");

                var spellingDictionary = (await standardizedTitleManager.FindManyAsync(wordArray.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList()))
                    .ToDictionary(r => r.ReplacePattern, s => s.SearchPattern);

                title.StandardizedName = spellingDictionary.Aggregate(namestring, (current, value)
                    => current.ToUpperInvariant().Replace(value.Key, value.Value));
            }
            return (true, submission);
        }
    }
}
