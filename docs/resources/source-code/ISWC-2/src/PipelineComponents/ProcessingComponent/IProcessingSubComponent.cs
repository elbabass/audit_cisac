using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.PipelineComponents.ProcessingComponent.Processing
{
    public interface IProcessingSubComponent
    {
        string Identifier { get; }
        IEnumerable<TransactionType> ValidTransactionTypes { get; }
        bool? IsEligible { get; }
        PreferedIswcType PreferedIswcType { get; }
		string PipelineComponentVersion { get; }
        IEnumerable<RequestType> ValidRequestTypes { get; }
		Task<IswcModel> ProcessSubmission(Submission submission);
    }

    public enum PreferedIswcType
    {
        Different,
        Existing,
        New
    }
}
