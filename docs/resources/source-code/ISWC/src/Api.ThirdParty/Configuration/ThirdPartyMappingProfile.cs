using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.Configuration
{
    internal class ThirdPartyMappingProfile : Profile
    {
        public ThirdPartyMappingProfile()
        {
            CreateMap<V1.DisambiguateFrom, DisambiguateFrom>().ReverseMap();
            CreateMap<V1.Performer, Performer>().ReverseMap();
            CreateMap<V1.Instrumentation, Instrumentation>().ReverseMap();


            CreateMap<V1.InterestedParty, InterestedPartyModel>()
                .ForMember(dest => dest.Names, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<NameModel>();
                    if (src.NameNumber != null)
                        list.Add(new NameModel { IpNameNumber = (int)src.NameNumber, LastName = src.Name });
                    return list;
                }))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => 
                { 
                    if (src.Role != null) 
                        return src.Role.ToString().StringToEnum<InterestedPartyType>(); 
                    return InterestedPartyType.C; 
                }))
                .ForMember(dest => dest.IPNameNumber, opt => opt.MapFrom(src => src.NameNumber))
                .ForMember(dest => dest.LegalEntityType, opt => opt.Ignore());


            CreateMap<IswcModel, V1.ISWCMetadata>()
                .ForMember(dest => dest.OriginalTitle, opt => opt.MapFrom((src, dest) =>
                {
                    var title = src.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate).Where(x => x.IswcEligible && x.IswcStatus == "Preferred" && !x.WorkNumber.Number.StartsWith("PRS_"))?
                    .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                    if (string.IsNullOrWhiteSpace(title)) title = src.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate)?.Where(x => x.IswcStatus == "Preferred" && !x.WorkNumber.Number.StartsWith("PRS_"))?
                    .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                    if (string.IsNullOrWhiteSpace(title)) title = src.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate)?
                    .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                    if (string.IsNullOrWhiteSpace(title)) title = "No OT title found.";

                    return title;
                }))
                .ForMember(dest => dest.OtherTitles, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Title>();

                    if (src.VerifiedSubmissions.Any() && src.VerifiedSubmissions.Any(x => x?.Titles != null))
                    {
                        if (src.IswcStatusId.HasValue && (IswcStatus)src.IswcStatusId == IswcStatus.Preferred)
                        {
                            foreach (var item in src.VerifiedSubmissions.Where(x => !x.WorkNumber.Number.StartsWith("PRS_")).SelectMany(x => x.Titles).Where(t => t.Type != TitleType.OT))
                            {
                                if (!list.Any(l => l.Name == item.Name && l.Type == item.Type))
                                    list.Add(item);
                            }
                        }
                        else if (src.IswcStatusId.HasValue && (IswcStatus)src.IswcStatusId == IswcStatus.Provisional)
                        {
                            foreach (var item in src.VerifiedSubmissions.SelectMany(x => x.Titles).Where(t => t.Type != TitleType.OT))
                            {
                                if (!list.Any(l => l.Name == item.Name && l.Type == item.Type))
                                    list.Add(item);
                            }
                        }
                    }
                    return list;
                }))
                .ForMember(dest => dest.InterestedParties, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<InterestedPartyModel>();

                    if (src.VerifiedSubmissions.Where(x => x.IswcEligible == true).Any() && src.VerifiedSubmissions.Where(x => x.IswcEligible == true).Any(x => x?.InterestedParties != null))
                    {
                        var ipTuples = new List<Tuple<InterestedPartyModel, DateTime?>>();

                        bool isPreferred = src.IswcStatusId.HasValue && (IswcStatus)src.IswcStatusId == IswcStatus.Preferred;
                        var verifiedSubmissions = (isPreferred) ?
                                                src.VerifiedSubmissions.Where(x => x.IswcEligible == true && !x.WorkNumber.Number.StartsWith("PRS_")) :
                                                src.VerifiedSubmissions.Where(x => x.IswcEligible == true);

                        foreach (var x in verifiedSubmissions)
                            foreach (var y in x.InterestedParties)
                            {
                                if (!y.IsExcludedFromIswc && !y.IsPseudonymGroupMember)
                                    ipTuples.Add(Tuple.Create(y, x.CreatedDate));
                            }

                        var ipsGroups = ipTuples.GroupBy(x => !string.IsNullOrWhiteSpace(x.Item1.IpBaseNumber) ? x.Item1.IpBaseNumber : x.Item1.IPNameNumber?.ToString());
                        if (!isPreferred)
                            ipsGroups = ipTuples.GroupBy(x => $"{x.Item1.Name} {x.Item1.LastName}");

                        foreach (var group in ipsGroups)
                        {
                            if (group.Count() == 1) list.Add(group.First().Item1);
                            else if (group.Count() > 1)
                            {
                                var orderedIps = group.OrderByDescending(d => d.Item2);
                                var authIp = orderedIps.FirstOrDefault(x => x.Item1.IsAuthoritative == true);

                                if (src.VerifiedSubmissions.Any(x => x.IsPublicRequest))
                                {
                                    if (authIp != null)
                                        authIp.Item1.IpBaseNumber = string.Empty;
                                    else
                                        orderedIps.First().Item1.IpBaseNumber = string.Empty;
                                }
                                if (authIp != null)
                                    list.Add(authIp.Item1);
                                else
                                    list.Add(orderedIps.First().Item1);
                            }
                        };

                    }
                    return list;
                }))
                .ForMember(dest => dest.Recordings, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<V1.Recordings>();

                    if (src.VerifiedSubmissions.Any() && src.VerifiedSubmissions.Any(x => x?.AdditionalIdentifiers != null))
                    {
                        foreach (var ai in src.VerifiedSubmissions.SelectMany(x => x.AdditionalIdentifiers))
                        {
                            if (!string.IsNullOrEmpty(ai.RecordingTitle))
                            {
                                list.Add(new V1.Recordings
                                {
                                    Isrc = ai.WorkCode,
                                    RecordingTitle = ai.RecordingTitle,
                                    SubTitle = ai.SubTitle,
                                    LabelName = ai.LabelName,
                                    ReleaseEmbargoDate = ai.ReleaseEmbargoDate,
                                    Performers = ai.Performers != null && ai.Performers.Count() > 0 ? ai.Performers.Select(p => new V1.Performer {
                                        Isni = p.Isni,
                                        Ipn = p.Ipn,
                                        FirstName = p.FirstName,
                                        LastName = p.LastName,
                                        Designation = p.Designation?.ToString().StringToEnum<V1.PerformerDesignation>()
                                    }).ToList() : null
                                });
                            }
                        }
                    }
                    return list;
                }))
                .ForMember(dest => dest.IswcStatus, opt => opt.MapFrom(src => src.IswcStatusId == null ? "" : ((IswcStatus)src.IswcStatusId).ToString()))
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.Agency))
                .ForMember(dest => dest.LastModifiedBy, opt => opt.MapFrom(src => src.LastModifiedUser))
                .ForMember(dest => dest.Works, opt => opt.MapFrom(src => src.VerifiedSubmissions))
                .ForMember(dest => dest.LinkedISWC, opt => opt.MapFrom(src => src.LinkedIswc.Any() ? src.LinkedIswc.GroupBy(x => x.Iswc).Select(x => x.Key).ToList() : src.LinkedIswc.Select(x => x.Iswc)))
                .ForMember(dest => dest.ParentISWC, opt => opt.MapFrom(src => src.ParentIswc!.Iswc))
                .ForMember(dest => dest.OverallParentISWC, opt => opt.MapFrom(src => src.OverallParentIswc!.Iswc))
                .AfterMap((src, dest) =>
                {
                    foreach (var work in dest.Works)
                        work.OverallParentISWC = src.OverallParentIswc?.Iswc;
                });

            CreateMap<Title, V1.Title>()
                .ForMember(dest => dest.Title1, opt => opt.MapFrom(src => src.Name)).ReverseMap();

            CreateMap<V1.DerivedFrom, DerivedFrom>().ReverseMap();

            CreateMap<VerifiedSubmissionModel, V1.VerifiedSubmission>()
                .ForMember(dest => dest.OriginalTitle, opt => opt.MapFrom((src, dest) =>
                {
                    var title = src.Titles.Where(x => x.Type == TitleType.OT).FirstOrDefault()?.Name;
                    if (string.IsNullOrWhiteSpace(title)) title = "No OT title found.";
                    return title;
                }))
                .ForMember(dest => dest.Performers, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<Performer>();

                    if (src.Performers != null && src.Performers.Any())
                    {
                        foreach (var item in src.Performers)
                        {
                            if (!list.Any(l => l.FirstName == item.FirstName && l.LastName == item.LastName))
                                list.Add(item);
                        }
                    }
                    return list;
                }))
                .ForMember(dest => dest.OtherTitles, opt => opt.MapFrom(src => src.Titles.Where(x => x.Type != TitleType.OT)))
                .ForMember(dest => dest.LastModifiedBy, opt => opt.MapFrom(src => src.LastModifiedUser))
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.Agency))
                .ForMember(dest => dest.DerivedFromIswcs, opt => opt.MapFrom(src => src.DerivedFrom ?? default))
                .ForMember(dest => dest.Workcode, opt => opt.MapFrom(src => src.WorkNumber != null ? src.WorkNumber.Number : default))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WorkInfoID))
                .ForMember(dest => dest.DisambiguateFrom, opt => opt.MapFrom(src => src.DisambiguateFrom ?? default))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WorkInfoID))
                .ForMember(dest => dest.AdditionalIdentifiers, opt => opt.MapFrom((src, dest) =>
                {
                    var isrcs = new List<string>();
                    var publisherIdentifier = new List<V1.PublisherIdentifiers> { };
                    var additionalWorkcodes = new List<V1.AgencyWorkCodes> { };
                    var labelIdentifier = new List<V1.LabelIdentifiers> { };

                    if (src.AdditionalIdentifiers != null && src.AdditionalIdentifiers.Any())
                    {
                        foreach (var additionalIdentifier in src.AdditionalIdentifiers.Where(x => x?.SubmitterCode == "ISRC"))
                        {
                            if (additionalIdentifier.WorkCode != null)
                            {
                                if (additionalIdentifier.SubmitterCode == "ISRC")
                                    isrcs.Add(additionalIdentifier.WorkCode);
                            }
                        }

                        if (src.AdditionalIdentifiers.Any(x => x?.SubmitterCode != "ISRC" && x?.NameNumber != null))
                        {
                            var results = src.AdditionalIdentifiers.Where(x => x?.SubmitterCode != "ISRC" && x?.NameNumber != null).GroupBy(p => p.NameNumber, p => p,
                                (key, pubs) => new { sub = key, publishers = pubs.ToList() });

                            foreach (var result in results)
                            {
                                var pub = new V1.PublisherIdentifiers();
                                pub.NameNumber = result.publishers.FirstOrDefault()?.NameNumber;
                                pub.SubmitterCode = result.publishers.FirstOrDefault()?.SubmitterCode;

                                if (pub.WorkCode == null)
                                    pub.WorkCode = new List<string>();


                                pub.WorkCode.AddRange(result.publishers.Select(x => x.WorkCode));

                                publisherIdentifier.Add(pub);
                            }
                        }

                        if (src.AdditionalIdentifiers.Any(x => x?.SubmitterCode != "ISRC" && x?.SubmitterDPID != null && x?.NameNumber == null))
                        {
                            var results = src.AdditionalIdentifiers.Where(x => x?.SubmitterCode != "ISRC" && x?.SubmitterDPID != null).GroupBy(p => p.SubmitterDPID, p => p,
                                (key, labels) => new { sub = key, labels = labels.ToList() });

                            foreach (var result in results)
                            {
                                var li = new V1.LabelIdentifiers();
                                li.SubmitterDPID = result.labels.FirstOrDefault()?.SubmitterDPID;

                                if (li.WorkCode == null)
                                    li.WorkCode = new List<string>();


                                li.WorkCode.AddRange(result.labels.Select(x => x.WorkCode));

                                labelIdentifier.Add(li);
                            }
                        }
                    }

                    return new V1.AdditionalIdentifiers { Isrcs = isrcs, PublisherIdentifiers = publisherIdentifier, AgencyWorkCodes = additionalWorkcodes, LabelIdentifiers = labelIdentifier };
                }))
                .ForMember(dest => dest.InterestedParties, opt => opt.MapFrom((src, dest) =>
                {
                    var ips = new List<V1.InterestedParty>();

                    foreach (var ip in src.InterestedParties)
                    {
                        if (ip.IsPseudonymGroupMember) continue;

                        if (src.IsPublicRequest || src.WorkNumber.Number.StartsWith("PRS_"))
                            ip.IpBaseNumber = null;

                        ips.Add(new V1.InterestedParty
                        {
                            Role = ip.CisacType.ToString().StringToEnum<V1.InterestedPartyRole>(),
                            Affiliation = ip.Affiliation,
                            NameNumber = ip.IPNameNumber,
                            Name = ip.Name,
                            LastName = ip.LastName
                        });
                    }

                    return ips;

                }));

            CreateMap<Submission, V1.ISWCMetadataBatch>()
                .ForMember(dest => dest.SearchId, opt => opt.MapFrom(src => src.SubmissionId))
                .ForMember(dest => dest.Rejection, opt => opt.MapFrom(src => src.Rejection))
                .ForMember(dest => dest.SearchResults, opt => opt.MapFrom(src => src.SearchedIswcModels));

            CreateMap<Rejection, V1.Rejection>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.ToString().Replace("_", "") ?? string.Empty));


            CreateMap<V1.Rejection, Rejection>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => (Bdo.Rules.ErrorCode)Enum.Parse(typeof(Bdo.Rules.ErrorCode), src.Code)));

            CreateMap<InterestedPartyModel, V1.InterestedParty>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.CisacType.ToString().StringToEnum<V1.InterestedPartyRole>()))
                .ForMember(dest => dest.NameNumber, opt => opt.MapFrom(src => src.IPNameNumber))
                .ForMember(dest => dest.Affiliation, opt => opt.MapFrom(src => src.Affiliation))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.LegalEntityType, opt => opt.MapFrom(src => (int)src.LegalEntityType));
        }
    }
}
