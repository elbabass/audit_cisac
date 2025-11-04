using SpanishPoint.Azure.Iswc.Bdo.Edi;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Rules
{
    public interface IBaseRule
    {
        string Identifier { get; }
        string ParameterName { get; }
        IEnumerable<TransactionType> ValidTransactionTypes { get; }
        ValidatorType ValidatorType { get; }
        string PipelineComponentVersion { get; }
        string? RuleConfiguration { get; }
    }
}
