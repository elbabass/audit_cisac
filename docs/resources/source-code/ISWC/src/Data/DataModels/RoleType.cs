using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class RoleType
    {
        public RoleType()
        {
            CisacRoleMapping = new HashSet<CisacRoleMapping>();
            Creator = new HashSet<Creator>();
            Publisher = new HashSet<Publisher>();
        }

        public int RoleTypeId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public string Description { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual ICollection<CisacRoleMapping> CisacRoleMapping { get; set; }
        public virtual ICollection<Creator> Creator { get; set; }
        public virtual ICollection<Publisher> Publisher { get; set; }
    }
}
