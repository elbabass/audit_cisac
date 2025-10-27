using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Agent
{
    public class AgentNotification
    {
        public AgentNotification()
        {
            CsnNotifications = new List<CsnNotification>();
        }

        public string ContinuationToken { get; set; } = string.Empty;
        public IEnumerable<CsnNotification> CsnNotifications { get; set; }
        public Rejection? Rejection { get; set; }
    }
}
