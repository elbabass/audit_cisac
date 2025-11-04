using AutoMapper;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SpanishPoint.Azure.Iswc.Bdo.Agent;
using SpanishPoint.Azure.Iswc.Bdo.Audit;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Reports;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Services.AgentRun.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Matching.MatchingEngine.Entities;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Models;
using SpanishPoint.Azure.Iswc.Framework.Databricks.Models;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDb = SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;

namespace SpanishPoint.Azure.Iswc.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DataModels.Iswc, IswcModel>()
                .ForMember(dest => dest.Iswc, opts => opts.MapFrom(src => src.Iswc1))
                .ForMember(dest => dest.VerifiedSubmissions, opts => opts.MapFrom(src => src.WorkInfo.Any() ? src.WorkInfo.Where(w => w.Status) : src.WorkInfo))
                .ForMember(dest => dest.LastModifiedUser, opt => opt.MapFrom(src => src.LastModifiedUser.Name))
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.AgencyId))
                .ForMember(dest => dest.LinkedIswc, opt => opt.MapFrom(src => src.IswclinkedTo.Where(x => x.Status)));


            CreateMap<IswclinkedTo, IswcModel>()
                .ForMember(dest => dest.Iswc, opt => opt.MapFrom(src => src.LinkedToIswc))
                .ForMember(dest => dest.VerifiedSubmissions, opt => opt.MapFrom(src => src.Iswc.WorkInfo))
                .ForMember(dest => dest.LinkedIswc, opt => opt.MapFrom(src => src.Iswc.IswclinkedTo.Where(x => x.Status)));

            CreateMap<Bdo.Work.DerivedFrom, WorkNumber>()
                .ForMember(dest => dest.Number, map => map.MapFrom(src => src.Iswc ?? null))
                .ForMember(dest => dest.Type, map => map.MapFrom(src => (src.Iswc != null) ? "ISWC" : null));

            CreateMap<SubmissionSource, SubmissionSourceModel>().ReverseMap();

            CreateMap<InterestedParty, InterestedPartyModel>()
                .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.NameReference.Select(x => x.IpnameNumberNavigation)))
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.Agency.AgencyId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.FirstOrDefault()))
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Status, StatusModel>().ReverseMap();

            CreateMap<ICollection<Status>, StatusModel>();

            CreateMap<Creator, InterestedPartyModel>()
                .ForMember(dest => dest.Names, opt => opt.MapFrom(src => src.IpnameNumberNavigation));

            CreateMap<DataModels.Name, NameModel>()
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.AgencyId));

            CreateMap<WorkInfo, SubmissionModel>()
                .ForMember(dest => dest.Iswc, opt => opt.MapFrom(src => src.Iswc.Iswc1))
                .ForMember(dest => dest.WorkNumber, opt => opt.MapFrom(src =>
                    new Bdo.Work.WorkNumber { Number = src.AgencyWorkCode, Type = src.AgencyId }))
                .ForMember(dest => dest.SourceDb, opt => opt.MapFrom(src => src.SourceDatabase))
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.AgencyId))
                .ForMember(dest => dest.InterestedParties, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<InterestedPartyModel>();
                    list.AddRange(src.Creator?.Where(x => x.Status).Select(x => new InterestedPartyModel
                    {
                        CisacType = (CisacInterestedPartyType)x.RoleTypeId,
                        Type = InterestedPartyType.C,
                        IpBaseNumber = x.IpbaseNumber,
                        IPNameNumber = x.IpnameNumber,
                        IsExcludedFromIswc = x.IsExcludedFromIswc,
                        Name = x.IpnameNumberNavigation?.LastName + ' ' + x.IpnameNumberNavigation?.FirstName,
                        Names = new NameModel[] { new NameModel { IpNameNumber = x.IpnameNumberNavigation?.IpnameNumber ?? default } }
                    }));
                    list.AddRange(src.Publisher?.Where(x => x.Status).Select(x => new InterestedPartyModel
                    {
                        CisacType = (CisacInterestedPartyType)x.RoleTypeId,
                        Type = InterestedPartyType.E,
                        IpBaseNumber = x.IpbaseNumber,
                        IPNameNumber = x.IpnameNumber,
                        Name = x.IpnameNumberNavigation?.LastName + ' ' + x.IpnameNumberNavigation?.FirstName,
                        Names = new NameModel[] { new NameModel { IpNameNumber = x.IpnameNumberNavigation?.IpnameNumber ?? default } }
                    }));
                    return list;
                }))
                .ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Title.Where(x => x.Status)));

            CreateMap<Bdo.Work.Title, Services.Matching.MatchingEngine.Entities.Name>()
                .ForMember(dest => dest.Title, map => map.MapFrom(src => src.Name));

            CreateMap<InterestedPartyModel, WorkContributor>()
                .ForMember(dest => dest.IPIBaseNumber, opt => opt.MapFrom(src => src.IpBaseNumber))
                .ForMember(dest => dest.IPINumber, opt => opt.MapFrom(src => src.IPNameNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Type != null ? (ContributorRole)Enum.Parse(typeof(ContributorRole), src.Type.ToString()) : default))
                .ForMember(dest => dest.Name, opt => opt.MapFrom((src, dest) =>
                {
                    var name = src.Name;
                    if (string.IsNullOrEmpty(src.IpBaseNumber))
                        name = null;
                    return name;
                }));

            CreateMap<Bdo.Work.DisambiguateFrom, WorkNumber>()
                .ForMember(dest => dest.Number, map => map.MapFrom(src => src.Iswc ?? null))
                .ForMember(dest => dest.Type, map => map.MapFrom(src => (src.Iswc != null) ? "ISWC" : null));

            CreateMap<Bdo.Work.DerivedFrom, WorkNumber>()
                .ForMember(dest => dest.Number, map => map.MapFrom(src => src.Iswc ?? null))
                .ForMember(dest => dest.Type, map => map.MapFrom(src => (src.Iswc != null) ? "ISWC" : null));

            CreateMap<Submission, InputWorkInfo>()
                 .ForMember(dest => dest.Contributors, map => map.MapFrom((src, dest) =>
                 {
                     var ips = src.Model.InterestedParties.Where(x => x.IsWriter);

                     if (src.RulesApplied != null && src.RulesApplied.FirstOrDefault(x => x.RuleName == "MD_17")?.RuleConfiguration == "True")
                         return ips.Where(x => x.Names.FirstOrDefault()?.TypeCode != NameType.PG);
                     else
                         return ips;
                 }))
                 .ForMember(dest => dest.DisambiguateFromNumbers, map => map.MapFrom(src => src.Model.DisambiguateFrom))
                 .ForMember(dest => dest.WorkType, map => map.MapFrom(src => src.Model.DerivedWorkType))
                 .ForMember(dest => dest.Id, map => map.MapFrom(src => src.SubmissionId))
                 .ForMember(dest => dest.Source, map => map.MapFrom(src => "ISWC Database"))
                 .ForMember(dest => dest.Titles, map => map.MapFrom((src, dest) =>
                 {
                     var titles = new List<Services.Matching.MatchingEngine.Entities.Name>();
                     foreach (var title in src.Model.Titles)
                     {
                         var sanitisedName = StringExtensions.Sanitise(title.Name);
                         if (sanitisedName != title.Name)
                             titles.Add(new Services.Matching.MatchingEngine.Entities.Name { Title = sanitisedName, Type = title.Type.ToFriendlyString() });

                         titles.Add(new Services.Matching.MatchingEngine.Entities.Name { Title = title.Name, Type = title.Type.ToFriendlyString() });
                     }

                     return titles;

                 }))
                 .ForMember(dest => dest.Numbers, map => map.MapFrom((src, dest) =>
                 {
                     var list = new List<WorkNumber>();

                     if (!string.IsNullOrWhiteSpace(src.Model.Iswc) && !src.Model.AllowProvidedIswc) list.Add(new WorkNumber { Type = "ARCHIVED", Number = src.Model.Iswc });
                     if (!string.IsNullOrWhiteSpace(src.Model.PreferredIswc)) list.Add(new WorkNumber { Type = "ISWC", Number = src.Model.PreferredIswc });
                     if (!string.IsNullOrWhiteSpace(src.Model.WorkNumber?.Number)) list.Add(new WorkNumber { Type = src.Model.WorkNumber.Type, Number = src.Model.WorkNumber.Number });
                     else if (src.Model.DerivedFrom?.Count() > 0)
                     {
                         foreach (var iswc in src.Model.DerivedFrom)
                         {
                             list.Add(new WorkNumber { Type = "ISWC", Number = iswc.Iswc });
                         }
                     }
                     if (src.Model.IswcsToMerge?.Count() > 0)
                     {
                         foreach (var iswc in src.Model.IswcsToMerge)
                         {
                             list.Add(new WorkNumber { Type = "ISWC", Number = iswc });
                         }
                     }
                     if (src.Model.WorkNumbersToMerge?.Count() > 0)
                     {
                         foreach (var workNumber in src.Model.WorkNumbersToMerge)
                         {
                             if (!string.IsNullOrWhiteSpace(workNumber?.Number) && !string.IsNullOrWhiteSpace(workNumber?.Type))
                                 list.Add(new WorkNumber { Type = workNumber.Type, Number = workNumber.Number });
                         }
                     }
                     if (src.Model.AdditionalAgencyWorkNumbers.Any())
                     {
                         foreach (var iswc in src.Model.AdditionalAgencyWorkNumbers)
                         {
                             list.Add(new WorkNumber { Type = iswc.WorkNumber.Type, Number = iswc.WorkNumber.Number });
                         }
                     }
                     return list;
                 }))
                 .ForMember(dest => dest.IndependentWorkNumberMatch, map => map.MapFrom(src => src.TransactionType == TransactionType.FSQ))
                 .ForMember(dest => dest.SkipContributorCountRules, map => map.MapFrom(src => !new TransactionType[] { Bdo.Edi.TransactionType.CAR, TransactionType.FSQ }.Contains(src.TransactionType)
                 || src.Model.WorkNumber.Number == null))
                 .ForMember(dest => dest.ExcludeMatchesOTBelowSimilarity, map => map.MapFrom(src => new TransactionType[] { Bdo.Edi.TransactionType.CAR, Bdo.Edi.TransactionType.FSQ }.Contains(src.TransactionType)))
                 .ForMember(dest => dest.ExcludeInEligibleWorks, map => map.MapFrom(src => new TransactionType[] { Bdo.Edi.TransactionType.FSQ }.Contains(src.TransactionType)))
                 .ForMember(dest => dest.SkipStandardizedTitleSearch, map => map.MapFrom(src => src.IsPublicRequest || src.RequestType == RequestType.ThirdParty ? true : false));

            CreateMap<Submission, UsageWorkGroup>()
                .ForMember(dest => dest.ID, map => map.MapFrom(src => src.SubmissionId))
                .ForMember(dest => dest.Composers, map => map.MapFrom((src, dest) =>
                {
                    var ips = src.Model.InterestedParties.Where(x => x.IsWriter);
                    var composers = new List<Person>();

                    foreach (var ip in ips)
                    {
                        composers.Add(new Person() { Name = $"{ip.LastName} {ip.Name}".Trim() });
                    }

                    return composers;
                }))
                .ForMember(dest => dest.Performers, opt => opt.MapFrom((src, dest) =>
                {
                    var performers = new List<Person>();

                    foreach (var performer in src.Model.Performers)
                    {
                        performers.Add(new Person() { Name = $"{performer.LastName} {performer.FirstName}".Trim() });
                    }

                    return performers;
                }))
                .ForMember(dest => dest.Titles, map => map.MapFrom((src, dest) =>
                {
                    var titles = new List<Services.Matching.MatchingEngine.Entities.WorkName>();
                    foreach (var title in src.Model.Titles)
                    {
                        var sanitisedName = StringExtensions.Sanitise(title.Name);
                        if (sanitisedName != title.Name)
                            titles.Add(new Services.Matching.MatchingEngine.Entities.WorkName { Title = sanitisedName });

                        titles.Add(new Services.Matching.MatchingEngine.Entities.WorkName { Title = title.Name });
                    }

                    return titles;

                }));

            CreateMap<Title, Bdo.Work.Title>()
                .ForMember(dest => dest.Name, map => map.MapFrom(src => src.Title1))
                .ForMember(dest => dest.StandardizedName, map => map.MapFrom(src => src.StandardizedTitle))
                .ForMember(dest => dest.Type, map => map.MapFrom(src => src.TitleTypeId));

            CreateMap<Submission, WorkInfo>()
                .ForMember(dest => dest.Status, map => map.MapFrom(o => true))
                .ForMember(dest => dest.IswcId, map => map.Ignore())
                .ForMember(dest => dest.ArchivedIswc, map => map.MapFrom(src => src.Model.Iswc))
                .ForMember(dest => dest.Ipcount, map => map.MapFrom((src, dest) =>
                {
                    if (src.RulesApplied != null && src.RulesApplied.Any() && src.RulesApplied.FirstOrDefault(x => x.RuleName == "MD_17").RuleConfiguration == "True")
                        return src.Model.InterestedParties.Where(x => x.Names.FirstOrDefault()?.TypeCode != NameType.PG && x.CisacType != CisacInterestedPartyType.X).Count();
                    else
                        return src.Model.InterestedParties.Where(x => x.CisacType != CisacInterestedPartyType.X).Count();
                }))
                .ForMember(dest => dest.IsReplaced, map => map.MapFrom(o => false))
                .ForMember(dest => dest.WorkInfoId, map => map.Ignore())
                .ForMember(dest => dest.IswcEligible, map => map.MapFrom(src => src.IsEligible))
                .ForMember(dest => dest.MwiCategory, map => map.MapFrom(src => src.Model.Category))
                .ForMember(dest => dest.MatchTypeId, map => map.MapFrom(src => src.MatchedResult.Matches.FirstOrDefault().MatchType ?? null))
                .ForMember(dest => dest.AgencyId, map => map.MapFrom(src => src.Model.Agency))
                .ForMember(dest => dest.SourceDatabase, map => map.MapFrom(src => src.Model.SourceDb))
                .ForMember(dest => dest.AgencyWorkCode, map => map.MapFrom(src => src.Model.WorkNumber != null ? src.Model.WorkNumber.Number : null))
                .ForMember(dest => dest.DisambiguationReasonId, map => map.MapFrom(src => src.Model.DisambiguationReason ?? null))
                .ForMember(dest => dest.Bvltr, map => map.MapFrom(src => src.Model.BVLTR))
                .ForMember(dest => dest.DerivedWorkTypeId, map => map.MapFrom(src => src.Model.DerivedWorkType))
                .ForMember(dest => dest.Disambiguation, map => map.MapFrom(src => src.Model.Disambiguation))
                .ForMember(dest => dest.DisambiguationIswc, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<DisambiguationIswc>();
                    if (src.Model.DisambiguateFrom == null)
                        return list;

                    foreach (var disambiguateFrom in src.Model.DisambiguateFrom)
                    {
                        list.Add(
                        new DisambiguationIswc
                        {
                            Status = true,
                            LastModifiedUserId = dest.LastModifiedUserId,
                            Iswc = disambiguateFrom.Iswc,
                            LastModifiedDate = dest.LastModifiedDate,
                            CreatedDate = dest.CreatedDate

                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.Title, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Title>();
                    if (src.Model.Titles == null)
                        return list;

                    foreach (var title in src.Model.Titles)
                    {
                        list.Add(
                        new Title
                        {
                            Title1 = title.Name,
                            TitleTypeId = (int)title?.Type,
                            StandardizedTitle = title.StandardizedName,
                            Status = true,
                            IswcId = dest.IswcId,
                            WorkInfoId = dest.WorkInfoId,
                            LastModifiedDate = dest.LastModifiedDate,
                            CreatedDate = dest.CreatedDate,
                            LastModifiedUserId = dest.LastModifiedUserId
                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.DerivedFrom, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<DerivedFrom>();
                    if (src.Model.DerivedFrom == null)
                        return list;

                    foreach (var derivedFrom in src.Model.DerivedFrom)
                    {
                        list.Add(
                        new DerivedFrom
                        {
                            Status = true,
                            LastModifiedUserId = dest.LastModifiedUserId,
                            Iswc = derivedFrom.Iswc,
                            LastModifiedDate = dest.LastModifiedDate,
                            CreatedDate = dest.CreatedDate,
                            Title = derivedFrom.Title
                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Creator>();
                    if (src.Model.InterestedParties == null)
                        return list;

                    foreach (var ip in src.Model.InterestedParties.Where(c => c.CisacType == CisacInterestedPartyType.C ||
                    c.CisacType == CisacInterestedPartyType.MA || c.CisacType == CisacInterestedPartyType.TA))
                    {
                        list.Add(
                        new Creator
                        {
                            IpbaseNumber = ip.IpBaseNumber,
                            Status = true,
                            CreatedDate = dest.CreatedDate,
                            LastModifiedDate = dest.LastModifiedDate,
                            LastModifiedUserId = dest.LastModifiedUserId,
                            IsDispute = false,
                            Authoritative = ip.IsAuthoritative,
                            RoleTypeId = (int)ip.CisacType,
                            IswcId = dest.IswcId,
                            WorkInfoId = dest.WorkInfoId,
                            IpnameNumber = ip.IPNameNumber,
                            IsExcludedFromIswc = ip.IsExcludedFromIswc,
                            FirstName = ip.Name,
                            LastName = ip.LastName,
                            DisplayName = ip.DisplayName,
                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Publisher>();
                    if (src.Model.InterestedParties == null)
                        return list;

                    foreach (var p in src.Model.InterestedParties.Where(ip => ip.CisacType == CisacInterestedPartyType.E || ip.CisacType == CisacInterestedPartyType.AM))
                    {
                        list.Add(
                        new Publisher
                        {
                            IpbaseNumber = p.IpBaseNumber,
                            Status = true,
                            CreatedDate = dest.CreatedDate,
                            LastModifiedDate = dest.LastModifiedDate,
                            LastModifiedUserId = dest.LastModifiedUserId,
                            RoleTypeId = (int)p.CisacType,
                            IswcId = dest.IswcId,
                            WorkInfoId = dest.WorkInfoId,
                            IpnameNumber = p.IPNameNumber,
                            FirstName = p.Name,
                            LastName = p.LastName,
                            DisplayName = p.DisplayName
                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.WorkInfoInstrumentation, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<WorkInfoInstrumentation>();
                    if (src.Model.Instrumentation == null)
                        return list;

                    foreach (var i in src.Model?.Instrumentation)
                    {
                        list.Add(
                        new WorkInfoInstrumentation
                        {
                            InstrumentationId = int.Parse(i.Code),
                            Status = true,
                            CreatedDate = dest.CreatedDate,
                            LastModifiedDate = dest.LastModifiedDate,
                            LastModifiedUserId = dest.LastModifiedUserId
                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.WorkInfoPerformer, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<WorkInfoPerformer>();

                    foreach (var perf in src.Model.Performers)
                    {
                        list.Add(new WorkInfoPerformer()
                        {
                            Status = true,
                            CreatedDate = dest.CreatedDate,
                            LastModifiedDate = dest.LastModifiedDate,
                            LastModifiedUserId = dest.LastModifiedUserId,
                            PerformerDesignationId = !perf.Designation.IsNullOrEmpty() && perf.Designation == "Other" ? 2 : 1,
                            Performer = new DataModels.Performer()
                            {
                                LastName = perf.LastName,
                                FirstName = perf.FirstName,
                                LastModifiedDate = dest.LastModifiedDate,
                                CreatedDate = dest.CreatedDate,
                                LastModifiedUserId = dest.LastModifiedUserId,
                                Status = true,
                                Isni = perf.Isni,
                                Ipn = perf.Ipn
                            }
                        });
                    }
                    return list;
                }))
                .ForMember(dest => dest.AdditionalIdentifier, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<AdditionalIdentifier>();
                    if (src.Model.AdditionalIdentifiers == null)
                        return list;

                    list.AddRange(src.Model.AdditionalIdentifiers.Select(x => new AdditionalIdentifier
                    {
                        WorkIdentifier = x.WorkCode,
                        NumberTypeId = x.NumberTypeId ?? 1000,
                        Recording = x.RecordingTitle.IsNullOrEmpty() ? null : new Recording
                        {
                            RecordingTitle = x.RecordingTitle,
                            SubTitle = x.SubTitle,
                            LabelName = x.LabelName,
                            ReleaseEmbargoDate = x.ReleaseEmbargoDate != null ? (x.ReleaseEmbargoDate?.DateTime) : null,
                            RecordingArtist = x.Performers != null && x.Performers.Count() > 0 ? x.Performers.Select(perf => new RecordingArtist
                            {
                                Performer = new DataModels.Performer
                                {
                                    LastName = perf.LastName,
                                    FirstName = perf.FirstName,
                                    LastModifiedDate = dest.LastModifiedDate,
                                    CreatedDate = dest.CreatedDate,
                                    LastModifiedUserId = dest.LastModifiedUserId,
                                    Status = true,
                                    Isni = perf.Isni,
                                    Ipn = perf.Ipn
                                },
                                Status = true,
                                CreatedDate = dest.CreatedDate,
                                LastModifiedDate = dest.LastModifiedDate,
                                LastModifiedUserId = dest.LastModifiedUserId,
                                PerformerDesignationId = !perf.Designation.IsNullOrEmpty() && perf.Designation == "Other" ? 2 : 1
                            }).ToList() : null
                        }
                    })) ;

                    return list;
                })).ReverseMap();

            CreateMap<DataModels.Performer, Bdo.Work.Performer>().ReverseMap();

            CreateMap<WorkInfo, VerifiedSubmissionModel>()
                .ForMember(dest => dest.Iswc, opt => opt.MapFrom(src => src.Iswc.Iswc1))
                .ForMember(dest => dest.IswcStatus, opt => opt.MapFrom(src => src.Iswc.IswcStatusId == null ? "" : ((Bdo.Iswc.IswcStatus)src.Iswc.IswcStatusId).ToString()))
                .ForMember(dest => dest.ArchivedIswc, opt => opt.MapFrom(src => src.ArchivedIswc != src.Iswc.Iswc1 ? src.ArchivedIswc : default))
                .ForMember(dest => dest.WorkNumber, opt => opt.MapFrom(src =>
                    new Bdo.Work.WorkNumber { Number = src.AgencyWorkCode, Type = src.AgencyId }))
                .ForMember(dest => dest.SourceDb, opt => opt.MapFrom(src => src.SourceDatabase))
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.AgencyId))
                .ForMember(dest => dest.Titles, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Bdo.Work.Title>();

                    if (src.Title != null && src.Title.Any())
                    {
                        list.AddRange(src.Title.Where(x => x.Status).Select(x => new Bdo.Work.Title
                        {
                            TitleID = x.TitleId,
                            Name = x.Title1,
                            StandardizedName = x.StandardizedTitle,
                            Type = (Bdo.Work.TitleType)x.TitleTypeId
                        }).Distinct());
                    }
                    return list;
                }))
                .ForMember(dest => dest.DisambiguateFrom, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Bdo.Work.DisambiguateFrom>();

                    list.AddRange(src.DisambiguationIswc?.Where(x => x.Status).Select(x => new Bdo.Work.DisambiguateFrom
                    {
                        Iswc = x.Iswc
                    }));

                    return list;
                }))
                .ForMember(dest => dest.DisambiguationReason, opt => opt.MapFrom(src => src.DisambiguationReasonId))
                .ForMember(dest => dest.DerivedFrom, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Bdo.Work.DerivedFrom>();

                    list.AddRange(src.DerivedFrom?.Where(x => x.Status).Select(x => new Bdo.Work.DerivedFrom
                    {
                        Iswc = x.Iswc,
                        Title = x.Title
                    }));

                    return list;
                }))
                .ForMember(dest => dest.DerivedWorkType, opt => opt.MapFrom(src => src.DerivedWorkTypeId))
                .ForMember(dest => dest.Performers, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Bdo.Work.Performer>();

                    foreach(var workInfoPerformer in src.WorkInfoPerformer)
                    {
                        var performer = new Bdo.Work.Performer
                        {
                            PerformerID = (int?)workInfoPerformer.Performer.PerformerId,
                            Isni = workInfoPerformer.Performer.Isni,
                            Ipn = workInfoPerformer.Performer.Ipn,
                            FirstName = workInfoPerformer.Performer.FirstName,
                            LastName = workInfoPerformer.Performer.LastName,
                            Designation = workInfoPerformer.PerformerDesignationId != null ? ((Bdo.Work.PerformerDesignation)workInfoPerformer.PerformerDesignationId).ToDesignationString() : null
                        };

                        list.Add(performer);
                    }

                    return list;
                }))
                .ForMember(dest => dest.LastModifiedUser, opt => opt.MapFrom(src => src.LastModifiedUser.Name))
                .ForMember(dest => dest.InterestedParties, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<InterestedPartyModel>();
                    if (src.Creator != null && src.Creator.Any())
                    {
                        var membersOfPseudonymGroup = src.Creator?.Where(x => x.IpnameNumberNavigation != null && x.IpnameNumberNavigation.TypeCode == "PG");
                        bool isPseudonymGroupMember;

                        foreach (var creator in src.Creator.Where(x => x.Status))
                        {
                            isPseudonymGroupMember = membersOfPseudonymGroup.Any()
                           && membersOfPseudonymGroup.Any(x => x.IpnameNumberNavigation.NameReference.Any(y => y.IpbaseNumber == creator.IpbaseNumber) && creator.IpnameNumberNavigation?.TypeCode != "PG");

                            string lastName, name = null;
                            if (!string.IsNullOrEmpty(creator.IpbaseNumber))
                            {
                                lastName = creator.IpnameNumberNavigation?.LastName;
                                name = creator.IpnameNumberNavigation?.LastName + ' ' + creator.IpnameNumberNavigation?.FirstName;
                            }
                            else
                            {
                                lastName = creator?.LastName;
                                name = creator?.FirstName;
                            }

                            list.Add(new InterestedPartyModel
                            {
                                ContributorID = creator.IpnameNumber,
                                CisacType = (CisacInterestedPartyType)creator.RoleTypeId,
                                Type = InterestedPartyType.C,
                                IpBaseNumber = creator.IpbaseNumber,
                                IPNameNumber = creator.IpnameNumber,
                                LastName = lastName,
                                Name = name?.Trim(),
                                DisplayName = creator.DisplayName,
                                Names = new NameModel[] { new NameModel { IpNameNumber = creator.IpnameNumberNavigation?.IpnameNumber ?? default } },
                                IsAuthoritative = creator.Authoritative,
                                IsExcludedFromIswc = creator.IsExcludedFromIswc,
                                IsPseudonymGroupMember = isPseudonymGroupMember
                            });
                        }

                        if (list.Any(x => x.IsPseudonymGroupMember) && !src.Creator.Any(x => x.IpnameNumberNavigation?.TypeCode == "PG"))
                        {
                            var x = list.Where(y => y.IsPseudonymGroupMember
                            && src.Creator.Any(x => x.IpnameNumberNavigation.NameReference.Any(x => x.IpbaseNumber == y.IpBaseNumber && x.IpnameNumberNavigation.TypeCode == "PG")));

                            if (x.Any(x => list.Any(y => y.IPNameNumber == x.IPNameNumber)))
                            {
                                foreach (var ip in x.ToList())
                                {

                                    if (list.Any(x => x.IpBaseNumber == ip.IpBaseNumber && !x.IsPseudonymGroupMember))
                                        continue;

                                    var pseudonymGroupMember = membersOfPseudonymGroup.Select(x =>
                                           x.IpbaseNumberNavigation.NameReference.FirstOrDefault(x => x.IpnameNumberNavigation.TypeCode == "PG" && ip.IpBaseNumber == x.IpbaseNumber));

                                    var pg = pseudonymGroupMember.FirstOrDefault(y => y?.IpnameNumberNavigation != null)?.IpnameNumberNavigation;

                                    if (pg == null) continue;

                                    list.Add(new InterestedPartyModel
                                    {
                                        ContributorID = pg?.IpnameNumber,
                                        CisacType = ip?.CisacType,
                                        Type = InterestedPartyType.C,
                                        IpBaseNumber = ip?.IpBaseNumber,
                                        IPNameNumber = pg?.IpnameNumber,
                                        LastName = pg?.LastName,
                                        Name = pg?.LastName + ' ' + pg?.FirstName,
                                        Names = new NameModel[] { new NameModel { IpNameNumber = pg?.IpnameNumber ?? default } },
                                        DisplayName = ip?.DisplayName,
                                        IsAuthoritative = ip.IsAuthoritative,
                                        IsExcludedFromIswc = ip.IsExcludedFromIswc,
                                        IsPseudonymGroupMember = false
                                    });
                                }
                            }

                        }
                    }

                    if (src.Publisher != null && src.Publisher.Any())
                    {
                        var membersOfPseudonymGroup = src.Publisher?.Where(x => x.IpbaseNumberNavigation != null && x.IpbaseNumberNavigation.NameReference.Any(y => y.IpnameNumberNavigation.TypeCode == "PG"));
                        bool isPseudonymGroupMember;

                        foreach (var publisher in src.Publisher.Where(x => x.Status))
                        {
                            isPseudonymGroupMember = membersOfPseudonymGroup.Any()
                         && membersOfPseudonymGroup.Any(x => x.IpnameNumberNavigation.NameReference.Any(y => y.IpbaseNumber == publisher.IpbaseNumber) && publisher.IpnameNumberNavigation?.TypeCode != "PG");

                            string lastName, name = null;
                            if (!string.IsNullOrEmpty(publisher.IpbaseNumber))
                            {
                                lastName = publisher.IpnameNumberNavigation?.LastName;
                                name = publisher.IpnameNumberNavigation?.LastName + ' ' + publisher.IpnameNumberNavigation?.FirstName;
                            }
                            else
                            {
                                lastName = publisher?.LastName;
                                name = publisher?.FirstName;
                            }

                            list.Add(new InterestedPartyModel
                            {
                                ContributorID = publisher.IpnameNumber,
                                CisacType = (CisacInterestedPartyType)publisher.RoleTypeId,
                                Type = InterestedPartyType.C,
                                IpBaseNumber = publisher.IpbaseNumber,
                                IPNameNumber = publisher.IpnameNumber,
                                LastName = lastName,
                                Name = name?.Trim(),
                                DisplayName = publisher.DisplayName,
                                Names = new NameModel[] { new NameModel { IpNameNumber = publisher.IpnameNumberNavigation?.IpnameNumber ?? default } },
                                IsPseudonymGroupMember = isPseudonymGroupMember
                            });
                        }

                        if (list.Any(x => x.IsPseudonymGroupMember) && !src.Publisher.Any(x => x.IpnameNumberNavigation.TypeCode == "PG"))
                        {
                            var x = list.Where(y => y.IsPseudonymGroupMember
                            && src.Publisher.Any(x => x.IpnameNumberNavigation.NameReference.Any(x => x.IpbaseNumber == y.IpBaseNumber && x.IpnameNumberNavigation.TypeCode == "PG")));

                            if (x.Any(x => list.Any(y => y.IPNameNumber == x.IPNameNumber)))
                            {
                                foreach (var ip in x.ToList())
                                {
                                    if (list.Any(x => x.IpBaseNumber == ip.IpBaseNumber && !x.IsPseudonymGroupMember))
                                        break;

                                    var pseudonymGroupMember = membersOfPseudonymGroup.Select(x =>
                                           x.IpnameNumberNavigation.NameReference.FirstOrDefault(x => x.IpnameNumberNavigation.TypeCode == "PG" && ip.IpBaseNumber == x.IpbaseNumber));

                                    var pg = pseudonymGroupMember.FirstOrDefault().IpnameNumberNavigation;
                                    list.Add(new InterestedPartyModel
                                    {
                                        ContributorID = pg?.IpnameNumber,
                                        CisacType = ip?.CisacType,
                                        Type = InterestedPartyType.E,
                                        IpBaseNumber = ip?.IpBaseNumber,
                                        IPNameNumber = pg?.IpnameNumber,
                                        LastName = pg?.LastName,
                                        Name = pg?.LastName + ' ' + pg?.FirstName,
                                        Names = new NameModel[] { new NameModel { IpNameNumber = pg?.IpnameNumber ?? default } },
                                        DisplayName = ip?.DisplayName,
                                        IsAuthoritative = ip.IsAuthoritative,
                                        IsExcludedFromIswc = ip.IsExcludedFromIswc,
                                        IsPseudonymGroupMember = false
                                    });
                                }
                            }
                        }
                    }

                    return list;

                }))
                .ForMember(dest => dest.WorkInfoID, opt => opt.MapFrom(src => src.WorkInfoId))
                .ForMember(dest => dest.Instrumentation, opt => opt.MapFrom(src => src.WorkInfoInstrumentation.Select(x => x.Instrumentation)))
                .ForMember(dest => dest.AdditionalIdentifiers, opt => opt.MapFrom((src, dest) =>
                {

                    var list = new List<Bdo.Work.AdditionalIdentifier>();
                    if (src.AdditionalIdentifier != null)
                        list.AddRange(src.AdditionalIdentifier.Select(x => new Bdo.Work.AdditionalIdentifier
                        {
                            WorkCode = x?.WorkIdentifier,
                            SubmitterCode = x?.NumberType?.Code,
                            SubmitterDPID = x?.NumberType?.Code,
                            RecordingTitle = x?.Recording?.RecordingTitle,
                            SubTitle = x?.Recording?.SubTitle,
                            LabelName = x?.Recording?.LabelName,
                            ReleaseEmbargoDate = x?.Recording?.ReleaseEmbargoDate,
                            Performers = x?.Recording?.RecordingArtist != null && x.Recording.RecordingArtist.Count() > 0 ?
                                x.Recording.RecordingArtist.Select(p => new Bdo.Work.Performer
                                {
                                    FirstName = p.Performer.FirstName,
                                    LastName = p.Performer.LastName,
                                    Isni = p.Performer.Isni,
                                    Ipn = p.Performer.Ipn,
                                    Designation = ((Bdo.Work.PerformerDesignation)p.PerformerDesignationId).ToDesignationString()
                                }) : null
                        }));

                    return list;
                })).ReverseMap();

            CreateMap<MatchResult, Bdo.MatchingEngine.MatchResult>()
                .ForMember(dest => dest.StandardizedName, opt => opt.MapFrom(src => src.StandardizedTitle))
                .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches ?? null));

            CreateMap<MatchingWork, Bdo.MatchingEngine.MatchingWork>()
                .ForMember(dest => dest.Numbers, opt => opt.MapFrom(src => src.Numbers))
                .ForMember(dest => dest.StandardizedTitle, opt => opt.MapFrom(src => src.StandardizedTitle))
                .ForMember(dest => dest.IswcStatus, opt => opt.MapFrom(src => src.IswcStatusID))
                .ForMember(dest => dest.Titles, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Bdo.Work.Title> { };
                    list.AddRange(src.Titles.Select(x => new Bdo.Work.Title
                    {
                        Name = x.Title,
                        Type = !string.IsNullOrWhiteSpace(x.Type) ? (Bdo.Work.TitleType)Enum.Parse(typeof(Bdo.Work.TitleType), x.Type) : default,
                        StandardizedName = x.StandardizedTitle
                    }));


                    return list;
                }));

            CreateMap<MatchingResult, Bdo.MatchingEngine.MatchResult>()
                .ForMember(dest => dest.Matches, opt => opt.MapFrom((src, dest) =>
                {
                    var matchingWorks = new List<Bdo.MatchingEngine.MatchingWork> { };
                    matchingWorks.AddRange(src.MatchedEntities.Select(x => new Bdo.MatchingEngine.MatchingWork
                    {
                        Id = x.EntityID,
                        IsDefinite = x.IsDefinite,
                        RankScore = x.TotalQGRam
                    }));

                    return matchingWorks;
                }));

            CreateMap<Services.Matching.MatchingEngine.Entities.Name, Bdo.Work.Title>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));

            CreateMap<WorkContributor, Bdo.Ipi.InterestedPartyModel>()
                .ForMember(dest => dest.IpBaseNumber, opt => opt.MapFrom(src => src.IPIBaseNumber ?? default))
                .ForMember(dest => dest.IPNameNumber, opt => opt.MapFrom(src => src.IPINumber ?? default))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? default))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.IPICreatedDate ?? default))
                .ForMember(dest => dest.ContributorType, opt => opt.MapFrom(src => src.TypeCode ?? default))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) =>
                {
                    var x = Enum.TryParse(src.Role.ToString(), true, out InterestedPartyType role);
                    return x ? role : default;
                }));

            CreateMap<Services.Matching.MatchingEngine.Entities.Performer, Bdo.Work.Performer>();
            CreateMap<WorkNumber, Bdo.Work.WorkNumber>()
                .ForPath(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<SubmissionModel, IswclinkedTo>()
             .ForMember(dest => dest.LinkedToIswc, opt => opt.MapFrom(src => src.PreferredIswc))
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
             .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
             .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
             .ForMember(dest => dest.Iswc, opt => opt.Ignore());


            CreateMap<Instrumentation, Bdo.Work.Instrumentation>();

            CreateMap<WorkflowTask, Bdo.Work.WorkflowTask>()
              .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.WorkflowTaskId))
              .ForMember(dest => dest.WorkflowTaskType, opt => opt.MapFrom(src => src.WorkflowInstance.WorkflowType));

            CreateMap<WorkflowInstance, Bdo.Work.WorkflowInstance>();

            CreateMap<DataModels.Iswc, DataModels.Iswc>()
                .ForMember(dest => dest.Iswc1, opt => opt.MapFrom(src => src.Iswc1))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AgencyId, opt => opt.MapFrom(src => src.AgencyId))
                .ForMember(dest => dest.LastModifiedUserId, opt => opt.MapFrom(src => src.LastModifiedUserId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastModifiedDate));

            CreateMap<WorkNameContributorPerformerIndexModel, InterestedPartySearchModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PersonFullName))
                .ForMember(dest => dest.IpBaseNumber, opt => opt.MapFrom(src => src.IPIBaseNumber))
                .ForMember(dest => dest.IPNameNumber, opt => opt.MapFrom(src => src.IPINumber))
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.ContributorType, opt => opt.MapFrom(src => src.ContributorType))
                .ForMember(dest => dest.Affiliatiion, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AuditRequestModel, AuditHistoryResult>()
                        .ForMember(dest => dest.SubmittedDate, opt => opt.MapFrom(src => src.CreatedDate))
                        .ForMember(dest => dest.WorkNumber, opt => opt.MapFrom(src => src.Work.WorkNumber != null ? src.Work.WorkNumber.Number : string.Empty))
                        .ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Work.Titles))
                        .ForMember(dest => dest.SubmittingAgency, opt => opt.MapFrom(src => src.AgencyCode))
                        .ForMember(dest => dest.LastModifiedUser, opt => opt.MapFrom(src => src.RequestSource))
                        .ForMember(dest => dest.UpdateAllocatedIswc, opt => opt.MapFrom(src => src.UpdateAllocatedIswc))                       
                        .ForMember(dest => dest.Creators, opt => opt.MapFrom(src => src.Work.InterestedParties.Where(c => c.CisacType == CisacInterestedPartyType.C ||
                            c.CisacType == CisacInterestedPartyType.MA || c.CisacType == CisacInterestedPartyType.TA)))
                        .ForMember(dest => dest.WorkCode, opt => opt.MapFrom(src => src.WorkIdAfter));


            CreateMap<AuditRequestModel, AuditReportResult>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.AgencyWorkCode, opt => opt.MapFrom(src => src.Work != null && src.Work.WorkNumber != null ? src.Work.WorkNumber.Number : string.Empty))
                .ForMember(dest => dest.OriginalTitle, opt => opt.MapFrom(src => src.Work != null && src.Work.Titles.Any() ? src.Work.Titles.FirstOrDefault(x => x.Type.Equals(Bdo.Work.TitleType.OT)).Name : string.Empty))
                .ForMember(dest => dest.CreatorNames, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Work == null || !src.Work.InterestedParties.Any())
                        return string.Empty;


                    var creators = src.Work.InterestedParties.Where(c => c.CisacType == CisacInterestedPartyType.C ||
                      c.CisacType == CisacInterestedPartyType.MA || c.CisacType == CisacInterestedPartyType.TA);

                    if (!creators.Any())
                        return string.Empty;

                    foreach (var creator in creators)
                    {
                        if (string.IsNullOrEmpty(creator.Name))
                            creator.Name = creator.Names?.FirstOrDefault(x => x.IpNameNumber == creator.IPNameNumber)?.FirstName;
                        if (string.IsNullOrEmpty(creator.LastName))
                            creator.LastName = creator.Names?.FirstOrDefault(x => x.IpNameNumber == creator.IPNameNumber)?.LastName;
                    }

                    return StringExtensions.CreateSemiColonSeperatedString(creators.Select(c => $"{c.LastName} {c.Name}").ToArray());
                }))
                .ForMember(dest => dest.CreatorNameNumbers, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Work == null || !src.Work.InterestedParties.Any())
                        return string.Empty;


                    var creators = src.Work.InterestedParties.Where(c => c.CisacType == CisacInterestedPartyType.C ||
                      c.CisacType == CisacInterestedPartyType.MA || c.CisacType == CisacInterestedPartyType.TA);

                    if (!creators.Any())
                        return string.Empty;


                    return StringExtensions.CreateSemiColonSeperatedString(creators.Select(c => $"{c.IPNameNumber}").ToArray());
                }))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.TransactionError.Code.ToString()))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.TransactionError.Message))
                .ForMember(dest => dest.PreferredIswc, opt => opt.MapFrom(src => src.Work != null ? src.Work.PreferredIswc : string.Empty))
                .ForMember(dest => dest.TransactionSource, opt => opt.MapFrom(src => src.Work != null? src.TransactionSource : TransactionSource.Agency))
                .ForMember(dest => dest.SourceDb, opt => opt.MapFrom(src => src.Work != null ? src.Work.SourceDb : 0))
                .ForMember(dest => dest.PublisherNameNumber, opt => opt.MapFrom(
                    (src, dest) => src.Work != null && src.Work.AdditionalIdentifiers != null && src.Work.AdditionalIdentifiers.Any() ? src.Work.AdditionalIdentifiers.FirstOrDefault(x => x.NameNumber != null).NameNumber : null))
                .ForMember(dest => dest.PublisherWorkNumber, opt => opt.MapFrom(src => src.Work != null && src.Work.AdditionalIdentifiers.Any() ? src.Work.AdditionalIdentifiers.FirstOrDefault(x => x.NameNumber != null).WorkCode : string.Empty));

            CreateMap<DerivedFrom, Bdo.Work.DerivedFrom>().ReverseMap();

            CreateMap<SubmissionModel, VerifiedSubmissionModel>();

            CreateMap<Contributor, InterestedPartyModel>()
                .ForMember(dest => dest.IPNameNumber, opt => opt.MapFrom(src => src.IPINumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.LastName} {src.FirstName}"))
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.LegalEntityType, opt => opt.MapFrom(src => src.LegalEntityType != null ? (LegalEntityType)Enum.Parse(typeof(LegalEntityType), src.LegalEntityType.ToString()) : default));

            CreateMap<IswcModel, ISWCMetadataModel>()
                .ForMember(dest => dest.InterestedParties, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<InterestedPartyModel>();

                    if (src.VerifiedSubmissions.Where(x => x.IswcEligible == true).Any() && src.VerifiedSubmissions.Where(x => x.IswcEligible == true).Any(x => x?.InterestedParties != null))
                    {
                        var ipTuples = new List<Tuple<InterestedPartyModel, DateTime?>>();

                        foreach (var x in src.VerifiedSubmissions.Where(x => x.IswcEligible == true))
                            foreach (var y in x.InterestedParties)
                                ipTuples.Add(Tuple.Create(y, x.CreatedDate));

                        var ipsGroups = ipTuples.GroupBy(x => x.Item1.IpBaseNumber);

                        foreach (var group in ipsGroups)
                        {
                            if (group.Count() == 1) list.Add(group.First().Item1);
                            else if (group.Count() > 1)
                            {
                                var orderedIps = group.OrderByDescending(d => d.Item2);
                                var authIp = orderedIps.FirstOrDefault(x => x.Item1.IsAuthoritative == true);

                                if (authIp != null)
                                    list.Add(authIp.Item1);
                                else
                                    list.Add(orderedIps.First().Item1);
                            }
                        };

                    }
                    return list;
                }))
                .ForMember(dest => dest.Titles, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Bdo.Work.Title>();

                    if (src.VerifiedSubmissions.Any() && src.VerifiedSubmissions.Any(x => x?.Titles != null))
                    {
                        foreach (var item in src.VerifiedSubmissions.SelectMany(x => x.Titles))
                        {
                            if (!list.Any(l => l.Name == item.Name && l.Type == item.Type))
                                list.Add(item);
                        }
                    }
                    return list;
                }));

            CreateMap<FileAuditModel, FileAuditReportResult>()
                .ForMember(dest => dest.PublisherNameNumber, opt => opt.MapFrom(src => src.SubmittingPublisherIPNameNumber));

            CreateMap<AgencyStatisticsModel, AgencyStatisticsResult>();

            CreateMap<AuditReportSearchParameters, NotebookParameters>()
                .ForMember(dest => dest.AgencyCode, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.AgencyName) ? src.AgencyName : string.Empty))
                .ForMember(dest => dest.AgencyWorkCode, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.AgencyWorkCode) ? src.AgencyWorkCode : string.Empty))
                .ForMember(dest => dest.SubmittingAgencyCode, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.SubmittingAgency) ? src.SubmittingAgency : string.Empty))
                .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.Report.ToFriendlyString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToFriendlyString()))
                .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate == default ? string.Empty : src.FromDate.ToString()))
                .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate == default ? string.Empty : src.ToDate.ToString()))
                .ForMember(dest => dest.TransactionSource, opt => opt.MapFrom(src => src.TransactionSource.ToFriendlyString()))
                .ForMember(dest => dest.AgreementFromDate, opt => opt.MapFrom(src => src.AgreementFromDate == default ? string.Empty : src.AgreementFromDate.ToString()))
                .ForMember(dest => dest.AgreementToDate, opt => opt.MapFrom(src => src.AgreementToDate == default ? string.Empty : src.AgreementToDate.ToString()))
                .ForMember(dest => dest.MostRecentVersion, opt => opt.MapFrom(src => src.MostRecentVersion != null ? src.MostRecentVersion.ToString() : string.Empty))
                .ForMember(dest => dest.ConsiderOriginalTitlesOnly, opt => opt.MapFrom(src => src.ConsiderOriginalTitlesOnly != null ? src.ConsiderOriginalTitlesOnly.ToString() : string.Empty))
                .ForMember(dest => dest.PotentialDuplicatesCreateExtractMode, opt => opt.MapFrom(src => src.PotentialDuplicatesCreateExtractMode != null ? src.PotentialDuplicatesCreateExtractMode.ToString() : string.Empty))
                .ForMember(dest => dest.CreatorNameNumber, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.CreatorNameNumber) ? src.CreatorNameNumber : string.Empty))
                .ForMember(dest => dest.CreatorBaseNumber, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.CreatorBaseNumber) ? src.CreatorBaseNumber : string.Empty));

            CreateMap<List<DataModels.WorkInfo>, List<DataModels.WorkInfo>>();

            CreateMap<DataModels.WorkInfo, DataModels.WorkInfo>()
                .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                .ForMember(dest => dest.Iswc, opt => opt.Ignore())
                .ForMember(dest => dest.ArchivedIswc, opt => opt.MapFrom(src => src.ArchivedIswc));

            CreateMap<DataModels.Title, DataModels.Title>()
                        .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                        .ForMember(dest => dest.Iswc, opt => opt.Ignore())
                        .ForMember(dest => dest.TitleId, opt => opt.Ignore());

            CreateMap<DataModels.Creator, DataModels.Creator>()
                        .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                        .ForMember(dest => dest.Iswc, opt => opt.Ignore())
                        .ForMember(dest => dest.WorkInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.CreatorId, opt => opt.Ignore());

            CreateMap<DataModels.Publisher, DataModels.Publisher>()
                        .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                        .ForMember(dest => dest.Iswc, opt => opt.Ignore())
                        .ForMember(dest => dest.WorkInfo, opt => opt.Ignore())
                        .ForMember(dest => dest.PublisherId, opt => opt.Ignore());

            CreateMap<DataModels.WorkInfoInstrumentation, WorkInfoInstrumentation>()
                       .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                       .ForMember(dest => dest.WorkInfo, opt => opt.Ignore());

            CreateMap<DataModels.DisambiguationIswc, DisambiguationIswc>()
                       .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                       .ForMember(dest => dest.WorkInfo, opt => opt.Ignore())
                       .ForMember(dest => dest.DisambiguationIswcId, opt => opt.Ignore());

            CreateMap<DataModels.WorkInfoPerformer, DataModels.WorkInfoPerformer>()
                       .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                       .ForMember(dest => dest.WorkInfo, opt => opt.Ignore());

            CreateMap<DataModels.AdditionalIdentifier, DataModels.AdditionalIdentifier>()
                       .ForMember(dest => dest.WorkInfoId, opt => opt.Ignore())
                       .ForMember(dest => dest.WorkInfo, opt => opt.Ignore())
                       .ForMember(dest => dest.AdditionalIdentifierId, opt => opt.Ignore());

            CreateMap<DataModels.Recording, DataModels.Recording>()
                        .ForMember(dest => dest.AdditionalIdentifierId, opt => opt.Ignore())
                        .ForMember(dest => dest.AdditionalIdentifier, opt => opt.Ignore())
                        .ForMember(dest => dest.RecordingId, opt => opt.Ignore());

            CreateMap<DataModels.RecordingArtist, DataModels.RecordingArtist>()
                        .ForMember(dest => dest.RecordingId, opt => opt.Ignore())
                        .ForMember(dest => dest.Recording, opt => opt.Ignore());

            CreateMap<List<Bdo.Portal.WebUser>, List<Data.DataModels.WebUser>>();

            CreateMap<Bdo.Portal.WebUser, WebUser>()
                .ForMember(dest => dest.WebUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Agency, opt => opt.Ignore())
                .ForMember(dest => dest.AgencyId, opt => opt.MapFrom(src => src.AgencyId))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.WebUserRole, opt => opt.MapFrom(src => src.WebUserRoles));

            CreateMap<WebUser, Bdo.Portal.WebUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.AgencyId, opt => opt.MapFrom(src => src.AgencyId))
                .ForMember(dest => dest.WebUserRoles, opt => opt.MapFrom(src => src.WebUserRole.Where(x => x.Status)));

            CreateMap<ICollection<Bdo.Portal.WebUserRole>, ICollection<WebUserRole>>();

            CreateMap<Bdo.Portal.WebUserRole, WebUserRole>()
                .ForMember(dest => dest.WebUserRoleId, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.WebUserId, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => (int)src.Role))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.WebUser, opt => opt.Ignore())
               .ForMember(dest => dest.Notification, opt => opt.MapFrom((src, dest) =>
               {

                   var list = new List<Notification>();
                   if (string.IsNullOrWhiteSpace(src.Notification.Message))
                       list.Add(new Notification
                       {
                           Message = src.Notification.Message
                       });

                   return list;

               }));

            CreateMap<WebUserRole, Bdo.Portal.WebUserRole>()
                .ForMember(dest => dest.RequestedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (Bdo.Portal.PortalRoleType)src.RoleId))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Notification, opt => opt.MapFrom(src => src.Notification.FirstOrDefault(x => x.Status)));

            CreateMap<Bdo.Portal.Notification, Notification>()
                 .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                 .ForMember(dest => dest.NotificationId, opt => opt.Ignore())
                 .ForMember(dest => dest.NotificationType, opt => opt.Ignore())
                 .ForMember(dest => dest.WebUserRole, opt => opt.Ignore())
                 .ForMember(dest => dest.WebUserRoleId, opt => opt.Ignore())
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => true))
                 .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Notification, Bdo.Portal.Notification>()
                 .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));

            CreateMap<Message, Bdo.Portal.Message>()
                .ForMember(dest => dest.MessageHeader, opt => opt.MapFrom(src => src.Header));

            CreateMap<SubmissionChecksums, SubmissionChecksum>();

            CreateMap<SubmissionChecksum, SubmissionChecksums>()
                .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => src.ID));

            CreateMap<CsnNotifications, Bdo.Agent.CsnNotification>()
                .ForMember(dest => dest.IswcMetaData, opt => opt.MapFrom(src => src.HttpResponse));

            CreateMap<AgentRun, AgentRuns>()
                .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => src.RunId));

            CreateMap<CosmosDb.CacheIswcsModel, Submission>()
                .ForMember(dest => dest.SearchedIswcModels, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<IswcModel>();
                    var linkedIswcList = new List<IswcModel>();
                    var overallParentList = new List<IswcModel>();
                    var verfiedSubmissionsList = new List<VerifiedSubmissionModel>();
                    var Titles = new List<Bdo.Work.Title>();

                    var interestedPartList = new List<InterestedPartyModel>();


                    src.IswcMetadata.OtherTitles.ForEach(x => Titles.Add(new Bdo.Work.Title { Name = x.Title1, Type = (Bdo.Work.TitleType)Enum.Parse(typeof(Bdo.Work.TitleType), x.Type, true) }));

                    Titles.Add(new Bdo.Work.Title { Name = src.IswcMetadata.OriginalTitle, Type = Bdo.Work.TitleType.OT });

                    src.IswcMetadata.InterestedParties.ForEach(i => interestedPartList.Add(new InterestedPartyModel
                    {
                        Name = i.Name,
                        Type = i.Role.ToString().StringToEnum<InterestedPartyType>(),
                        CisacType = i.Role.ToString().StringToEnum<CisacInterestedPartyType>(),
                        IPNameNumber = i.IPNameNumber,
                        LastName = i.LastName,
                        LegalEntityType = i.LegalEntityType,
                        Affiliation = i.Affiliation

                    }));

                    var now = DateTimeOffset.UtcNow;

                    List<Bdo.Work.AdditionalIdentifier> additionalIdentifiers = new List<Bdo.Work.AdditionalIdentifier>();
                    if (src.IswcMetadata.Recordings != null)
                    {
                        foreach (var recording in src.IswcMetadata.Recordings)
                        {
                            if (recording.ReleaseEmbargoDate == null || (recording.ReleaseEmbargoDate != null && recording.ReleaseEmbargoDate <= now))
                            {
                                additionalIdentifiers.Add(new Bdo.Work.AdditionalIdentifier
                                {
                                    WorkCode = recording.Isrc,
                                    RecordingTitle = recording.RecordingTitle,
                                    SubTitle = recording.SubTitle,
                                    LabelName = recording.LabelName,
                                    ReleaseEmbargoDate = (recording.ReleaseEmbargoDate?.DateTime).GetValueOrDefault(),
                                    Performers = recording.Performers != null && recording.Performers.Count() > 0 ? recording.Performers.Select(p => new Bdo.Work.Performer
                                    {
                                        FirstName = p.FirstName,
                                        LastName = p.LastName,
                                        Isni = p.Isni,
                                        Ipn = (int?)p.Ipn,
                                        Designation = p.Designation
                                    }).ToList() : null
                                });
                            }
                        }
                    }

                    verfiedSubmissionsList.Add(new VerifiedSubmissionModel
                    {
                        Titles = Titles,
                        InterestedParties = interestedPartList,
                        IswcEligible = true,
                        AdditionalIdentifiers = additionalIdentifiers
                    });

                    if (src.IswcMetadata.LinkedISWC != null)
                    {
                        src.IswcMetadata.LinkedISWC.ForEach(x => linkedIswcList.Add(new IswcModel { Iswc = x }));

                    }

                    list.Add(new IswcModel
                    {
                        Agency = src.IswcMetadata.Agency,
                        Iswc = src.IswcMetadata.Iswc,
                        IswcStatusId = Enum.TryParse(src.IswcMetadata.IswcStatus, out Bdo.Iswc.IswcStatus iswcStatus) ? (int) iswcStatus : 1,
                        LinkedIswc = linkedIswcList,
                        CreatedDate = src.IswcMetadata.CreatedDate.Value.UtcDateTime,
                        LastModifiedDate = src.IswcMetadata.LastModifiedDate.Value.UtcDateTime,
                        OverallParentIswc = new IswcModel { Iswc = src.IswcMetadata.OverallParentISWC },
                        ParentIswc = new IswcModel { Iswc = src.IswcMetadata.ParentISWC },
                        VerifiedSubmissions = verfiedSubmissionsList
                    });
                    return list;
                }));
        }
    }
}