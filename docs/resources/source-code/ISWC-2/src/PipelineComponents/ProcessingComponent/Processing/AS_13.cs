using AutoMapper;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public class AS_13 : IProcessingSubComponent
    {
        private readonly IWorkManager workManager;
        private readonly IAS_10 as_10;
        private readonly IMapper mapper;

        public AS_13(IWorkManager workManager, IAS_10 as_10, IMapper mapper)
        {
            this.workManager = workManager;
            this.as_10 = as_10;
            this.mapper = mapper;
        }

        public IEnumerable<TransactionType> ValidTransactionTypes => new List<TransactionType> { TransactionType.FSQ };

        public bool? IsEligible => false;

        public PreferedIswcType PreferedIswcType => PreferedIswcType.Existing;

        public string Identifier => nameof(AS_13);
        public string PipelineComponentVersion => typeof(ProcessingComponent).GetComponentVersion();
        public IEnumerable<RequestType> ValidRequestTypes => new List<RequestType> { RequestType.Agency, RequestType.Label, RequestType.Publisher, RequestType.ThirdParty };

        public async Task<IswcModel> ProcessSubmission(Submission submission)
        {
            submission = await as_10.RecaculateAuthoritativeFlag(submission);

            if (submission.Model.PreviewDisambiguation && submission.MatchedResult.Matches.Any())
            {
                var model = submission.Model;
                submission.IswcModel = new IswcModel
                {
                    Agency = model.Agency,
                    Iswc = model.PreferredIswc,
                    VerifiedSubmissions = new List<VerifiedSubmissionModel>()
                    {
                        mapper.Map<VerifiedSubmissionModel>(model)
                    }
                };


                var workInfoId = submission.MatchedResult.Matches.First().Id;
                var workNumber = submission.MatchedResult.Matches.First().Numbers.First(x => x.Type != "ISWC").Number;
                foreach (var x in submission.IswcModel.VerifiedSubmissions)
                {
                    x.IswcEligible = submission.IsEligible;
                    x.Iswc = model.PreferredIswc;
                    x.WorkInfoID = 0;

                    var iswc = (await workManager.FindManyAsync(new long[] { workInfoId })).FirstOrDefault();
                    if (iswc == null) continue;
                                        
                    if (x.Titles.Any(x => x.Type == TitleType.OT))
                        x.Titles.First(x => x.Type == TitleType.OT).Name = GetConsolidatedOTTitle(iswc);

                    foreach (var title in GetConsolidatedNonOTTitles(iswc))
                        x.Titles.Add(title);

                    x.InterestedParties =  GetConsolidatedIPs(iswc);
                };

                return submission.IswcModel;

                string GetConsolidatedOTTitle(IswcModel iswc)
                {
                    var title = iswc.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate).Where(x => x.IswcEligible)?
                         .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                    if (string.IsNullOrWhiteSpace(title)) title = iswc.VerifiedSubmissions.OrderBy(y => y.LastModifiedDate)?
                    .LastOrDefault(t => t.Titles.Any(x => x.Type == TitleType.OT))?.Titles.FirstOrDefault(x => x.Type == TitleType.OT)?.Name;

                    if (string.IsNullOrWhiteSpace(title)) title = "No OT title found.";

                    return title;
                }

                List<Title> GetConsolidatedNonOTTitles(IswcModel iswc)
                {
                    var list = new List<Title>();

                    if (iswc.VerifiedSubmissions.Any() && iswc.VerifiedSubmissions.Any(x => x?.Titles != null))
                    {
                        foreach (var item in iswc.VerifiedSubmissions.SelectMany(x => x.Titles).Where(t => t.Type != TitleType.OT))
                        {
                            if (!list.Any(l => l.Name == item.Name && l.Type == item.Type))
                                list.Add(item);
                        }
                    }
                    return list;
                }

                List<InterestedPartyModel> GetConsolidatedIPs(IswcModel iswc)
                {
                    var list = new List<InterestedPartyModel>();

                    if (iswc.VerifiedSubmissions.Where(x => x.IswcEligible == true).Any() && iswc.VerifiedSubmissions.Where(x => x.IswcEligible == true).Any(x => x?.InterestedParties != null))
                    {
                        var ipTuples = new List<Tuple<InterestedPartyModel, DateTime?>>();

                        foreach (var x in iswc.VerifiedSubmissions.Where(x => x.IswcEligible == true))
                            foreach (var y in x.InterestedParties)
                            {
                                if (!y.IsExcludedFromIswc && !y.IsPseudonymGroupMember)
                                    ipTuples.Add(Tuple.Create(y, x.CreatedDate));
                            }
                        var ipsGroups = ipTuples.GroupBy(x => !string.IsNullOrWhiteSpace(x.Item1.IpBaseNumber) ? x.Item1.IpBaseNumber : x.Item1.IPNameNumber?.ToString());

                        foreach (var group in ipsGroups)
                        {
                            if (group.Count() == 1) list.Add(group.First().Item1);
                            else if (group.Count() > 1)
                            {
                                var orderedIps = group.OrderByDescending(d => d.Item2);
                                var authIp = orderedIps.FirstOrDefault(x => x.Item1.IsAuthoritative == true);

                                if (iswc.VerifiedSubmissions.Any(x => x.IsPublicRequest))
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
                }


            }

            var workinfoId = await workManager.AddWorkInfoAsync(submission, getNewIswc: false);
            submission.IswcModel = (await workManager.FindManyAsync(new long[] { workinfoId })).First();

            return submission.IswcModel;
        }
    }
}
