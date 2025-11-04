using System.Diagnostics.CodeAnalysis;
using SpanishPoint.Azure.Iswc.Bdo.Rules;
using SpanishPoint.Azure.Iswc.Business.Managers;
using SpanishPoint.Azure.Iswc.Business.Validators;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.StaticDataValidator
{
    public interface IStaticDataValidator : IValidator
    {
    }
    [ExcludeFromCodeCoverage]
    public class StaticDataValidator : BaseValidator, IStaticDataValidator
    {
        public StaticDataValidator(IRulesManager rulesManager) : base(rulesManager, ValidatorType.StaticValidator)
        {
        }
    }
}
