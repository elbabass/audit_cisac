using AutoMapper;
using IdentityServer4.Extensions;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Bdo.Agent;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Configuration
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<V1.Submission, SubmissionModel>()
               .ForMember(dest => dest.WorkNumber, opt => opt.MapFrom((src, dest) =>
               {
                   if (!string.IsNullOrEmpty(src.Agency) && !string.IsNullOrEmpty(src.Workcode))
                       return new WorkNumber { Type = src.Agency, Number = src.Workcode };

                   var workNumber = new WorkNumber(); 

                   string? publisherWorkcode = src.AdditionalIdentifiers?.PublisherIdentifiers?.FirstOrDefault()?.WorkCode?.FirstOrDefault();
                   publisherWorkcode = publisherWorkcode ?? GetRandomWorkCode();
                   string submission = JsonConvert.SerializeObject(src);

                   if (!string.IsNullOrEmpty(publisherWorkcode))
                   {
                       var stringToHash = $"{publisherWorkcode}{submission}{DateTime.Now.Ticks}";
                       var blakeHash = Blake3.Hasher.Hash(Encoding.UTF8.GetBytes(stringToHash));
                       var workCode = $"AS{ blakeHash.ToString().ToUpper().Substring(0, 18) }";

                       workNumber.Type = src.Agency;
                       workNumber.Number = workCode;
                   }
            
                   return workNumber;

                   string GetRandomWorkCode()
                   {
                       string workCode = string.Empty;
                       using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                       {
                           byte[] codeBuffer = new byte[32];
                           byte[] numberBuffer = new byte[4];

                           rng.GetBytes(numberBuffer);
                           int num = BitConverter.ToInt32(numberBuffer, 0);
                           int r = new Random(num).Next(10, 15);
                           rng.GetBytes(codeBuffer);
                           workCode = Convert.ToBase64String(codeBuffer).Substring(0, r).Replace("+", "").Replace("/", "");       
                       }
                       return workCode;
                   }
               }))
               .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.Agency))
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
               .ForMember(dest => dest.DerivedFrom, opt => opt.MapFrom(src => src.DerivedFromIswcs))
               .ForMember(dest => dest.Titles, opt => opt.MapFrom((src, dest) =>
               {
                   var list = new List<Title>();

                   if (src.OtherTitles != null && src.OtherTitles.Any())
                   {
                       list.AddRange(src.OtherTitles
                           .Select(x => new Bdo.Work.Title
                           {
                               Name = x.Title1,
                               Type = (TitleType)Enum.Parse(typeof(TitleType), x.Type.ToString())
                           }).Distinct());
                   }
                   if (!string.IsNullOrEmpty(src.OriginalTitle))
                       list.Add(new Title { Name = src.OriginalTitle, Type = TitleType.OT });

                   return list;
               }))
               .ForMember(dest => dest.AdditionalIdentifiers, opt => opt.MapFrom((src, dest) =>
               {
                   var list = new List<AdditionalIdentifier>();

                   if (src.AdditionalIdentifiers != null)
                   {
                       if (src.AdditionalIdentifiers.Isrcs != null)
                           list.AddRange(src.AdditionalIdentifiers.Isrcs.Select(x => new AdditionalIdentifier
                           {
                               SubmitterCode = "ISRC",
                               WorkCode = x
                           }));
                       if (src.AdditionalIdentifiers.PublisherIdentifiers != null)
                       {
                           foreach (var pub in src.AdditionalIdentifiers.PublisherIdentifiers)
                           {
                               list.AddRange(pub.WorkCode.Select(x => new AdditionalIdentifier
                               {
                                   WorkCode = x,
                                   NameNumber = pub.NameNumber,
                                   SubmitterCode = pub.SubmitterCode
                               }));
                           }

                       }
                       if (src.AdditionalIdentifiers.LabelIdentifiers != null)
                       {
                           foreach (var li in src.AdditionalIdentifiers.LabelIdentifiers)
                           {
                               list.AddRange(li.WorkCode.Select(x => new AdditionalIdentifier
                               {
                                   WorkCode = x,
                                   SubmitterDPID = li.SubmitterDPID
                               }));
                           }
                       }
                       if (src.AdditionalIdentifiers.Recordings != null)
                       {
                           foreach (var rec in src.AdditionalIdentifiers.Recordings)
                           {
                               list.Add(new AdditionalIdentifier
                               {
                                   WorkCode = rec.Isrc,
                                   RecordingTitle = rec.RecordingTitle,
                                   SubTitle = rec.SubTitle,
                                   LabelName = rec.LabelName,
                                   ReleaseEmbargoDate = rec.ReleaseEmbargoDate
                               });
                           }
                       }
                   }

                   return list;
               }))
               .ForMember(dest => dest.AdditionalAgencyWorkNumbers, opt => opt.MapFrom((src, dest) =>
               {
                   var list = new List<AdditionalAgencyWorkNumber>();

                   if (src.AdditionalIdentifiers != null && src.AdditionalIdentifiers.AgencyWorkCodes != null && src.AdditionalIdentifiers.AgencyWorkCodes.Any())
                   {
                       var workNumbers = new List<WorkNumber>();

                       workNumbers.AddRange(src.AdditionalIdentifiers.AgencyWorkCodes.Select(x => new WorkNumber { Number = x.WorkCode, Type = x.Agency }));

                       foreach (var number in workNumbers)
                       {
                           list.Add(new AdditionalAgencyWorkNumber { WorkNumber = number });
                       }
                   }

                   return list;

               }))
               .ForMember(dest => dest.RelatedSubmissionIncludedIswc, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.PreferredIswc)))
               ;

            CreateMap<V1.DisambiguateFrom, DisambiguateFrom>().ReverseMap();
            CreateMap<V1.Performer, Performer>().ReverseMap();
            CreateMap<V1.Instrumentation, Instrumentation>().ReverseMap();

            CreateMap<V1.MultipleAgencyWorkCodes2, MultipleAgencyWorkCodes>()
                .ForMember(x => x.WorkCode, opt => opt.MapFrom(src => src.WorkCode))
                .ForMember(x => x.Agency, opt => opt.MapFrom(src => src.Agency));

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
                .ForMember(dest => dest.IpBaseNumber, opt => opt.MapFrom(src => src.BaseNumber))
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
                            ipsGroups = ipTuples.GroupBy(x => $"{ x.Item1.Name } { x.Item1.LastName }" );

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
                                var recording = new V1.Recordings()
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
                                };

                                recordings.Add(recording);
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
                            BaseNumber = ip.IpBaseNumber,
                            Affiliation = ip.Affiliation,
                            NameNumber = ip.IPNameNumber,
                            Name = ip.Name,
                            LastName = ip.LastName,
                            DisplayName = ip.DisplayName
                        });
                    }

                    return ips;

                }));

            CreateMap<V1.WorkNumber, WorkNumber>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.AgencyCode))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.AgencyWorkCode));

            CreateMap<Submission, V1.ISWCMetadataBatch>()
                .ForMember(dest => dest.SearchId, opt => opt.MapFrom(src => src.SubmissionId))
                .ForMember(dest => dest.Rejection, opt => opt.MapFrom(src => src.Rejection))
                .ForMember(dest => dest.SearchResults, opt => opt.MapFrom(src => src.SearchedIswcModels));

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
                                    if (ai.RecordingTitle != null && !recordings.Any(r => r.Isrc == ai.WorkCode))
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
                                    if (!performers.Any(p => p.FirstName == perf.FirstName && p.LastName == perf.LastName))
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
                }))
                .ForMember(dest => dest.AlternateIswcMatches, opt => opt.MapFrom((src, dest) =>
                {
                    var list = new List<V1.AlternateIswcMatch>();

                    if (src.HasAlternateIswcMatches)
                    {
                        var inconsistentMetadataIrscMatch = src.Rejection?.Code == Bdo.Rules.ErrorCode._249;
                        var alternateMatches = inconsistentMetadataIrscMatch
                                                    ? src.MatchedResult.Matches.Concat(src.IsrcMatchedResult.Matches).ToList()
                                                    : src.MatchedResult.Matches.Concat(src.IsrcMatchedResult.Matches.Skip(1)).ToList();

                        foreach (var match in alternateMatches)
                        {
                            var iswc = match.Numbers.FirstOrDefault(n => n.Type == "ISWC")?.Number;

                            if (iswc != null && iswc != src.Model.PreferredIswc || inconsistentMetadataIrscMatch)
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
                                        BaseNumber = contributor.IpBaseNumber,
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
                }));

            CreateMap<IswcModel, V1.LinkedIswcs>()
                .ForMember(dest => dest.IswcMetadata, opt => opt.MapFrom(src => src));

            CreateMap<Submission, V1.VerifiedSubmissionBatch>()
                .ForMember(dest => dest.SubmissionId, opt => opt.MapFrom(src => src.SubmissionId))
                .ForPath(dest => dest.Submission.VerifiedSubmission,
                opt => opt.MapFrom(src => src.IswcModel.VerifiedSubmissions.FirstOrDefault(x => x.WorkNumber != null && src.Model.WorkNumber != null && x.WorkNumber.Number == src.Model.WorkNumber.Number)))
                .ForPath(dest => dest.Submission.LinkedIswcs, opt => opt.MapFrom(src => src.IswcModel != null ? src.IswcModel.LinkedIswc : default))
                .AfterMap((src, dest) =>
                {
                    var recordings = new List<V1.Recordings>();
                    var performers = new List<V1.Performer>();

                    if (src.IswcModel.VerifiedSubmissions.Any())
                    {
                        foreach (var sub in src.IswcModel.VerifiedSubmissions)
                        {
                            if (sub.AdditionalIdentifiers != null) {
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

                    var list = new List<V1.AlternateIswcMatch>();

                    if (src.HasAlternateIswcMatches)
                    {
                        var inconsistentMetadataIrscMatch = src.Rejection?.Code == Bdo.Rules.ErrorCode._249;
                        var alternateMatches = inconsistentMetadataIrscMatch
                                                    ? src.MatchedResult.Matches.Concat(src.IsrcMatchedResult.Matches).ToList()
                                                    : src.MatchedResult.Matches.Concat(src.IsrcMatchedResult.Matches.Skip(1)).ToList();

                        foreach (var match in alternateMatches)
                        {
                            var iswc = match.Numbers.FirstOrDefault(n => n.Type == "ISWC")?.Number;

                            if (iswc != null && iswc != src.Model.PreferredIswc || inconsistentMetadataIrscMatch)
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
                                        BaseNumber = contributor.IpBaseNumber,
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

                    dest.Submission.AlternateIswcMatches = list.DistinctBy(x => x.Iswc).ToList();
                })
                .ForPath(dest => dest.Submission.MultipleAgencyWorkCodes, opt => opt.MapFrom(src => src.MultipleAgencyWorkCodes)).ReverseMap();


            
            CreateMap<MultipleAgencyWorkCodes ,V1.MultipleAgencyWorkCodes>()
                .ForMember(dest => dest.Agency, opt => opt.MapFrom(src => src.Agency))
                .ForMember(dest => dest.WorkCode, opt => opt.MapFrom(src => src.WorkCode));

            CreateMap<Rejection, V1.Rejection>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.ToString().Replace("_", "") ?? string.Empty));

            CreateMap<WorkflowTask, V1.WorkflowTask>()
                .ForMember(dest => dest.WorkflowType, opt => opt.MapFrom(src => src.WorkflowTaskType.ToString().StringToEnum<V1.WorkflowTaskWorkflowType>()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TaskStatus))
                .ForMember(dest => dest.WorkflowTaskId, opt => opt.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.AssignedSociety, opt => opt.MapFrom(src => src.AssignedAgencyId))
                .ForMember(dest => dest.WorkflowMessage, opt => opt.MapFrom(src => src.Message)).ReverseMap();

            CreateMap<V1.WorkflowTaskUpdate, WorkflowTask>()
                .ForMember(dest => dest.TaskStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.WorkflowTaskType, opt => opt.MapFrom(src => src.WorkflowType.ToString().StringToEnum<WorkflowType>()))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.WorkflowMessage));

            CreateMap<V1.WorkflowStatus, Bdo.Work.InstanceStatus>();

            CreateMap<V1.Rejection, Rejection>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => (Bdo.Rules.ErrorCode)Enum.Parse(typeof(Bdo.Rules.ErrorCode), src.Code)));

            CreateMap<InterestedPartyModel, V1.InterestedParty>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.CisacType.ToString().StringToEnum<V1.InterestedPartyRole>()))
                .ForMember(dest => dest.BaseNumber, opt => opt.MapFrom(src => src.IpBaseNumber))
                .ForMember(dest => dest.NameNumber, opt => opt.MapFrom(src => src.IPNameNumber))
                .ForMember(dest => dest.Affiliation, opt => opt.MapFrom(src => src.Affiliation))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.LegalEntityType, opt => opt.MapFrom(src => (int)src.LegalEntityType));


            CreateMap<AgentNotification, V1.AgentNotificationResponse>()
                .ForMember(dest => dest.Notifications, opt => opt.MapFrom(src => src.CsnNotifications));

            CreateMap<CsnNotification, V1.CsnNotification>()
                .ForMember(dest => dest.CsnTransactionType, opt => opt.MapFrom(src => StringExtensions.StringToEnum<V1.CsnTransactionType>(src.TransactionType.ToString())))
                .ForMember(dest => dest.IswcMetadata, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.IswcMetaData) ?
                JsonConvert.DeserializeObject<V1.ISWCMetadata>(src.IswcMetaData) : default));

            CreateMap<V1.AgentRun, AgentRun>();

        }
    }
}
