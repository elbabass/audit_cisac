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
    public class MD_13 : IRule
    {
		private readonly IRulesManager rulesManager;
		public MD_13(IRulesManager rulesManager)
		{
			this.rulesManager = rulesManager;
		}

        public string Identifier => nameof(MD_13);
        public string ParameterName => "StandardizeTitleZ";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
		public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
		public string? RuleConfiguration { get; private set; }

		public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
			var paramValue = await rulesManager.GetParameterValue<bool>("StandardizeTitleZ");
			RuleConfiguration = paramValue.ToString();

			if (!paramValue)
				return (true, submission);

            var standardIZE = new Dictionary<string, string>
            {
                {"ize","ise"},
                {"izE","isE"},
                {"iZe","iSe"},
                {"iZE","iSE"},
                {"Ize","Ise"},
                {"IzE","IsE"},
                {"IZe","ISe"},
                {"IZE","ISE"}
            };

            var standardYZE = new Dictionary<string, string>
            {

                {"yze","yse"},
                {"yzE","ysE"},
                {"yZe","ySe"},
                {"yZE","ySE"},
                {"Yze","Yse"},
                {"YzE","YsE"},
                {"YZe","YSe"},
                {"YZE","YSE"}
            };

            var standardPART = new Dictionary<string, string>
            {
                {"part","pt"},
                {"Part","Pt"},
                {"PART","PT"},
                {"PArt","PT"},
                {"PArT","PT"},
                {"PaRT","PT"},
                {"pARt","PT"},
                {"pArT","PT"},
                {"paRT","PT"},
                {"PARt","PT"},
                {"PaRt","PT"},
                {"pArt","PT"},
                {"pART","PT"},
                {"paRt","PT"},
                {"parT","PT"},
                {"ParT","PT"}
            };

            foreach (var title in submission.Model.Titles)
            {
                var namestring = !string.IsNullOrEmpty(title.StandardizedName) ? title.StandardizedName : title.Name;
                if (namestring == null) continue;

                var wordArray = namestring.SplitWordOnSeparators();
                foreach ((var word, var idx) in wordArray.Select((w, i) => (w.ToUpper(), i)))
                {
                    if (string.IsNullOrWhiteSpace(word)) continue;

                    StandardizeWord("IZE", standardIZE);
                    StandardizeWord("YZE", standardYZE);
                    StandardizeWord("PART", standardPART);

                    void StandardizeWord(string searchString, Dictionary<string, string> standardisedWords)
                    {
                        if (word.Contains(searchString))
                            wordArray[idx] = standardisedWords.Aggregate(wordArray[idx], (current, value) => current.Replace(value.Key, value.Value));
                    }
                }
                title.StandardizedName = string.Join("", wordArray);
            }

            return (true, submission);
        }
    }
}
