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
    internal class UsageController : BaseController, IISWC_Usage_SearchController
    {
        private readonly IPipelineManager pipelineManager;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor contextAccessor;

        public UsageController(IPipelineManager pipelineManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base()
        {
            this.pipelineManager = pipelineManager;
            this.mapper = mapper;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<ICollection<ISWCMetadataBatch>>> UsageSearchBatchAsync(IEnumerable<UPSearchModel> body)
        {
            var response = await pipelineManager.RunPipelines(body.Select((x, idx) => new Bdo.Submissions.Submission
            {
                SubmissionId = idx + 1,
                Model = new SubmissionModel()
                {  
                    InterestedParties = mapper.Map<ICollection<InterestedPartyModel>>(x.InterestedParties),
                    Titles = mapper.Map<ICollection<Bdo.Work.Title>>(x.Titles),
                    Performers = mapper.Map<ICollection<Bdo.Work.Performer>>(x.Performers)
                },
                TransactionType = TransactionType.CIQ,
                IsPublicRequest = contextAccessor.GetRequestItem<bool>(isPublicRequestContextItemKey),
                RequestSource = this.contextAccessor.GetRequestItem<RequestSource>(requestSourceContextItemKey),
                MatchingSource = "Usage",
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
