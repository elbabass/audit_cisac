using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator.Rules
{
    public class MD_17 : IRule
    {
        private readonly IRulesManager rulesManager;
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IAgreementManager agreementManager;
        private readonly IMessagingManager messagingManager;

        public MD_17(IRulesManager rulesManager, IInterestedPartyManager interestedPartyManager, IAgreementManager agreementManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.interestedPartyManager = interestedPartyManager;
            this.agreementManager = agreementManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(MD_17);
        public string ParameterName => "ResolveIPIGroupPseudonym";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.CAR, TransactionType.CUR, TransactionType.MER, TransactionType.CDR, TransactionType.FSQ };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            var paramValue = await rulesManager.GetParameterValue<bool>("ResolveIPIGroupPseudonym");
            RuleConfiguration = paramValue.ToString();
            var eligibleAgencies = ValidationRuleConverter.GetValues_IncludeAgenciesInEligibilityCheck(
               await rulesManager.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck"), submission.UpdateAllocatedIswc ? submission.AgencyToReplaceIasAgency : submission.Model.Agency);

            if (submission.Model.InterestedParties == null || submission.Model.InterestedParties.Count == 0)
                return (true, submission);

            var iswcEligibleRuleEL01 = new EL_01(rulesManager, agreementManager);
            var iswcEligibleRuleEL02 = new EL_02(agreementManager, rulesManager, messagingManager);

            if(submission.TransactionType != TransactionType.FSQ)
            {
                submission = (await iswcEligibleRuleEL01.IsValid(submission)).Submission;

                submission = submission.IsEligible ? submission
                    : (await iswcEligibleRuleEL02.IsValid(submission)).Submission;
            }
            

            if (paramValue)
            {
                var pgIpNameNumbers = await interestedPartyManager.CheckForPgIps(model.InterestedParties.SelectMany(ip => ip.Names ?? Enumerable.Empty<NameModel>()).Select(x => x.IpNameNumber));

                if (!pgIpNameNumbers.Any())
                    return (true, submission);

                var nameNumbers = new List<long>();

                foreach (var ip in model.InterestedParties.Where(ip => ip.IPNameNumber != null && pgIpNameNumbers.Contains((long)ip.IPNameNumber)))
                {
                    if (ip.IPNameNumber != null) nameNumbers.Add(ip.IPNameNumber.Value);
                }

                var pgIps = await interestedPartyManager.FindManyByNameNumber(nameNumbers);

                if (pgIps != null && pgIps.Any())
                {
                    var ipsToAdd = new List<InterestedPartyModel>();

                    foreach (var contributor in model.InterestedParties.Where(ip => ip.IPNameNumber != null && pgIpNameNumbers.Contains((long)ip.IPNameNumber)))
                    {
                        if (pgIps.Any(x => (contributor.Names ?? Enumerable.Empty<NameModel>()).Any(y => (x.Names ?? Enumerable.Empty<NameModel>()).Any(z => z.IpNameNumber == y.IpNameNumber)))
                            && (submission.UpdateAllocatedIswc ? submission.AgencyToReplaceIasAgency : submission.Model.Agency) != null)
                        {
                            contributor.IsAuthoritative = submission.IsEligible && await interestedPartyManager.IsAuthoritative(contributor, eligibleAgencies);
                            if (contributor.Names != null)                                                                                              
                                contributor.Names.First().TypeCode = NameType.PG;

                            foreach (var pgIp in pgIps.Where(x => (contributor.Names ?? Enumerable.Empty<NameModel>())
                                .Any(y => (x.Names ?? Enumerable.Empty<NameModel>()).Any(z => z.IpNameNumber == y.IpNameNumber))))
                            {
                                var paNames = (pgIp.Names ?? Enumerable.Empty<NameModel>()).Where(n => n.TypeCode == NameType.PA).ToList();
                                var paNumber = paNames.FirstOrDefault()?.IpNameNumber;

                                if (!model.InterestedParties.Select(ip => ip.IPNameNumber).Contains(paNumber))
                                {
                                    var ipToAdd = new InterestedPartyModel()
                                    {
                                        Agency = pgIp.Agency,
                                        Name = pgIp.Name,
                                        IpBaseNumber = pgIp.IpBaseNumber,
                                        IPNameNumber = paNumber,
                                        Names = paNames,
                                        DeathDate = pgIp.DeathDate,
                                        CreatedDate = pgIp.CreatedDate,
                                        Status = pgIp.Status,
                                        IsAuthoritative = contributor.IsAuthoritative.Value ? false : await interestedPartyManager.IsAuthoritative(pgIp, eligibleAgencies),
                                        CisacType = contributor.CisacType,
                                        Type = contributor.Type
                                    };

                                    if (ipsToAdd.Any(x=> x.IPNameNumber == ipToAdd.IPNameNumber && x.IpBaseNumber == ipToAdd.IpBaseNumber)) continue;

                                    ipsToAdd.Add(ipToAdd);
                                }

                            }
                        }
                    }

                    model.InterestedParties = model.InterestedParties.Concat(ipsToAdd).ToList();
                    return (true, submission);
                }
                else
                    return (true, submission);

            }
            else
                return (true, submission);
        }
    }
}
