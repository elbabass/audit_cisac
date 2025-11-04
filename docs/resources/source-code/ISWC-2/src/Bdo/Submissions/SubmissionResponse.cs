using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Submissions
{
    public partial class SubmissionResponse
    {
        public SubmissionResponse()
        {
            LinkedIswcs = new List<Submission>();
        }
        public Submission? VerifiedSubmission { get; set; }
        public List<Submission> LinkedIswcs { get; set; }
    }
}
