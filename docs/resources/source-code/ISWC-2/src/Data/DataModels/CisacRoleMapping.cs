using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class CisacRoleMapping
    {
        public int CisacRoleMappingId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public int RoleTypeId { get; set; }
        public string CisacRoleType { get; set; }

        public virtual User LastModifiedUser { get; set; }
        public virtual RoleType RoleType { get; set; }
    }
}
