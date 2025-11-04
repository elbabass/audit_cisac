using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Caching;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IInterestedPartyManager
    {
        Task<IEnumerable<InterestedPartyModel>> FindManyByBaseNumber(IEnumerable<string> ipBaseNumbers);
        Task<IEnumerable<InterestedPartyModel>> FindManyByNameNumber(IEnumerable<long> ipNameNumbers);
        Task<bool> IsAuthoritative(InterestedPartyModel interestedParty, IEnumerable<string> eligibleAgencies);
        Task<InterestedPartyModel> FollowChainForStatus(InterestedPartyModel interestedParty);
        Task<IEnumerable<long>> CheckForPgIps(IEnumerable<long> ipNameNumbers);
        Task UpsertInterestedPartyAsync(InterestedParty interestedParty);
    }

    public class InterestedPartyManager : IInterestedPartyManager
    {
        private readonly IInterestedPartyRepository interestedPartyRepository;
        private readonly IMapper mapper;
        private readonly IAgreementRepository agreementRepository;
        private readonly INameRepository nameRepository;
        private readonly ICacheClient cacheClient;

        public InterestedPartyManager(
            IInterestedPartyRepository interestedPartyRepository,
            IMapper mapper,
            IAgreementRepository agreementRepository,
            INameRepository nameRepository,
            ICacheClient cacheClient)
        {
            this.interestedPartyRepository = interestedPartyRepository;
            this.mapper = mapper;
            this.agreementRepository = agreementRepository;
            this.nameRepository = nameRepository;
            this.cacheClient = cacheClient;
        }

        public async Task<IEnumerable<InterestedPartyModel>> FindManyByBaseNumber(IEnumerable<string> ipBaseNumbers)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                var ips = await interestedPartyRepository.FindManyAsyncOptimizedByPath(x => ipBaseNumbers.Contains(x.IpbaseNumber),
                 $"{nameof(NameReference)}", $"{nameof(NameReference)}.IpnameNumberNavigation", $"{nameof(Agency)}", $"{nameof(Status)}");

                foreach (var ip in ips)
                    ip.Status = ip.Status.OrderByDescending(x => x.ToDate).ToList();

                return mapper.Map<IEnumerable<InterestedPartyModel>>(ips);

            }, keys: ipBaseNumbers.ToArray());
        }

        public async Task<IEnumerable<InterestedPartyModel>> FindManyByNameNumber(IEnumerable<long> ipNameNumbers)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                var ips = await interestedPartyRepository
                .FindManyAsyncOptimizedByPath(x => x.NameReference.Any(i => ipNameNumbers.Contains(i.IpnameNumber)),
                 $"{nameof(NameReference)}", $"{nameof(NameReference)}.IpnameNumberNavigation", $"{nameof(Agency)}", $"{nameof(Status)}");

                foreach (var ip in ips)
                    ip.Status = ip.Status.OrderByDescending(x => x.ToDate).ToList();

                return mapper.Map<IEnumerable<InterestedPartyModel>>(ips);

            }, keys: ipNameNumbers.Cast().ToArray());
        }

        public async Task<IEnumerable<long>> CheckForPgIps(IEnumerable<long> ipNameNumbers)
        {
            return await cacheClient.GetOrCreateAsync(async () =>
            {
                var names = await nameRepository.FindManyAsync(n => ipNameNumbers.Contains(n.IpnameNumber));
                return names.Where(x => x.TypeCode == "PG").Select(y => y.IpnameNumber).ToList();

            }, keys: ipNameNumbers.Cast().ToArray());
        }

        public async Task<bool> IsAuthoritative(InterestedPartyModel interestedParty, IEnumerable<string> eligibleAgencies)
        {
            if (eligibleAgencies == null || interestedParty == null || interestedParty.IpBaseNumber == null) return false;

            return await cacheClient.GetOrCreateAsync(async () =>
            {
                var eligibleRightTypes = new List<string>()
                {
                    "MW", "BT", "DB", "ER", "MA", "MB", "MD", "MP", "MR", "MT", "MV", "OB","OD",
                    "PC", "PR","PT", "RB", "RG", "RL", "RP", "RT", "SY", "TB", "TO", "TP", "TV"
                };
                var now = DateTime.UtcNow;

                return (await agreementRepository.FindManyAsync(a => a.IpbaseNumber.Equals(interestedParty.IpBaseNumber) && eligibleAgencies.Contains(a.AgencyId)
                             && a.FromDate <= now && a.ToDate > now && eligibleRightTypes.Contains(a.EconomicRights))).Any();

            }, keys: new[] { interestedParty.IpBaseNumber }.Concat(eligibleAgencies).ToArray());
        }

        public async Task<InterestedPartyModel> FollowChainForStatus(InterestedPartyModel interestedParty)
        {
            return await Recursive(interestedParty);

            async Task<InterestedPartyModel> Recursive(InterestedPartyModel parentModel)
            {
                var child = await interestedPartyRepository.FindAsyncOptimizedByPath(ip =>
                    ip.Status.Any(status => parentModel.Status!.ForwardingBaseNumber == status.IpbaseNumber),
                    $"{nameof(NameReference)}",
                    $"{nameof(NameReference)}.IpnameNumberNavigation",
                    $"{nameof(Agency)}",
                    $"{nameof(Status)}");

                var now = DateTime.UtcNow;

                if (child == null || interestedParty.IpBaseNumber == child.IpbaseNumber)
                    return parentModel;

                var childStatuses = child.Status ?? new List<Status>();
                var hasDeletionXref = childStatuses.Any(x => x.StatusCode == (int)IpStatus.DeletionCrossReference);

                if (hasDeletionXref)
                {
                    var current = childStatuses.FirstOrDefault(s => s.FromDate <= now && s.ToDate > now);

                    if (!string.IsNullOrWhiteSpace(child.IpbaseNumber) && !string.IsNullOrWhiteSpace(current?.ForwardingBaseNumber)
                        && string.Equals(child.IpbaseNumber.Trim(), current.ForwardingBaseNumber.Trim(), StringComparison.OrdinalIgnoreCase))
                        return mapper.Map<InterestedPartyModel>(child);
                    else
                    {
                        var childModel = mapper.Map<InterestedPartyModel>(child);
                        var delXrefStatus = childStatuses
                    .       FirstOrDefault(x => x.StatusCode == (int)IpStatus.DeletionCrossReference);

                        if (delXrefStatus != null)
                            childModel.Status = mapper.Map<StatusModel>(delXrefStatus);

                        return await Recursive(childModel);
                    }
                }
                else
                    return mapper.Map<InterestedPartyModel>(child);
            }
        }

        public async Task UpsertInterestedPartyAsync(InterestedParty interestedParty)
        {
            var existingIp = await interestedPartyRepository
                .FindManyAsyncOptimizedByPath(x => interestedParty.IpbaseNumber == x.IpbaseNumber,
                $"{nameof(NameReference)}",
                $"{nameof(NameReference)}.IpnameNumberNavigation",
                $"{nameof(Status)}",
                $"{nameof(IpnameUsage)}",
                $"{nameof(Agreement)}");

            var existingNames = new List<Name>();
            await CheckExistingNames();

            await interestedPartyRepository.UpsertInterestedParty(interestedParty, existingIp?.FirstOrDefault(), existingNames);

            async Task CheckExistingNames()
            {
                foreach (var nameReference in interestedParty.NameReference)
                {
                    var existingName = await nameRepository.FindAsyncOptimized(n => n.IpnameNumber == nameReference.IpnameNumber);

                    if (existingName != null)
                    {
                        existingNames.Add(existingName);
                    }
                }
            }
        }
    }
}
