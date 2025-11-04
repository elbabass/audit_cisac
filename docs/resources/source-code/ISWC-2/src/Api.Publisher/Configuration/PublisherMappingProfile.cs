using AutoMapper;
using IdentityServer4.Extensions;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Api.Publisher.Configuration
{
    internal class PublisherMappingProfile : Profile
    {
        public PublisherMappingProfile()
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

            CreateMap<WorkNumber, V1.WorkNumber>()
                .ForMember(dest => dest.AgencyCode, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.AgencyWorkCode, opt => opt.MapFrom(src => src.Number));

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

            CreateMap<Submission, V1.ISWCMetadataBatch>()
                .ForMember(dest => dest.SearchId, opt => opt.MapFrom(src => src.SubmissionId))
                .ForMember(dest => dest.Rejection, opt => opt.MapFrom(src => src.Rejection))
                .ForMember(dest => dest.SearchResults, opt => opt.MapFrom(src => src.SearchedIswcModels));

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
                .ForMember(dest => dest.IswcStatus, opt => opt.MapFrom(src => src.IswcStatus))
                .ForMember(dest => dest.DisambiguateFrom, opt => opt.MapFrom(src => src.DisambiguateFrom ?? default))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WorkInfoID))
                .ForMember(dest => dest.AdditionalIdentifiers, opt => opt.MapFrom((src, dest) =>
                {
                    var isrcs = new List<string>();
                    var publisherIdentifier = new List<V1.PublisherIdentifiers> { };
                    var additionalWorkcodes = new List<V1.AgencyWorkCodes> { };
                    var labelIdentifier = new List<V1.LabelIdentifiers> { };
                    var recordings = new List<V1.Recordings>() { };

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

                        if (src.AdditionalIdentifiers.Any(x => !x.RecordingTitle.IsNullOrEmpty()))
                        {
                            var results = src.AdditionalIdentifiers.Where(x => !x.RecordingTitle.IsNullOrEmpty());

                            foreach (var result in results)
                            {
                                recordings.Add(new V1.Recordings
                                {
                                    Isrc = result.WorkCode,
                                    RecordingTitle = result.RecordingTitle,
                                    SubTitle = result.SubTitle,
                                    LabelName = result.LabelName,
                                    ReleaseEmbargoDate = result.ReleaseEmbargoDate,
                                    Performers = result.Performers != null && result.Performers.Count() > 0 ? result.Performers.Select(x => new V1.Performer
                                    {
                                        Isni = x.Isni,
                                        Ipn = x.Ipn,
                                        FirstName = x.FirstName,
                                        LastName = x.LastName,
                                        Designation = x.Designation?.ToString().StringToEnum<V1.PerformerDesignation>()
                                    }).ToList() : null
                                });
                            }
                        }
                    }

                    return new V1.AdditionalIdentifiers
                    {
                        Isrcs = isrcs,
                        PublisherIdentifiers = publisherIdentifier,
                        AgencyWorkCodes = additionalWorkcodes,
                        LabelIdentifiers = labelIdentifier,
                        Recordings = recordings
                    };
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
                            LastName = ip.LastName,
                            DisplayName = ip.DisplayName
                        });
                    }

                    return ips;

                }));

            CreateMap<Submission, V1.SubmissionResponse>()
                .ForMember(dest => dest.LinkedIswcs, opt => opt.MapFrom(src => src.IswcModel!.LinkedIswc))
                .ForMember(dest => dest.VerifiedSubmission, opt => opt.MapFrom(
                    src => src.IswcModel!.VerifiedSubmissions.FirstOrDefault(x => x.WorkNumber != null && src.Model.WorkNumber != null && (x.WorkNumber.Number == src.Model.WorkNumber.Number || src.Model.AdditionalAgencyWorkNumbers.Any()))))
                .AfterMap((src, dest) =>
                {
                    var recordings = new List<V1.Recordings>();
                    var performers = new List<V1.Performer>();

                    if (src.IswcModel.VerifiedSubmissions.Any())
                    {
                        foreach (var sub in src.IswcModel.VerifiedSubmissions)
                        {
                            if (sub.AdditionalIdentifiers != null)
                            {
                                foreach (var ai in sub.AdditionalIdentifiers)
                                {
                                    if (ai.RecordingTitle != null)
                                    {
                                        recordings.Add(new V1.Recordings
                                        {
                                            Isrc = ai.WorkCode,
                                            RecordingTitle = ai.RecordingTitle,
                                            SubTitle = ai.SubTitle,
                                            LabelName = ai.LabelName,
                                            ReleaseEmbargoDate = ai.ReleaseEmbargoDate
                                        });
                                    }
                                }
                            }
                            if (sub.Performers != null)
                            {
                                foreach (var perf in sub.Performers)
                                {
                                    performers.Add(new V1.Performer
                                    {
                                        FirstName = perf.FirstName,
                                        LastName = perf.LastName,
                                        Designation = perf.Designation?.ToString().StringToEnum<V1.PerformerDesignation>(),
                                        Isni = perf.Isni,
                                        Ipn = perf.Ipn
                                    });
                                }
                            }

                            dest.VerifiedSubmission.AdditionalIdentifiers.Recordings = recordings;
                            dest.VerifiedSubmission.Performers = performers;
                        }
                    }
                })
                .ForMember(dest => dest.PotentialMatches, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<V1.ISWCMetadata>();

                    if (src.MatchedResult.Matches.Any() && src.DetailLevel != DetailLevel.Core)
                    {
                        foreach (var match in src.MatchedResult.Matches.OrderByDescending(x => x.Id).Where(x => x.Numbers.Any(n => n.Type == "ISWC")))
                        {
                            var iswc = match.Numbers.FirstOrDefault(n => n.Type == "ISWC")?.Number;

                            if (iswc != null && iswc != src.IswcModel.Iswc && !list.Any(n => n.Iswc == iswc))
                            {
                                list.Add(new V1.ISWCMetadata
                                {
                                    Iswc = iswc,
                                    OriginalTitle = match.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name ?? string.Empty
                                });
                            }
                        }
                    }
                    return list;
                }));

            CreateMap<IswcModel, V1.LinkedIswcs>()
                .ForMember(dest => dest.IswcMetadata, opt => opt.MapFrom(src => src));

            CreateMap<Submission, V1.VerifiedSubmissionBatch>()
                .ForMember(dest => dest.SubmissionId, opt => opt.MapFrom(src => src.SubmissionId))
                .ForMember(dest => dest.AlternateIswcMatches, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<V1.AlternateIswcMatch>();

                    if (src.HasAlternateIswcMatches)
                    {

                        var alternateMatches = src.Rejection?.Code == Bdo.Rules.ErrorCode._249
                                                    ? src.MatchedResult.Matches.Concat(src.IsrcMatchedResult.Matches).ToList()
                                                    : src.MatchedResult.Matches.Concat(src.IsrcMatchedResult.Matches.Skip(1)).ToList();

                        foreach (var match in alternateMatches)
                        {
                            var iswc = match.Numbers.FirstOrDefault(n => n.Type == "ISWC")?.Number;

                            if (iswc != null && iswc != src.Model.PreferredIswc)
                            {

                                var otherTitles = new List<V1.Title>();
                                var interestedParties = new List<V1.InterestedParty>();

                                foreach (var title in match.Titles)
                                {
                                    if (title.Type != TitleType.OT)
                                    {
                                        otherTitles.Add(new V1.Title
                                        {
                                            Title1 = title.Name,
                                            Type = title.Type.ToString().StringToEnum<V1.TitleType>()

                                        });
                                    }
                                }

                                foreach (var contributor in match.Contributors)
                                {
                                    interestedParties.Add(new V1.InterestedParty
                                    {
                                        Name = contributor.Name,
                                        LastName = contributor.LastName,
                                        DisplayName = contributor.DisplayName,
                                        NameNumber = contributor.IPNameNumber,
                                        Affiliation = contributor.Affiliation,
                                        Role = contributor.CisacType.ToString().StringToEnum<V1.InterestedPartyRole>(),
                                        LegalEntityType = contributor.LegalEntityType.ToString().StringToEnum<V1.InterestedPartyLegalEntityType>(),
                                    });
                                }



                                list.Add(new V1.AlternateIswcMatch
                                {
                                    Iswc = iswc,
                                    IswcStatus = match.IswcStatus == null ? "" : ((IswcStatus)match.IswcStatus).ToString(),
                                    Agency = src.Model.Agency,
                                    OriginalTitle = match.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name ?? string.Empty,
                                    OtherTitles = otherTitles,
                                    InterestedParties = interestedParties

                                });
                            }
                        }
                    }

                    return list.DistinctBy(x => x.Iswc).ToList();
                }))
                .ForPath(dest => dest.Submission.VerifiedSubmission,
                opt => opt.MapFrom(src => src.IswcModel.VerifiedSubmissions.FirstOrDefault(x => x.WorkNumber != null && src.Model.WorkNumber != null && x.WorkNumber.Number == src.Model.WorkNumber.Number)))
                .AfterMap((src, dest) =>
                {
                    var recordings = new List<V1.Recordings>();
                    var performers = new List<V1.Performer>();

                    if (src.IswcModel.VerifiedSubmissions.Any())
                    {
                        foreach (var sub in src.IswcModel.VerifiedSubmissions)
                        {
                            if (sub.AdditionalIdentifiers != null)
                            {
                                foreach (var ai in sub.AdditionalIdentifiers)
                                {
                                    if (ai.RecordingTitle != null)
                                    {
                                        recordings.Add(new V1.Recordings
                                        {
                                            Isrc = ai.WorkCode,
                                            RecordingTitle = ai.RecordingTitle,
                                            SubTitle = ai.SubTitle,
                                            LabelName = ai.LabelName,
                                            ReleaseEmbargoDate = ai.ReleaseEmbargoDate,
                                            Performers = ai.Performers != null && ai.Performers.Count() > 0 ? ai.Performers.Select(x => new V1.Performer
                                            {
                                                Isni = x.Isni,
                                                Ipn = x.Ipn,
                                                FirstName = x.FirstName,
                                                LastName = x.LastName,
                                                Designation = x.Designation?.ToString().StringToEnum<V1.PerformerDesignation>()
                                            }).ToList() : null
                                        });
                                    }
                                }
                            }
                            if (sub.Performers != null)
                            {
                                foreach (var perf in sub.Performers)
                                {
                                    performers.Add(new V1.Performer
                                    {
                                        FirstName = perf.FirstName,
                                        LastName = perf.LastName,
                                        Designation = perf.Designation?.ToString().StringToEnum<V1.PerformerDesignation>(),
                                        Isni = perf.Isni,
                                        Ipn = perf.Ipn
                                    });
                                }
                            }

                            dest.Submission.VerifiedSubmission.AdditionalIdentifiers.Recordings = recordings;
                            dest.Submission.VerifiedSubmission.Performers = performers;
                        }
                    }
                })
                .ForPath(dest => dest.Submission.LinkedIswcs, opt => opt.MapFrom(src => src.IswcModel != null ? src.IswcModel.LinkedIswc : default))
                .ForPath(dest => dest.Submission.MultipleAgencyWorkCodes, opt => opt.MapFrom(src => src.MultipleAgencyWorkCodes)).ReverseMap();



            CreateMap<MultipleAgencyWorkCodes, V1.MultipleAgencyWorkCodes>()
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.Agency))
                .ForMember(dest => dest.WorkCode, opt => opt.MapFrom(src => src.WorkCode));

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

            CreateMap<V1.WorkNumber, AdditionalAgencyWorkNumber>()
            .ForMember(dest => dest.WorkNumber, opt => opt.MapFrom(src => new WorkNumber
            {
                Type = src.AgencyCode,
                Number = src.AgencyWorkCode
            }));

            CreateMap<V1.Works, WorkModel>()
            .ForMember(dest => dest.PreferredIswc, opt => opt.MapFrom(src => src.Iswc))
            .ForPath(dest => dest.Titles, opt => opt.MapFrom(src => src.Titles))
            .ForPath(dest => dest.WorkNumbers, opt => opt.MapFrom(src => src.WorkCodes));

            CreateMap<V1.IPContextSearchModel, SubmissionModel>()
                .ForMember(dest => dest.InterestedParties, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<InterestedPartyModel>();

                    var ip = new InterestedPartyModel
                    {
                        IPNameNumber = src.NameNumber,
                        Name = $"{src.LastName} {src.FirstName}",
                        LastName = src.LastName,
                        BirthDate = src.DateOfBirth?.DateTime,
                        DeathDate = src.DateOfDeath?.DateTime,
                        Age = src.Age,
                        AgeTolerance = src.AgeTolerance ?? 0,
                        Type = InterestedPartyType.C,
                        Names = new List<NameModel>() { new NameModel() { FirstName = src.FirstName, LastName = src.LastName }  }
                    };

                    if (src.NameNumber.HasValue)
                    {
                        ip.Names = new List<NameModel>
                        {
                            new NameModel
                            {
                                IpNameNumber = src.NameNumber.Value,
                                FirstName = src.FirstName,
                                LastName = src.LastName
                            }
                        };
                    }

                    list.Add(ip);
                    return list;
                }))
                .ForMember(dest => dest.Affiliations, opt => opt.MapFrom(src =>
                    src.Affiliations != null
                        ? src.Affiliations
                            .Where(a => !string.IsNullOrWhiteSpace(a.Affiliation))
                            .Select(a => a.Affiliation.Trim())
                            .Distinct()
                            .ToList()
                        : new List<string>()
                ))
                .ForMember(dest => dest.AdditionalIdentifiers, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<AdditionalIdentifier>();

                    if (src.AdditionalIdentifiers != null)
                    {
                        if (src.AdditionalIdentifiers?.Isrcs != null)
                            list.AddRange(src.AdditionalIdentifiers.Isrcs.Select(x => new AdditionalIdentifier
                            {
                                SubmitterCode = "ISRC",
                                WorkCode = x
                            }));
                    }

                    return list;
                }));

            CreateMap<Submission, List<V1.IPContextSearchResponse>>()
                .AfterMap((src, dest) =>
                {
                    if (src.Rejection != null)
                        return;

                    var responses = new List<V1.IPContextSearchResponse>();

                    foreach (var ip in src.Model.InterestedParties.DistinctBy(x => x.IpBaseNumber)) 
                    {
                        var nameNumber = ip.IPNameNumber ?? ip.Names?.FirstOrDefault()?.IpNameNumber;
                        var response = new V1.IPContextSearchResponse
                        {
                            NameNumber = nameNumber,
                            FirstName = ip.Names?.FirstOrDefault(x => x.IpNameNumber == nameNumber)?.FirstName ?? string.Empty,
                            LastName = ip.Names?.FirstOrDefault(x => x.IpNameNumber == nameNumber)?.LastName ?? string.Empty,
                            MatchingIswcs = new List<V1.MatchingWork>()
                        };

                        if (src.SearchedIswcModels.Any())
                        {
                            foreach (var work in src.SearchedIswcModels)
                            {
                                var matchingWork = new V1.MatchingWork();

                                var title = work.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate).Where(x => x.IswcEligible && x.IswcStatus == "Preferred" && !x.WorkNumber.Number.StartsWith("PRS_"))?
                                .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                                if (string.IsNullOrWhiteSpace(title)) title = work.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate)?.Where(x => x.IswcStatus == "Preferred" && !x.WorkNumber.Number.StartsWith("PRS_"))?
                                .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                                if (string.IsNullOrWhiteSpace(title)) title = work.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate)?
                                .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                                if (string.IsNullOrWhiteSpace(title)) title = "No OT title found.";

                                matchingWork.OriginalTitle = title;
                                matchingWork.Iswc = work.Iswc;
                                matchingWork.Agency = work.Agency;

                                if (src.SocietyWorkCodes)
                                {
                                    matchingWork.WorkNumbers = work.VerifiedSubmissions
                                     .Select(x => new V1.WorkNumber
                                     {
                                         AgencyCode = x.WorkNumber.Type,
                                         AgencyWorkCode = x.WorkNumber.Number
                                     })
                                     .ToList();
                                }
                                else
                                {
                                    var subWorkCodes = src.SearchWorks
                                    .SelectMany(x => x.WorkNumbers)
                                    .Select(x => new V1.WorkNumber
                                    {
                                        AgencyCode = x.WorkNumber.Type,
                                        AgencyWorkCode = x.WorkNumber.Number
                                    })
                                    .ToList();

                                    if (subWorkCodes.Any())
                                    {
                                        matchingWork.WorkNumbers = work.VerifiedSubmissions
                                        .Select(x => new V1.WorkNumber
                                        {
                                            AgencyCode = x.WorkNumber.Type,
                                            AgencyWorkCode = x.WorkNumber.Number
                                        })
                                        .Where(x => subWorkCodes.Any(y =>
                                            y.AgencyCode == x.AgencyCode &&
                                            y.AgencyWorkCode == x.AgencyWorkCode))
                                        .ToList();
                                    }
                                }

                                if (src.AdditionalIPNames)
                                {
                                    var subIpBaseNumber = src.Model.InterestedParties?.FirstOrDefault()?.IpBaseNumber;
                                    if (subIpBaseNumber != null)
                                    {
                                        matchingWork.OtherInterestedParties = work.VerifiedSubmissions
                                            .SelectMany(vs => vs.InterestedParties)
                                            .Where(ip => ip.IpBaseNumber != subIpBaseNumber)
                                            .GroupBy(ip => ip.IpBaseNumber)
                                            .Select(g => g.First())
                                            .Select(ip => new V1.InterestedParty
                                            {
                                                Role = ip.CisacType.ToString().StringToEnum<V1.InterestedPartyRole>(),
                                                Name = ip.Name,
                                                LastName = ip.LastName,
                                                DisplayName = ip.DisplayName,
                                                NameNumber = ip.IPNameNumber,
                                                Affiliation = ip.Affiliation
                                            })
                                            .ToList();
                                    }
                                }
                                response.MatchingIswcs.Add(matchingWork);
                            }
                        }
                        responses.Add(response);
                    }
                    dest.AddRange(responses);
                });

            CreateMap<Submission, V1.IPContextSearchResponseBatch>()
            .ForMember(dest => dest.SearchId, opt => opt.MapFrom(src => src.SubmissionId))
            .ForMember(dest => dest.Rejection, opt => opt.MapFrom(src => src.Rejection))
            .ForMember(dest => dest.SearchResults, opt => opt.MapFrom(src => src));
        }
    }
}
