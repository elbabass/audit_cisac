using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class PortalRoleType
    {
        public PortalRoleType()
        {
            WebUserRole = new HashSet<WebUserRole>();
        }

        public int PortalRoleTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WebUserRole> WebUserRole { get; set; }
    }
}
