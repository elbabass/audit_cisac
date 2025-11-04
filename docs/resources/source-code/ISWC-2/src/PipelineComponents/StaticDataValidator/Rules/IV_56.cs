using System.Collections.Generic;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator.Rules
{
    public class IV_56 : IRule
    {
        private readonly IRulesManager rulesManager;

        public IV_56(IRulesManager rulesManager)
        {
            this.rulesManager = rulesManager;
        }
        public string Identifier => nameof(IV_56);
        public string ParameterName => string.Empty;
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR };
        public ValidatorType ValidatorType => ValidatorType.StaticValidator;
        public string PipelineComponentVersion => typeof(StaticDataValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }
        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValueEnumerable<string>("StripCharactersFromTitles");

            var model = submission.Model;

            if (!string.IsNullOrWhiteSpace(submission.Model?.WorkNumber?.Number))
                submission.Model.WorkNumber.Number = RemoveSpecialChars(submission.Model.WorkNumber.Number);

            foreach (var title in model.Titles)
                if (!string.IsNullOrWhiteSpace(title.Name))
                    title.Name = RemoveSpecialChars(title.Name);

            if (model.AdditionalAgencyWorkNumbers.Any())
                foreach (var workNumbers in model.AdditionalAgencyWorkNumbers)
                    if (!string.IsNullOrWhiteSpace(workNumbers.WorkNumber.Number))
                        workNumbers.WorkNumber.Number = RemoveSpecialChars(workNumbers.WorkNumber.Number);


            return await Task.FromResult((true, submission));

            string RemoveSpecialChars(string value)
            {
                value = value.ReplaceUnicode();

                if (paramValue != null && paramValue.Any())
                {
                    foreach (var characterReplace in paramValue)
                    {
                        value = value.Replace(characterReplace, "");
                    }
                }

                return value;
            }
        }
    }
}

