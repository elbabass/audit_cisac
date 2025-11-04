using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpanishPoint.Azure.Iswc.Api.Agency.Configuration.Swagger;
using SpanishPoint.Azure.Iswc.Api.Agency.Managers;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Ipi;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.Http.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Api.Agency.V1.Controllers
{
    internal class SearchController : BaseController, IISWC_SearchController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public SearchController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base()
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<ICollection<ISWCMetadata>>> SearchByAgencyWorkCodeAsync(string agency, string workCode, DetailLevel? detailLevel)
        {
            var response = await pipelineManager.RunPipelines(
                new Bdo.Submissions.Submission
                {
                    Model = new SubmissionModel()
                    {
                        WorkNumber = new Bdo.Work.WorkNumber()
                        {
                            Type = agency,
                            Number = workCode
                        }
                    },
                    TransactionType = TransactionType.CIQ,
                    IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                    RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                    DetailLevel = detailLevel.ToString().StringToEnum<Bdo.Submissions.DetailLevel>(),
                    RequestType = RequestType.Agency
                });

            if (response.Rejection != null)
            {
                if (response.Rejection.Code == Bdo.Rules.ErrorCode._180) return NotFound("ISWC not found.");
                else return BadRequest(response.Rejection);
            }

            var result = mapper.Map<ICollection<ISWCMetadata>>(response.SearchedIswcModels);
            if (detailLevel != DetailLevel.Full)
			{
                result.First().Works = null;

                if (detailLevel == DetailLevel.Core || detailLevel == DetailLevel.Minimal)
                    result.First().LinkedISWC = null;
                if(detailLevel == DetailLevel.Minimal)
                    result.First().OriginalTitle = string.Empty;
            }
                
            return Ok(result);
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> SearchByAgencyWorkCodeBatchAsync(IEnumerable<AgencyWorkCodeSearchModel> body)
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
                RequestType = RequestType.Agency
            }));

            return MultiStatus(mapper.Map<ICollection<ISWCMetadataBatch>>(response));
        }

        [HttpGet]
        public async Task<ActionResult<ISWCMetadata>> SearchByISWCAsync(string iswc)
        {
            var response = await pipelineManager.RunPipelines(new Bdo.Submissions.Submission
            {
                Model = new SubmissionModel() { PreferredIswc = iswc, Iswc = iswc },
                TransactionType = TransactionType.CMQ,
                IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Agency
            });

            if (response.Rejection != null)
            {
                if (response.Rejection.Code == Bdo.Rules.ErrorCode._180) return NotFound("ISWC not found.");
                else return BadRequest(response.Rejection);
            }

            return Ok(mapper.Map<ISWCMetadata>(response.SearchedIswcModels.FirstOrDefault(), opt => opt.Items.Add("ExcludeBaseNumbers", true)));
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> SearchByISWCBatchAsync(IEnumerable<IswcSearchModel> body)
        {

            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = idx + 1,
                Model = new SubmissionModel { PreferredIswc = x.Iswc },
                TransactionType = TransactionType.CMQ,
                RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Agency
            }));

            return MultiStatus(mapper.Map<ICollection<ISWCMetadataBatch>>(response));
        }

        public async Task<ActionResult<ICollection<ISWCMetadata>>> SearchByTitleAndContributorAsync(TitleAndContributorSearchModel body)
        {

            var response = await pipelineManager.RunPipelines(
                new Bdo.Submissions.Submission
                {
                    Model = new SubmissionModel()
                    {
                        InterestedParties = mapper.Map<ICollection<InterestedPartyModel>>(body.InterestedParties),
                        Titles = mapper.Map<ICollection<Bdo.Work.Title>>(body.Titles)
                    },
                    TransactionType = TransactionType.CIQ,
                    IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                    RequestSource = contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                    RequestType = RequestType.Agency
                });

            if (response.Rejection != null) 
            {
                if (response.Rejection.Code == Bdo.Rules.ErrorCode._180) return NotFound("ISWC not found.");
                else return BadRequest(response.Rejection);
            }
            
            if (response.IsPublicRequest == true)
            {
                var result = mapper.Map<ICollection<ISWCMetadata>>(response.SearchedIswcModels);

                if (result == null || !result.Any()) 
                    return NotFound("ISWC not found.");

                if (!response.Model.InterestedParties.Any())
                    return Ok(result);

                var ipFilteredResult = result.Where(x => x.InterestedParties.Any(w => response.Model.InterestedParties.Any(x => x.IPNameNumber == w.NameNumber))
                                     || x.InterestedParties.Any(w => response.Model.InterestedParties.Any(x => w.LastName.Equals(x.LastName, StringComparison.OrdinalIgnoreCase)))).ToList();
                
                if (ipFilteredResult == null || !ipFilteredResult.Any()) 
                    return NotFound("ISWC not found.");
                else
                    return Ok(ipFilteredResult);
            }

            return Ok(mapper.Map<ICollection<ISWCMetadata>>(response.SearchedIswcModels));
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> SearchByTitleAndContributorBatchAsync(IEnumerable<TitleAndContributorSearchModel> body)
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
                IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                RequestSource = this.contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                RequestType = RequestType.Agency
            }));

            var results = mapper.Map<ICollection<ISWCMetadataBatch>>(response);
            if (results != null && results.Any())
            {
                foreach (var result in results)
                {
                    result.SearchResults.ForEach(x => x.InterestedParties.ForEach(y => y.BaseNumber = null));
                    result.SearchResults.ForEach(x => x.Works.ForEach(y => y.InterestedParties.ForEach(z => z.BaseNumber = null)));
                }
            }
           
            return results != null ? MultiStatus(results) : MultiStatus(mapper.Map<ICollection<ISWCMetadataBatch>>(response));
        }
    }
}
