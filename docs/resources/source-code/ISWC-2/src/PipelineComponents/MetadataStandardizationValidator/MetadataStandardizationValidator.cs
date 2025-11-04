using System.Diagnostics.CodeAnalysis;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Business.Validators;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.MetadataStandardizationValidator
{
    public interface IMetadataStandardizationValidator : IValidator
    {
    }
    [ExcludeFromCodeCoverage]
    public class MetadataStandardizationValidator : BaseValidator, IMetadataStandardizationValidator
    {
        public MetadataStandardizationValidator(IRulesManager rulesManager) : base(rulesManager, ValidatorType.MetadataStandardizationValidator)
        {
        }
    }
}
