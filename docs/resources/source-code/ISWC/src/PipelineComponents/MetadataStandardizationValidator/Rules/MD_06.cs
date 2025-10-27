using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules
{
    public class MD_06 : IRule
    {
        private readonly IRulesManager rulesManager;
        public MD_06(IRulesManager rulesManager)
        {
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(MD_06);
        public string ParameterName => "RemoveTitleCharacters";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<string>(this.ParameterName);
            RuleConfiguration = paramValue;

            foreach (var title in submission.Model.Titles)
            {
                var namestring = !string.IsNullOrEmpty(title.StandardizedName) ? title.StandardizedName : title.Name;
                title.StandardizedName = Regex.Replace(namestring, @paramValue, "");
            }

            return (true, submission);
        }
    }
}
