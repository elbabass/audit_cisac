using System;
using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Data.DataModels
{
    public partial class NotificationType
    {
        public NotificationType()
        {
            Notification = new HashSet<Notification>();
        }

        public int NotificationTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Notification> Notification { get; set; }
    }
}
