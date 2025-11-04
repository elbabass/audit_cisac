using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Portal
{
    public class WebUserRole
    {
        public WebUserRole()
        {
        }

        public bool IsApproved { get; set; }
        public PortalRoleType Role { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool Status { get; set; }
        public Notification? Notification { get; set; }

    }
}
