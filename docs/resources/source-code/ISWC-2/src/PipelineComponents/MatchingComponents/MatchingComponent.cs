using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.InitialMatching;
using SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.PostMatching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MatchingComponent
{
    public interface IMatchingComponent
    {
        Task<IEnumerable<Submission>> ProcessBatch(IEnumerable<Submission> batch);
    }

    public class MatchingComponent : IMatchingComponent
    {
        private readonly IServiceProvider serviceProvider;

        public MatchingComponent(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<Submission>> ProcessBatch(IEnumerable<Submission> batch)
        {
            IDictionary<int, Submission> submissionsBeingProcessed = batch.ToDictionary(x => x.SubmissionId);

            await ProcessSubmissions<IInitialMatchingComponent>();
            await ProcessSubmissions<IPostMatchingComponent>();

            return submissionsBeingProcessed.Values;

            async Task ProcessSubmissions<T>() where T : IMatchingSubComponent
            {
                foreach (var component in AppDomain.CurrentDomain.GetComponentsOfType<T>(serviceProvider))
                {
                    var sw = Stopwatch.StartNew();

                    var submissions = submissionsBeingProcessed.Values.ToList()
                        .Where(sub => sub.Rejection == null && component.ValidTransactionTypes.Contains(sub.TransactionType)
                        && (!component.IsEligible.HasValue || component.IsEligible.Value == sub.IsEligible)
                        && component.ValidRequestTypes.Contains(sub.RequestType));

                    if (!submissions.Any()) continue;

                    foreach (var submission in await component.ProcessSubmissions(submissions))
                    {
                        submission.RulesApplied.Add(new RuleExecution
                        {
                            RuleName = component.Identifier,
                            RuleVersion = component.PipelineComponentVersion,
                            TimeTaken = sw.Elapsed
                        });

                        submissionsBeingProcessed[submission.SubmissionId] = submission;
                    }
                }
            }
        }
    }
}
