using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.Notification
{
    public class NotificationDto
    {
        public List<string> ToUserGuids { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public NotificationData NotificationData { get; set; }

    }
}