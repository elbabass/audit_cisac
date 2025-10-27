using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int WebUserRoleId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public int NotificationTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual NotificationType NotificationType { get; set; }
        public virtual WebUserRole WebUserRole { get; set; }
    }
}
