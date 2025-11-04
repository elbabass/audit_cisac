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
    public class MD_16 : IRule
    {
        private readonly IRulesManager rulesManager;
        private readonly IInterestedPartyManager interestedPartyManager;
        private readonly IAgreementManager agreementManager;
        private readonly IMessagingManager messagingManager;

        public MD_16(IRulesManager rulesManager, IInterestedPartyManager interestedPartyManager,
            IAgreementManager agreementManager, IMessagingManager messagingManager)
        {
            this.rulesManager = rulesManager;
            this.interestedPartyManager = interestedPartyManager;
            this.agreementManager = agreementManager;
            this.messagingManager = messagingManager;
        }

        public string Identifier => nameof(MD_16);
        public string ParameterName => "ResolveIPIBaseNumber";
        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> {
            TransactionType.CAR, TransactionType.CUR, TransactionType.MER, TransactionType.CDR, TransactionType.CIQ, TransactionType.FSQ
        };
        public ValidatorType ValidatorType => ValidatorType.MetadataStandardizationValidator;
        public string PipelineComponentVersion => typeof(MetadataStandardizationValidator).GetComponentVersion();
        public string? RuleConfiguration { get; private set; }

        public async Task<(bool IsValid, Submission Submission)> IsValid(Submission submission)
        {
            var model = submission.Model;
            bool paramValue = await rulesManager.GetParameterValue<bool>("ResolveIPIBaseNumber");
            RuleConfiguration = paramValue.ToString();
            var eligibleAgencies = ValidationRuleConverter.GetValues_IncludeAgenciesInEligibilityCheck(
               await rulesManager.GetParameterValue<string>("IncludeAgenciesInEligibilityCheck"), submission.UpdateAllocatedIswc ? submission.AgencyToReplaceIasAgency : submission.Model.Agency);

            if (submission.Model.InterestedParties == null || submission.Model.InterestedParties.Count == 0)
                return (true, submission);

            var ipNameNumbers = model.InterestedParties.SelectMany(ip => ip.Names ?? Enumerable.Empty<NameModel>()).Select(x => x.IpNameNumber);

            if (submission.TransactionType == TransactionType.CIQ)
            {
                var nameNumbers = submission.Model.InterestedParties.Select(x => x.IPNameNumber).Where(x => x.HasValue);
                if (!nameNumbers.Any())
                    return (true, submission);

                if (submission.RequestType == RequestType.ThirdParty && !model.InterestedParties.All(x => !x.IPNameNumber.HasValue))
                {
                    var allIpsNameNumber = await interestedPartyManager.FindManyByNameNumber(ipNameNumbers);

                    foreach (var interestedParty in model.InterestedParties.Where(x => x.IPNameNumber.HasValue).ToList())
                    {
                        var ipWithNameNumber = (allIpsNameNumber ?? Enumerable.Empty<InterestedPartyModel>()).FirstOrDefault(x => (interestedParty.Names ?? Enumerable.Empty<NameModel>())
                            .Any(y => (x.Names ?? Enumerable.Empty<NameModel>()).Any(z => z.IpNameNumber == y.IpNameNumber)));
                        var name = ipWithNameNumber?.Names?.FirstOrDefault(x => x.IpNameNumber == interestedParty.IPNameNumber);

                        var t = submission.Model.InterestedParties.Where(x => x.IPNameNumber == interestedParty.IPNameNumber);

                        submission.Model.InterestedParties.Where(x => x.IPNameNumber == interestedParty.IPNameNumber)
                            .Select(r =>
                            {
                                r.LastName = name?.LastName;
                                r.Name = name?.LastName + ' ' + name?.FirstName;
                                return r;
                            }).ToList();
                    }

                    return (true, submission);
                }
            }

            if (ipNameNumbers.Any(x => x == 0) && submission.RequestType != RequestType.Label && !submission.Model.WorkNumber.Number.StartsWith("PRS_"))
            {
                submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._102);
                return (false, submission);
            }

            var allIpsWithStatuses = await interestedPartyManager.FindManyByNameNumber(ipNameNumbers);

            if (allIpsWithStatuses == null || !allIpsWithStatuses.Any())
                return (true, submission);

            var validStatuses = new List<IpStatus>() { IpStatus.DeletionCrossReference, IpStatus.Purchased, IpStatus.SelfReferencedValidIp };

            foreach (var interestedParty in model.InterestedParties.ToList())
            {
                if ((interestedParty.IPNameNumber == null || interestedParty.IPNameNumber == 0) && (submission.RequestType == RequestType.Label || submission.Model.WorkNumber.Number.StartsWith("PRS_")))
                    continue;

                var ipWithStatus = (allIpsWithStatuses ?? Enumerable.Empty<InterestedPartyModel>()).FirstOrDefault(x => (interestedParty.Names ?? Enumerable.Empty<NameModel>())
                    .Any(y => (x.Names ?? Enumerable.Empty<NameModel>()).Any(z => z.IpNameNumber == y.IpNameNumber)));

                if (ipWithStatus == null || ipWithStatus.Status != null && !validStatuses.Contains(ipWithStatus.Status.StatusCode))
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._137);
                    return (false, submission);
                }

                submission.Model.InterestedParties.Where(x => x.IPNameNumber == interestedParty.IPNameNumber)
                    .Select(r =>
                    {
                        r.IpBaseNumber = ipWithStatus.IpBaseNumber;
                        r.LastName = ipWithStatus?.Names?.FirstOrDefault(x => r.IPNameNumber == x.IpNameNumber)?.LastName;
                        return r;
                    }).ToList();
            }

            if (submission.TransactionType != TransactionType.FSQ)
            {
                submission = submission.IsEligible ? submission : (await new EL_02(agreementManager, rulesManager, messagingManager).IsValid(submission)).Submission;
                submission = (await new EL_01(rulesManager, agreementManager).IsValid(submission)).Submission;
            }

            foreach (var interestedParty in model.InterestedParties.ToList())
            {
                if ((interestedParty.IPNameNumber == null || interestedParty.IPNameNumber == 0) && (submission.RequestType == RequestType.Label || submission.Model.WorkNumber.Number.StartsWith("PRS_")))
                    continue;

                var ipWithStatus = (allIpsWithStatuses ?? Enumerable.Empty<InterestedPartyModel>()).FirstOrDefault(x => (interestedParty.Names ?? Enumerable.Empty<NameModel>())
                    .Any(y => (x.Names ?? Enumerable.Empty<NameModel>()).Any(z => z.IpNameNumber == y.IpNameNumber)));

                if (ipWithStatus == null)
                    continue;

                if (paramValue && ipWithStatus.Status?.StatusCode == IpStatus.DeletionCrossReference)
                {
                    return await AddResolvedInterestedPartyAsync(interestedParty, ipWithStatus);
                }
                else if (ipWithStatus.Status != null && ipWithStatus.Status.StatusCode == IpStatus.DeletionCrossReference)
                {
                    if ((!submission.IsEligible) || (submission.IsEligible && !(await interestedPartyManager.IsAuthoritative(ipWithStatus, eligibleAgencies))))
                    {
                        return await AddResolvedInterestedPartyAsync(interestedParty, ipWithStatus);
                    }
                    else
                    {
                        submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._137);
                        return (false, submission);
                    }
                }
            }

            async Task<(bool IsValid, Submission Submission)> AddResolvedInterestedPartyAsync(InterestedPartyModel interestedParty, InterestedPartyModel ipWithStatus)
            {
                var newIp = await interestedPartyManager.FollowChainForStatus(ipWithStatus);

                if (newIp.Status != null && newIp.Status.StatusCode == IpStatus.DeletionCrossReference)
                {
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._137);
                    return (false, submission);
                }
                else
                {
                    model.InterestedParties.Remove(interestedParty);
                    newIp.IPNameNumber = newIp.Names?.FirstOrDefault(x => x.TypeCode == NameType.PA)?.IpNameNumber;
                    newIp.Type = interestedParty.Type;
                    newIp.CisacType = interestedParty.CisacType;
                    model.InterestedParties.Add(newIp);
                    return (true, submission);
                }
            }

            return (true, submission);
        }
    }
}
