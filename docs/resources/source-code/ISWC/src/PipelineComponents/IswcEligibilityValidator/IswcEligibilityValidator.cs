using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Business.Validators;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.IswcEligibilityValidator
{
    public interface IIswcEligibilityValidator : IValidator
    {
    }

    public class IswcEligibilityValidator : BaseValidator, IIswcEligibilityValidator
    {
        public IswcEligibilityValidator(IRulesManager rulesManager) : base(rulesManager, ValidatorType.IswcEligibiltyValidator)
        {
        }
    }
}
