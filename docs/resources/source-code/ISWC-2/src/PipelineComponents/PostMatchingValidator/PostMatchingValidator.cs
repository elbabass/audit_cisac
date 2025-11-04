using System.Diagnostics.CodeAnalysis;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Business.Validators;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.PostMatchingValidator
{
    public interface IPostMatchingValidator : IValidator
    {
    }
    [ExcludeFromCodeCoverage]
    public class PostMatchingValidator : BaseValidator, IPostMatchingValidator
    {
        public PostMatchingValidator(IRulesManager rulesManager) : base(rulesManager, ValidatorType.PostMatchingValidator)
        {
        }
    }
}
