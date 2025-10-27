using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Validators
{
    public abstract class BaseValidator : IValidator
    {
        private readonly IRulesManager rulesManager;
        private readonly ValidatorType validatorType;

        public BaseValidator(IRulesManager rulesManager, ValidatorType validatorType)
        {
            this.rulesManager = rulesManager;
            this.validatorType = validatorType;
        }

        public async Task<IEnumerable<Submission>> ValidateBatch(IEnumerable<Submission> batch)
        {
            var returnSubmissions = new List<Submission>();
            var rules = (await rulesManager.GetEnabledRules<IRule>());

            foreach (var submission in batch)
            {
                if (submission.Rejection == null && !submission.SkipProcessing)
                    returnSubmissions.Add(await ValidateSubmission(submission));
                else
                    returnSubmissions.Add(submission);
            }

            var batchRules = (await rulesManager.GetEnabledRules<IBatchRule>());

            if (batchRules.Any())
            {
                return await ValidateSubmissionBatch(returnSubmissions);
            }

            return returnSubmissions;

            async Task<IEnumerable<Submission>> ValidateSubmissionBatch(IEnumerable<Submission> batch)
            {
                var submissionBatchRules = batchRules
                    .Where(rule => rule.ValidatorType == validatorType)
                    .OrderBy(x => x.Identifier);

                foreach (var batchRule in submissionBatchRules)
                {
                    if (!batch.Any(s => batchRule.ValidTransactionTypes.Contains(s.TransactionType)))
                        continue;

                    batch = await batchRule.IsValid(batch);
                }

                return batch;
            }

            async Task<Submission> ValidateSubmission(Submission submission)
            {
                var submissionRules = rules
                    .Where(rule => rule.ValidTransactionTypes.Contains(submission.TransactionType))
                    .Where(rule => rule.ValidatorType == validatorType)
                    .OrderBy(x => x.Identifier);

                foreach (var rule in submissionRules)
                {
                    if (!rule.ValidTransactionTypes.Contains(submission.TransactionType))
                        continue;

                    var sw = Stopwatch.StartNew();
                    var result = await rule.IsValid(submission);
                    submission.RulesApplied.Add(new RuleExecution
                    {
                        RuleName = rule.Identifier,
                        RuleVersion = rule.PipelineComponentVersion,
                        RuleConfiguration = rule.RuleConfiguration,
                        TimeTaken = sw.Elapsed
                    });

                    if (!result.IsValid)
                        return result.Submission;
                }

                return submission;
            }
        }
    }
}
