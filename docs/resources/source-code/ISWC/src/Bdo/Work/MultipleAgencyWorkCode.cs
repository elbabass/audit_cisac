using SpanishPoint.Azure.Iswc.Bdo.Submissions;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class MultipleAgencyWorkCodes
    {
        public string Agency { get; set; } = string.Empty;

        public string WorkCode { get; set; } = string.Empty;

        public Rejection? Rejection { get; set; }
    }
}
