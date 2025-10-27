using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class WebUser
    {
        public WebUser()
        {
            WebUserRole = new HashSet<WebUserRole>();
        }

        public int WebUserId { get; set; }
        public string Email { get; set; }
        public string AgencyId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual ICollection<WebUserRole> WebUserRole { get; set; }
    }
}
