using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Business.Validators;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.LookupDataValidator
{
    public interface ILookupDataValidator : IValidator
    {
    }

    public class LookupDataValidator : BaseValidator, ILookupDataValidator
    {
        public LookupDataValidator(IRulesManager rulesManager) : base(rulesManager, ValidatorType.LookupDataValidator)
        {
        }
    }
}
