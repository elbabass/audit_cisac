using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator;
using SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator;
using SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator;
using SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Pipelines.ValidationPipeline
{
    public class ValidationPipeline : IValidationPipeline
    {
        private readonly IStaticDataValidator staticDataValidator;
        private readonly IMetadataStandardizationValidator metadataStandardizationValidator;
        private readonly ILookupDataValidator lookupDataValidator;
        private readonly IIswcEligibilityValidator iswcEligibilityValidator;

        public ValidationPipeline(IStaticDataValidator staticDataValidator, IMetadataStandardizationValidator metadataStandardizationValidator,
            ILookupDataValidator lookupDataValidator, IIswcEligibilityValidator iswcEligibilityValidator)
        {
            this.staticDataValidator = staticDataValidator;
            this.metadataStandardizationValidator = metadataStandardizationValidator;
            this.lookupDataValidator = lookupDataValidator;
            this.iswcEligibilityValidator = iswcEligibilityValidator;
        }

        public async Task<IEnumerable<Submission>> RunPipeline(IEnumerable<Submission> submissions)
        {
            while (submissions.Any(s => s.ToBeProcessed))
            {
                SetSubmissionsAsProcessed(submissions);

                submissions = await staticDataValidator.ValidateBatch(submissions);
            }

            submissions = await metadataStandardizationValidator.ValidateBatch(submissions);
            submissions = await lookupDataValidator.ValidateBatch(submissions);
            return await iswcEligibilityValidator.ValidateBatch(submissions);

            static void SetSubmissionsAsProcessed(IEnumerable<Submission> submissions)
            {
                foreach (var submission in submissions) submission.ToBeProcessed = false;
            }
        }
    }
}