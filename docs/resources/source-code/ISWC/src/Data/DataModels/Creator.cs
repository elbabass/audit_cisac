using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Creator
    {
        public string IpbaseNumber { get; set; }
        public long WorkInfoId { get; set; }
        public bool Status { get; set; }
        public byte[] Concurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int LastModifiedUserId { get; set; }
        public bool IsDispute { get; set; }
        public bool? Authoritative { get; set; }
        public int RoleTypeId { get; set; }
        public long IswcId { get; set; }
        public long? IpnameNumber { get; set; }
        public int CreatorId { get; set; }
        public bool IsExcludedFromIswc { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public virtual InterestedParty IpbaseNumberNavigation { get; set; }
        public virtual Name IpnameNumberNavigation { get; set; }
        public virtual Iswc Iswc { get; set; }
        public virtual User LastModifiedUser { get; set; }
        public virtual RoleType RoleType { get; set; }
        public virtual WorkInfo WorkInfo { get; set; }
    }
}
