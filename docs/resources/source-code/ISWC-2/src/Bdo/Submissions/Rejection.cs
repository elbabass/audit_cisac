using SpanishPoint.Azure.Iswc.Bdo.Rules;

namespace SpanishPoint.Azure.Iswc.Bdo.Submissions
{
    public class Rejection
    {

        public Rejection(ErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorCode Code { get; }
        public string Message { get; } = string.Empty;
    }
}
