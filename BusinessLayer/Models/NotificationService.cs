using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class NotificationService : INotificationService
    {
        private List<INotificationService> _notificationServiceList = new List<INotificationService>();

        public NotificationService(INotificationService notificationService)
        {
            _notificationServiceList.Add(notificationService);
        }

        public NotificationService(List<INotificationService> notificationServiceList)
        {
            _notificationServiceList = notificationServiceList;
        }

        public void AddNotificationService(INotificationService notificationService)
        {
            _notificationServiceList.Add(notificationService);
        }

        public bool SendReportMatchNotification(INotification notification)
        {
            foreach(INotificationService service in _notificationServiceList)
            {
                if (service.SendReportMatchNotification(notification))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
