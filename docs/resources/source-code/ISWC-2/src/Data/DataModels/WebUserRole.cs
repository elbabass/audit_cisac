using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WebUserRole
    {
        public WebUserRole()
        {
            Notification = new HashSet<Notification>();
        }

        public int WebUserRoleId { get; set; }
        public int WebUserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool Status { get; set; }
        public bool IsApproved { get; set; }

        public virtual PortalRoleType Role { get; set; }
        public virtual WebUser WebUser { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
    }
}
