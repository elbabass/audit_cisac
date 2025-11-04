using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Pipelines.MatchingPipeline;
using SpanishPoint.Azure.Iswc.Pipelines.PostMatchingPipeline;
using SpanishPoint.Azure.Iswc.Pipelines.ProcessingPipeline;
using SpanishPoint.Azure.Iswc.Pipelines.ValidationPipeline;

[assembly: InternalsVisibleTo("SpanishPoint.Azure.Iswc.Api.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]


namespace SpanishPoint.Azure.Iswc.Api.Label.Managers
{
    internal class PipelineManager : IPipelineManager
    {
        private readonly IValidationPipeline validationPipeline;
        private readonly IMatchingPipeline matchingPipeline;
        private readonly IPostMatchingPipeline postMatchingPipeline;
        private readonly IProcessingPipeline processingPipeline;
        private readonly IAuditManager auditManager;
        private readonly ILogger<PipelineManager> logger;
        private readonly IMapper mapper;

        public PipelineManager(IValidationPipeline validationPipeline,
            IMatchingPipeline matchingPipeline,
            IPostMatchingPipeline postMatchingPipeline,
            IProcessingPipeline processingPipeline,
            IAuditManager auditManager,
            ILogger<PipelineManager> logger,
            IMapper mapper)
        {
            this.validationPipeline = validationPipeline;
            this.matchingPipeline = matchingPipeline;
            this.postMatchingPipeline = postMatchingPipeline;
            this.processingPipeline = processingPipeline;
            this.auditManager = auditManager;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Submission>> RunPipelines(IEnumerable<Submission> submissions)
        {
            try
            {
                var auditId = Guid.NewGuid();
                foreach (var submission in submissions)
                    submission.AuditId = auditId;

                await auditManager.LogSubmissions(submissions
                    .Where(x => !(x.TransactionType == Bdo.Edi.TransactionType.CAR && x.Model.PreviewDisambiguation)
                        && !(x.TransactionType == Bdo.Edi.TransactionType.CUR && x.Model.PreviewDisambiguation)));

                if (submissions.Any(x => x.MultipleAgencyWorkCodes.Any()))
                    ExplodeMultipleAgencyWorkCodes();


                submissions = await validationPipeline.RunPipeline(submissions);
                submissions = await matchingPipeline.RunPipeline(submissions);

                if (submissions.Any(x => x.MultipleAgencyWorkCodes.Any()))
                    SortForPostMatcing();

                submissions = await postMatchingPipeline.RunPipeline(submissions.Where(x=> !x.SkipProcessing));

                if (submissions.Any(x => x.MultipleAgencyWorkCodes.Any()))
                    SortForProcessing();

                submissions = await processingPipeline.RunPipeline(submissions);


                void ExplodeMultipleAgencyWorkCodes()
                {
                    var subsToAdd = new List<Submission>();
                    var subId = submissions.Last().SubmissionId;
                    foreach (var multipleWorkcodeSub in submissions.Where(x => x.MultipleAgencyWorkCodes.Any()))
                    {
                        foreach (var x in multipleWorkcodeSub.MultipleAgencyWorkCodes)
                            subsToAdd.Add(CreateChildSubmission(multipleWorkcodeSub, x, subId += 1));

                    }

                    submissions = submissions.Concat(subsToAdd);
                }

                void SortForPostMatcing()
                {
                    var children = submissions.Where(x => x.MultipleAgencyWorkCodesChild).GroupBy(x => x.SubmissionParentId);

                    foreach (var child in children)
                    {
                        var parent = submissions?.FirstOrDefault(x => x.SubmissionId == child.FirstOrDefault()?.SubmissionParentId);
                        var parentId = child.FirstOrDefault()?.SubmissionParentId;

                        if (parent?.Rejection == null)
                        {
                            var update = child.FirstOrDefault(x => x.TransactionType == Bdo.Edi.TransactionType.CUR && x.Rejection == null);

                            if (update == null)
                                update = parent;

                            if (update != null)
                            {
                                if (parent?.Rejection == null)
                                    submissions?.Where(x => x.SubmissionParentId == parentId || x.SubmissionId == parentId && x.SubmissionId != update?.SubmissionId)
                                    .Select(x =>
                                    {
                                        x.ExistingWork = update?.ExistingWork != null ? update.ExistingWork : x.ExistingWork;
                                        x.MatchedResult = update?.MatchedResult != null ? update.MatchedResult : x.MatchedResult;
                                        x.Model.PreferredIswc = update?.Model.PreferredIswc ?? string.Empty;
                                        return x;
                                    }).ToList();
                                else
                                    submissions?.Where(x => x.SubmissionParentId == parentId).Select(x =>
                                    {
                                        x.SkipProcessing = true;
                                        return x;
                                    }).ToList();
                            }
                        }
                        else if (parent.Rejection != null)
                            submissions?.Where(x => x.SubmissionParentId == parentId).Select(x =>
                            {
                                x.SkipProcessing = true;
                                return x;
                            }).ToList();
                    }

                }

                void SortForProcessing()
                {

                    var children = submissions.Where(x => x.MultipleAgencyWorkCodesChild).GroupBy(x => x.SubmissionParentId);

                    if (children != null)
                        foreach (var child in children)
                        {
                            var parent = submissions?.FirstOrDefault(x => x.SubmissionId == child.FirstOrDefault()?.SubmissionParentId);
                            var parentId = child.FirstOrDefault()?.SubmissionParentId;

                            if (parent?.Rejection != null)
                                submissions?.Where(x => x.SubmissionParentId == parentId).Select(x =>
                                {
                                    x.SkipProcessing = true;
                                    return x;
                                }).ToList();
                        }
                }

               
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, $"Failed to run one or more pipelines in {nameof(PipelineManager)} - {nameof(RunPipelines)}");

                for (var x = 0; x < submissions.Count(); x++)
                {
                    var submission = submissions.ElementAt(x);
                    if (submission.IsProcessed)
                        continue;

                    submission.IsProcessed = true;
                    submission.ToBeProcessed = false;
                    submission.Rejection = new Rejection(Bdo.Rules.ErrorCode._100, "Internal Server Error.");
                }

                throw;
            }
            finally
            {
                await auditManager.LogSubmissions(submissions
                    .Where(x => !(x.TransactionType == Bdo.Edi.TransactionType.CAR && x.MatchedResult.Matches.Any() && x.Model.PreviewDisambiguation)
                    && !(x.TransactionType == Bdo.Edi.TransactionType.CUR && x.Model.PreviewDisambiguation)
                    && !(x.TransactionType == Bdo.Edi.TransactionType.CUR && x.Model.WorkNumber.Number.Length == 20 && x.Model.WorkNumber.Number.StartsWith("AS"))));
            }

            return submissions;
        }

        public async Task<Submission> RunPipelines(Submission submission)
        {
            return (await RunPipelines(new List<Submission> { submission })).First();
        }

        Submission CreateChildSubmission(Submission parent, MultipleAgencyWorkCodes multipleAgencyWorkCode, int id)
        {
            var parentId = parent.SubmissionId;
            var child = new Submission();
            child.Model = new SubmissionModel();
            child.Model = parent.Model.Copy();
            child.Model.Agency = multipleAgencyWorkCode.Agency;
            child.Model.SourceDb = int.TryParse(multipleAgencyWorkCode.Agency, out int i) ? i : 0;
            child.Model.WorkNumber = new WorkNumber { Number = multipleAgencyWorkCode.WorkCode, Type = multipleAgencyWorkCode.Agency };
            child.MultipleAgencyWorkCodes = new List<MultipleAgencyWorkCodes>();
            child.MultipleAgencyWorkCodesChild = true;
            child.SubmissionParentId = parentId;
            child.SubmissionId = id;
            child.RequestSource = parent.RequestSource;
            child.DetailLevel = parent.DetailLevel;

            return child;
        }
    }
}
