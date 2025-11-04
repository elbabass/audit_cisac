using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Data.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using Z.EntityFramework.Plus;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IIswcRepository : IRepository<DataModels.Iswc>
    {
        Task<IEnumerable<IswcModel>> GetIswcModels(IEnumerable<long> iswcIds, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full);
    }

    internal class IswcRepository : BaseRepository<DataModels.Iswc>, IIswcRepository
    {
        private readonly CsiContextReadOnly contextReadOnly;
        private readonly IMapper mapper;

        public IswcRepository(CsiContext context, CsiContextReadOnly contextReadOnly, IMapper mapper, IHttpContextAccessor httpContext) : base(context, httpContext)
        {
            this.contextReadOnly = contextReadOnly;
            this.mapper = mapper;
        }

        private class AgreementData
        {
            public string AgencyId { get; set; }
            public string IpBaseNumber { get; set; }
            public string CreationClass { get; set; }
            public string EconomicRights { get; set; }
        };

        private static readonly string[] eligibleRightTypes = new string[]
        {
            "MW", "BT", "DB", "ER", "MA", "MB", "MD", "MP", "MR", "MT", "MV", "OB","OD",
            "PC", "PR","PT", "RB", "RG", "RL", "RP", "RT", "SY", "TB", "TO", "TP", "TV"
        };
        private static readonly string[] prRightTypes = new string[]
        {
            "MP", "OB", "OD", "PC", "PR", "PT", "RB", "RT", "TB", "TO", "TP", "TV"
        };
        private static readonly string[] mrRightTypes = new string[]
        {
            "MA", "MB", "MD", "MR", "MT", "MV", "SY", "DB"
        };
        private static readonly string[] otherRightTypes = new string[]
        {
            "RL", "BT", "RP", "ER", "RG", "RR"
        };

        public async Task<IEnumerable<IswcModel>> GetIswcModels(IEnumerable<long> iswcIds, bool readOnly = false, DetailLevel detailLevel = DetailLevel.Full)
        {
            var dbSetIswc = readOnly ? contextReadOnly.Set<DataModels.Iswc>() : context.Set<DataModels.Iswc>();
            var dbSetIswclinkedTo = readOnly ? contextReadOnly.Set<IswclinkedTo>() : context.Set<IswclinkedTo>();

            IEnumerable<IswcModel> iswcRecords;
            IEnumerable<IswcModel> linkedToIswcs;
            string[] includes;
            IDictionary<string, List<AgreementData>> prAgreements;
            IDictionary<string, List<AgreementData>> mrAgreements;
            IDictionary<string, List<AgreementData>> otherAgreements;

            IQueryable<DataModels.Iswc> dbSetIswcQueryable = iswcIds.Distinct().Count() == 1
                ? dbSetIswc.Where(x => x.IswcId == iswcIds.First())
                : dbSetIswc.Where(x => iswcIds.Distinct().Contains(x.IswcId));

            switch (detailLevel)
            {
                case DetailLevel.Minimal:
                    return mapper.Map<IEnumerable<IswcModel>>(dbSetIswcQueryable);

                case DetailLevel.Core:
                case DetailLevel.CoreAndLinks:
                    includes = new string[] { "Creator", "Publisher", "Agency", "WorkInfo", "Title",
                    $"{nameof(Creator)}.IpnameNumberNavigation",
                        $"{nameof(Creator)}.IpnameNumberNavigation.NameReference",
                        $"{nameof(Publisher)}.IpnameNumberNavigation",
                        $"{nameof(Publisher)}.IpnameNumberNavigation.NameReference" };

                    iswcRecords = mapper.Map<IEnumerable<IswcModel>>(dbSetIswcQueryable
                        .IncludeAllByPath(includes));

                    if (detailLevel == DetailLevel.CoreAndLinks)
                    {
                        linkedToIswcs = (await
                            (from lt in dbSetIswclinkedTo
                             join linkedfrom in dbSetIswc on lt.IswcId equals linkedfrom.IswcId
                             where lt.Status && linkedfrom.Status && iswcRecords.Select(x => x.Iswc).Contains(lt.LinkedToIswc)
                             select new { key = lt.LinkedToIswc, children = linkedfrom })
                            .ToListAsync())
                            .Union((await
                            (from lt in dbSetIswclinkedTo
                             join linkedfrom in dbSetIswc on lt.IswcId equals linkedfrom.IswcId
                             where lt.Status && linkedfrom.Status && iswcRecords.Select(x => x.Iswc).Contains(lt.Iswc.Iswc1)
                             select new { key = lt.LinkedToIswc, children = linkedfrom })
                            .ToListAsync()))
                            .GroupBy(x => x.key)
                            .Select(x => new IswcModel
                            {
                                Iswc = x.Key,
                                LinkedIswc = mapper.Map<ICollection<IswcModel>>(x.Select(y => y.children))
                            });

                        foreach (var iswc in iswcRecords)
                        {
                            iswc.ParentIswc = linkedToIswcs.FirstOrDefault(x => x.LinkedIswc.Any(y => y.Iswc == iswc.Iswc));
                            iswc.LinkedIswc = linkedToIswcs.Where(x => x.Iswc == iswc.Iswc).SelectMany(x => x.LinkedIswc).ToList();
                            iswc.OverallParentIswc = iswc.FindOverallParentIswc(dbSetIswclinkedTo, dbSetIswc, mapper);
                        }
                    }

                    return iswcRecords;

                case DetailLevel.Full:
                default:
                    var query = dbSetIswcQueryable
                    .Include(i => i.Creator)
                        .ThenInclude(c => c.IpnameNumberNavigation)
                            .ThenInclude(n => n.NameReference)
                    .Include(i => i.Publisher)
                        .ThenInclude(p => p.IpnameNumberNavigation)
                            .ThenInclude(n => n.NameReference)
                    .Include(i => i.Agency)
                    .Include(i => i.WorkInfo)
                    .Include(i => i.Title)

                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.WorkflowInstance)
                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.Agency)
                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.DerivedFrom)
                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.DisambiguationIswc)

                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.WorkInfoInstrumentation)
                            .ThenInclude(wii => wii.Instrumentation)

                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.WorkInfoPerformer)
                            .ThenInclude(wip => wip.Performer)

                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.AdditionalIdentifier)
                            .ThenInclude(ai => ai.NumberType)

                    .Include(i => i.WorkInfo)
                        .ThenInclude(w => w.AdditionalIdentifier)
                            .ThenInclude(ai => ai.Recording)
                                .ThenInclude(r => r.RecordingArtist)
                                    .ThenInclude(ra => ra.Performer)

                    .AsSplitQuery();

                    var materialized = await query.ToListAsync();
                    iswcRecords = mapper.Map<IEnumerable<IswcModel>>(materialized);

                    linkedToIswcs = (await
                        (from lt in dbSetIswclinkedTo
                         join linkedfrom in dbSetIswc on lt.IswcId equals linkedfrom.IswcId
                         where lt.Status && linkedfrom.Status && iswcRecords.Select(x => x.Iswc).Contains(lt.LinkedToIswc)
                         select new { key = lt.LinkedToIswc, children = linkedfrom })
                        .ToListAsync())
                        .Union((await
                        (from lt in dbSetIswclinkedTo
                         join linkedfrom in dbSetIswc on lt.IswcId equals linkedfrom.IswcId
                         where lt.Status && linkedfrom.Status && iswcRecords.Select(x => x.Iswc).Contains(lt.Iswc.Iswc1)
                         select new { key = lt.LinkedToIswc, children = linkedfrom })
                        .ToListAsync()))
                        .GroupBy(x => x.key)
                        .Select(x => new IswcModel
                        {
                            Iswc = x.Key,
                            LinkedIswc = mapper.Map<ICollection<IswcModel>>(x.Select(y => y.children))
                        });

                    var maxDate = DateTime.MaxValue;
                    var creators = iswcRecords.SelectMany(x => x.VerifiedSubmissions.SelectMany(y => y.InterestedParties.Select(z => z.IpBaseNumber))).Distinct();

                    var now = DateTime.UtcNow;
                    var agreements = await contextReadOnly.Agreement
                        .Where(a => creators.Contains(a.IpbaseNumber) && a.FromDate <= now && a.ToDate > now && a.CreationClass == "MW")
                        .Select(a => new AgreementData()
                        {
                            AgencyId = a.AgencyId,
                            IpBaseNumber = a.IpbaseNumber,
                            CreationClass = a.CreationClass,
                            EconomicRights = a.EconomicRights
                        })
                        .ToListAsync();

                    prAgreements = agreements
                        .Where(x => prRightTypes.Contains(x.EconomicRights))
                        .GroupBy(x => x.IpBaseNumber)
                        .ToDictionary(x => x.Key, x => x.ToList());

                    mrAgreements = agreements
                        .Where(x => mrRightTypes.Contains(x.EconomicRights))
                        .GroupBy(x => x.IpBaseNumber)
                        .ToDictionary(x => x.Key, x => x.ToList());

                    otherAgreements = agreements
                        .Where(x => otherRightTypes.Contains(x.EconomicRights))
                        .GroupBy(x => x.IpBaseNumber)
                        .ToDictionary(x => x.Key, x => x.ToList());

                    foreach (var iswc in iswcRecords)
                    {
                        iswc.ParentIswc = linkedToIswcs.FirstOrDefault(x => x.LinkedIswc.Any(y => y.Iswc == iswc.Iswc));
                        iswc.LinkedIswc = linkedToIswcs.Where(x => x.Iswc == iswc.Iswc).SelectMany(x => x.LinkedIswc).ToList();
                        iswc.OverallParentIswc = iswc.FindOverallParentIswc(dbSetIswclinkedTo, dbSetIswc, mapper);
                        SetAffiliation(iswc);
                    }

                    return iswcRecords;
            }

            void SetAffiliation(IswcModel iswc)
            {
                for (int i = 0; i < iswc.VerifiedSubmissions.Count(); i++)
                {
                    for (int x = 0; x < iswc.VerifiedSubmissions.ElementAt(i).InterestedParties.Count(); x++)
                    {
                        var ip = iswc.VerifiedSubmissions.ElementAt(i).InterestedParties.ElementAt(x);

                        if (string.IsNullOrEmpty(ip.IpBaseNumber))
                            continue;

                        ip.Affiliation = CheckAgreements(prAgreements, ip.IpBaseNumber);

                        if (ip.Affiliation != null)
                            continue;

                        ip.Affiliation = CheckAgreements(mrAgreements, ip.IpBaseNumber);

                        if (ip.Affiliation != null)
                            continue;

                        ip.Affiliation = CheckAgreements(otherAgreements, ip.IpBaseNumber);

                        if (ip.Affiliation != null)
                            continue;

                        ip.Affiliation = "099";
                    }
                }

                static string CheckAgreements(IDictionary<string, List<AgreementData>> agreementData, string baseNumber)
                {
                    if (baseNumber.IsNullOrEmpty())
                        return null;

                    if (!agreementData.TryGetValue(baseNumber, out var agreements))
                        return null;

                    var agencies = agreements.Select(x => x.AgencyId).Distinct();
                    if (agencies.Count() == 1)
                        return agencies.First();
                    else if (agencies.Count() > 1)
                        return "Multiple";
                    else
                        return null;
                }
            }
        }
    }
}