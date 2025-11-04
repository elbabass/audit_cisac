using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules
{
    public class MD_01 : IRule
    {
        private readonly IRulesManager rulesManager;
        public MD_01(IRulesManager rulesManager)
        {
            this.rulesManager = rulesManager;
        }

        public string Identifier => nameof(MD_01);
        public string ParameterName => "CalculateRolledUpRole";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.CIQ, TransactionType.FSQ };

        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var paramValue = await rulesManager.GetParameterValue<string>(ParameterName);
            RuleConfiguration = paramValue;

            var roleMappings = GetRoleMappings();
            foreach (var ip in submission.Model.InterestedParties)
            {
                ip.CisacType = roleMappings.FirstOrDefault(x => x.Value.Any(y => y == ip.Type)).Key;

                if (ip.CisacType == 0)
                {
                    ip.CisacType = roleMappings.FirstOrDefault(x => x.Key.ToString() == ip.Type.ToString()).Key;

                    if (ip.CisacType != 0)
                        ip.Type = roleMappings.FirstOrDefault(x => x.Key.ToString() == ip.Type.ToString()).Value?.FirstOrDefault();
                    else
                        ip.CisacType = CisacInterestedPartyType.X;
                }
                    
            };

            return (true, submission);

            IDictionary<CisacInterestedPartyType, IEnumerable<InterestedPartyType>> GetRoleMappings()
            {
                IDictionary<CisacInterestedPartyType, IEnumerable<InterestedPartyType>> roleDictionary =
                new Dictionary<CisacInterestedPartyType, IEnumerable<InterestedPartyType>>();

                foreach (var roleMapping in paramValue.Split(";"))
                {
                    if (roleMapping.Length > 0)
                    {
                        var roles = Regex.Replace(roleMapping, "[(),]", ";").Split(";").Where(x => !string.IsNullOrWhiteSpace(x));

                        roleDictionary.Add(
                            key: Enum.Parse<CisacInterestedPartyType>(roles.First()),
                            value: roles.Skip(1).Select(x => Enum.TryParse(x.Trim(), out InterestedPartyType value) ? value : throw new Exception($"Couldn't convert '{x}' to InterestedPartyType.")));
                    }
                }

                return roleDictionary;
            }
        }
    }
}
