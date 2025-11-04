using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.Portal
{
    public class WebUser
    {
        public WebUser()
        {
            WebUserRoles = new List<WebUserRole>();
        }

        public string Email { get; set; } = string.Empty;
        public string AgencyId { get; set; } = string.Empty;
        public ICollection<WebUserRole> WebUserRoles { get; set; }
    }
}
