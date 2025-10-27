using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Bdo.Work;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent
{

    public interface IProcessingComponent
    {
        Task<IEnumerable<Submission>> ProcessBatch(IEnumerable<Submission> batch);
    }

    public class ProcessingComponent : IProcessingComponent
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IWorkManager workManager;
        private readonly IMessagingManager messagingManager;
        private readonly ILogger<ProcessingComponent> logger;

        public ProcessingComponent(
            IServiceProvider serviceProvider,
            IWorkManager workManager, 
            IMessagingManager messagingManager,
            ILogger<ProcessingComponent> logger)
        {
            this.serviceProvider = serviceProvider;
            this.workManager = workManager;
            this.messagingManager = messagingManager;
            this.logger = logger;
        }

        public async Task<IEnumerable<Submission>> ProcessBatch(IEnumerable<Submission> batch)
        {
            var processedSubmissions = new List<Submission>();

            foreach (var submission in batch)
            {
                var sw = Stopwatch.StartNew();

                try
                {
                    if (submission.Rejection == null && !submission.SkipProcessing)
                    {
                        if(submission.MultipleAgencyWorkCodesChild && string.IsNullOrWhiteSpace(submission.Model.PreferredIswc))
                            submission.Model.PreferredIswc = batch.FirstOrDefault(x=> x.SubmissionId == submission.SubmissionParentId).Model.PreferredIswc;

                        var preferedIswcType = GetPreferredIswcType(submission);

                        var validSubComponents = AppDomain.CurrentDomain.GetComponentsOfType<IProcessingSubComponent>(serviceProvider)
                            .Where(p =>
                            p.ValidTransactionTypes.Contains(submission.TransactionType)
                            && (!p.IsEligible.HasValue || p.IsEligible == submission.IsEligible)
                            && p.PreferedIswcType == preferedIswcType
                            && p.ValidRequestTypes.Contains(submission.RequestType))
                            .ToList();

                        foreach (var subComponent in validSubComponents)
                        {
                            submission.IswcModel = await subComponent.ProcessSubmission(submission);

                            submission.RulesApplied.Add(new RuleExecution
                            {
                                RuleName = subComponent.Identifier,
                                RuleVersion = subComponent.PipelineComponentVersion,
                                TimeTaken = sw.Elapsed
                            });
                        }
                    }

                    submission.IsProcessed = true;

                    if (submission.Rejection == null && !submission.SkipProcessing && submission.Model.AdditionalAgencyWorkNumbers.Any())
                    {
                        var additionalAgencyWorkNumber = new List<AdditionalAgencyWorkNumber>();

                        foreach (var x in submission.Model.AdditionalAgencyWorkNumbers)
                        {
                            additionalAgencyWorkNumber.Add(new AdditionalAgencyWorkNumber { WorkNumber = x.WorkNumber });
                        }

                        submission.IswcModel.VerifiedSubmissions.Select(x => { x.AdditionalAgencyWorkNumbers = additionalAgencyWorkNumber; return x; }).ToList();
                    }

                    if (submission.Model.AdditionalIdentifiers != null && submission.Model.AdditionalIdentifiers.Any())
                    {
                        foreach (var verifiedSubmission in submission.IswcModel.VerifiedSubmissions.Where(x => x.AdditionalIdentifiers.Any()))
                        {
                            foreach (var additionalIdentifier in submission.Model.AdditionalIdentifiers.Where(x => x.NameNumber != null))
                            {
                                if (verifiedSubmission.AdditionalIdentifiers.Where(x => x.WorkCode == additionalIdentifier.WorkCode).Any())
                                {
                                    verifiedSubmission.AdditionalIdentifiers.FirstOrDefault(x => x.WorkCode == additionalIdentifier.WorkCode)
                                        .NameNumber = additionalIdentifier?.NameNumber;
                                }
                            }
                            if (verifiedSubmission.AdditionalIdentifiers.Where(x => x.SubmitterDPID != null).Any())
                            {
                                foreach (var additionalIdentifier in submission.Model.AdditionalIdentifiers.Where(x => x.SubmitterDPID != null))
                                {
                                    if (verifiedSubmission.AdditionalIdentifiers.Where(x => x.WorkCode == additionalIdentifier.WorkCode).Any())
                                    {
                                        verifiedSubmission.AdditionalIdentifiers.FirstOrDefault(x => x.WorkCode == additionalIdentifier.WorkCode)
                                            .SubmitterDPID = additionalIdentifier?.SubmitterDPID;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    submission.IsProcessed = false;
                    submission.Rejection = await messagingManager.GetRejectionMessage(ErrorCode._155);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    submission.IsProcessed = false;
                    submission.Rejection = new Rejection(ErrorCode._100, "Internal Server Error.");
                }
                finally
                {
                    processedSubmissions.Add(submission);
                }
            }

            if (processedSubmissions.Count() == 1
                && processedSubmissions.FirstOrDefault(x => x.Rejection == null
                && x.IswcModel != null
                && x.IswcModel.VerifiedSubmissions.Any())?.TransactionType == TransactionType.CAR)
            {
                if (!(processedSubmissions.FirstOrDefault().MatchedResult.Matches.Any() && processedSubmissions.FirstOrDefault().Model.PreviewDisambiguation))
                {
                    await workManager.AddSubmissionAsync(processedSubmissions.FirstOrDefault());
                   
                    var processedSubmission = processedSubmissions.FirstOrDefault();
                    var verifiedSubmissions = processedSubmission.IswcModel.VerifiedSubmissions;
                    if (processedSubmission.RequestType != RequestType.Label 
                        && verifiedSubmissions.Count(y => y.WorkNumber.Number.StartsWith("PRS")) > 0 
                        && verifiedSubmissions.Count(y => !y.WorkNumber.Number.StartsWith("PRS")) == 1)
                        await workManager.UpdateLabelSubmissionsAsync(processedSubmissions.FirstOrDefault());
                }         
            } 

            return processedSubmissions;

            PreferedIswcType GetPreferredIswcType(Submission submission)
            {
                PreferedIswcType val = PreferedIswcType.Different;

                if (submission.MatchedResult?.Matches.Count() < 1
                    && (submission.IsrcMatchedResult?.Matches.Count() < 1 || submission.RequestType != RequestType.Label)
                    && !submission.MultipleAgencyWorkCodesChild)
                {
                    val = PreferedIswcType.New;
                }
                else if (submission.Model.PreferredIswc != null
                    && submission.MatchedResult?.Matches.Count() > 0
                    && submission.MatchedResult.Matches.SelectMany(x => x.Numbers).Any(x => x.Number == submission.Model.PreferredIswc)
                    || submission.MultipleAgencyWorkCodesChild)
                {
                    val = PreferedIswcType.Existing;
                }
                else if (submission.Model.PreferredIswc != null
                     && submission.IsrcMatchedResult?.Matches.Count() > 0
                     && submission.IsrcMatchedResult.Matches.SelectMany(x => x.Numbers).Any(x => x.Number == submission.Model.PreferredIswc)
                     || submission.MultipleAgencyWorkCodesChild)
                {
                    val = PreferedIswcType.Existing;
                }

                if (submission.TransactionType == TransactionType.CUR && submission.MatchedResult?.Matches.Count() > 0)
                {
                    if (!submission.IsEligible
                        && (submission.Model.PreferredIswc != submission.MatchedResult.Matches?.FirstOrDefault(
                        x => x.Numbers.Any(n => n.Number == submission.Model.WorkNumber.Number))?.Numbers.FirstOrDefault(i => i.Type == "ISWC")?.Number))
                    {
                        val = PreferedIswcType.Different;
                    }
                    else if (submission.IsEligible
                        && submission.MatchedResult.Matches.Any(x => x.IswcStatus == 2)
                        && submission.MatchedResult.Matches.Where(x => x.IswcStatus == 2).SelectMany(x => x.Numbers).Any(x => x.Type == "ISWC" && x.Number != submission.Model.PreferredIswc))
                    {
                        val = PreferedIswcType.Different;
                    }
                    else if (submission.IsEligible
                        && submission.Model.PreferredIswc != submission.MatchedResult.Matches?.FirstOrDefault().Numbers.FirstOrDefault(x => x.Type == "ISWC")?.Number)
                    {
                        val = PreferedIswcType.Different;
                    }
                }
                if (submission.TransactionType == TransactionType.CUR && submission.IsrcMatchedResult?.Matches.Count() > 0 && submission.RequestType == RequestType.Label)
                {
                    if (!submission.IsEligible
                        && (submission.Model.PreferredIswc != submission.IsrcMatchedResult.Matches?.FirstOrDefault(
                        x => x.Numbers.Any(n => n.Number == submission.Model.WorkNumber.Number))?.Numbers.FirstOrDefault(i => i.Type == "ISWC")?.Number))
                    {
                        val = PreferedIswcType.Different;
                    }
                    else if (submission.IsEligible
                        && submission.Model.PreferredIswc != submission.IsrcMatchedResult.Matches?.FirstOrDefault().Numbers.FirstOrDefault(x => x.Type == "ISWC")?.Number)
                    {
                        val = PreferedIswcType.Different;
                    }
                }

                return val;
            }
        }
    }
}
