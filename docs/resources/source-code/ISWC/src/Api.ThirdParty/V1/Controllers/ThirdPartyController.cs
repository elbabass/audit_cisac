using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.ThirdParty.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.ThirdParty.V1.Controllers
{
    internal class ThirdPartyController : BaseController, IISWC_Third_Party_SearchController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public ThirdPartyController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base()
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> ThirdPartySearchByISWCBatchAsync(IEnumerable<IswcSearchModel> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = idx + 1,
                Model = new SubmissionModel { PreferredIswc = x.Iswc },
                TransactionType = TransactionType.CMQ,
                IsPublicRequest = true,
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                DetailLevel = DetailLevel.CoreAndLinks,
                RequestType = RequestType.ThirdParty
            }));

            var results = mapper.Map<ICollection<ISWCMetadataBatch>>(response);

            if (results != null)
            {
                foreach (var result in results)
                {
                    var searchResult = result.SearchResults.FirstOrDefault();
                    if(searchResult != null)
                    {
                        searchResult.InterestedParties = searchResult.InterestedParties.Where(x => x.Role != InterestedPartyRole.E).ToList();
                        searchResult.Works = null;
                    }
                }

                return MultiStatus(results);
            }

            return MultiStatus(Enumerable.Empty<ISWCMetadataBatch>());
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> ThirdPartySearchByAgencyWorkCodeBatchAsync(IEnumerable<AgencyWorkCodeSearchModel> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = idx + 1,
                Model = new SubmissionModel()
                {
                    WorkNumber = new Bdo.Work.WorkNumber()
                    {
                        Type = x.Agency,
                        Number = x.WorkCode
                    }
                },
                TransactionType = TransactionType.CIQ,
                RequestSource = this.contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                IsPublicRequest = true,
                DetailLevel = DetailLevel.CoreAndLinks,
                RequestType = RequestType.ThirdParty
            }));

            var results = mapper.Map<ICollection<ISWCMetadataBatch>>(response);

            if (results != null)
            {
                foreach (var result in results)
                {
                    var searchResult = result.SearchResults.FirstOrDefault();
                    if (searchResult != null)
                    {
                        searchResult.Works = null;
                        searchResult.InterestedParties = searchResult.InterestedParties.Where(x => x.Role != InterestedPartyRole.E).ToList();
                    }
                }

                return MultiStatus(results);
            }

            return MultiStatus(mapper.Map<ICollection<ISWCMetadataBatch>>(response));
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> ThirdPartySearchByTitleAndContributorBatchAsync(IEnumerable<TitleAndContributorSearchModel> body)
        {

            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = idx + 1,
                Model = new SubmissionModel()
                {
                    InterestedParties = mapper.Map<ICollection<InterestedPartyModel>>(x.InterestedParties),
                    Titles = mapper.Map<ICollection<Bdo.Work.Title>>(x.Titles)
                },
                TransactionType = TransactionType.CIQ,
                RequestSource = this.contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                IsPublicRequest = true,
                DetailLevel = DetailLevel.CoreAndLinks,
                RequestType = RequestType.ThirdParty
            }));

            List<ISWCMetadataBatch> resultsList = new List<ISWCMetadataBatch>();

            foreach(var submission in response)
            {
                if(submission.Rejection != null)
                {
                    resultsList.Add(new ISWCMetadataBatch
                    {
                        SearchId = submission.SubmissionId,
                        Rejection = mapper.Map<Rejection>(submission.Rejection)
                    });
                    continue;
                }

                var filteredSearchResults = mapper.Map<ICollection<ISWCMetadata>>(submission.SearchedIswcModels)
                    .Where(x => (x.InterestedParties.Count(x => x.Role == InterestedPartyRole.C) == submission.Model.InterestedParties.Count(x => x.CisacType == CisacInterestedPartyType.C))
                        && (x.InterestedParties.Count(x => x.Role == InterestedPartyRole.MA) == submission.Model.InterestedParties.Count(x => x.CisacType == CisacInterestedPartyType.MA))
                        && (x.InterestedParties.Count(x => x.Role == InterestedPartyRole.TA) == submission.Model.InterestedParties.Count(x => x.CisacType == CisacInterestedPartyType.TA))
                        && (x.InterestedParties.Where(x => x.Role == InterestedPartyRole.C || x.Role == InterestedPartyRole.MA || x.Role == InterestedPartyRole.TA).All(w => submission.Model.InterestedParties.Any(x => x.IPNameNumber == w.NameNumber))
                        || (x.InterestedParties.Where(x => x.Role == InterestedPartyRole.C || x.Role == InterestedPartyRole.MA || x.Role == InterestedPartyRole.TA).All(w => submission.Model.InterestedParties.Any(x => x.LastName == w.LastName))))).ToList();

                if (filteredSearchResults.Count() > 1)
                {
                    resultsList.Add(new ISWCMetadataBatch
                    {
                        SearchId = submission.SubmissionId,
                        Rejection = mapper.Map<Rejection>(submission.Rejection) ?? new Rejection { Code = "164", Message = "Multiple matching ISWCs have been found" }
                    });
                    continue;
                }

                if (filteredSearchResults.Any())
                {
                    filteredSearchResults.FirstOrDefault().Works = null;
                    filteredSearchResults.FirstOrDefault().InterestedParties = filteredSearchResults.FirstOrDefault().InterestedParties.Where(x => x.Role != InterestedPartyRole.E).ToList();
                }
                else
                {
                    resultsList.Add(new ISWCMetadataBatch
                    {
                        SearchId = submission.SubmissionId,
                        Rejection = mapper.Map<Rejection>(submission.Rejection) ?? new Rejection { Code = "180", Message = "ISWC not found." }
                    });
                    continue;
                }

                resultsList.Add(new ISWCMetadataBatch
                {
                    SearchId = submission.SubmissionId,
                    Rejection = mapper.Map<Rejection>(submission.Rejection),
                    SearchResults = new List<ISWCMetadata> { filteredSearchResults.FirstOrDefault() }
                });
            }

            return resultsList != null ? MultiStatus(resultsList) : MultiStatus(mapper.Map<ICollection<ISWCMetadataBatch>>(response));
        }
    }
}
