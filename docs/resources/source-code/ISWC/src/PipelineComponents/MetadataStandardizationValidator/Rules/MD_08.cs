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
    public class MD_08 : IRule
    {
        private readonly IRulesManager rulesManager;
        public MD_08(IRulesManager rulesManager)
        {
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(MD_08);
        public string ParameterName => "ConvertENTitleNumbersToWords";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<bool>(ParameterName);
            RuleConfiguration = paramValue.ToString();

            if (!paramValue) return (true, submission);

            var convertNumbersDictionary = new Dictionary<string, string>
            {
                {"0", "ZERO"},
                {"1", "ONE"},
                {"2", "TWO"},
                {"3", "THREE"},
                {"4", "FOUR"},
                {"5", "FIVE"},
                {"6", "SIX"},
                {"7", "SEVEN"},
                {"8", "EIGHT"},
                {"9", "NINE"}
            };

            foreach (var title in submission.Model.Titles)
            {
                var namestring = !string.IsNullOrWhiteSpace(title.StandardizedName) ? title.StandardizedName.Trim() : title.Name;
                if (namestring == null)
                    continue;

                namestring = convertNumbersDictionary.Aggregate(namestring, (current, value) => current.Replace(value.Key, $" {value.Value} "));
                var removedMultipleWhiteSpaces = Regex.Replace(namestring, @"\s+", " ").Trim();
                title.StandardizedName = removedMultipleWhiteSpaces;
            }

            return (true, submission);
        }
    }
}
