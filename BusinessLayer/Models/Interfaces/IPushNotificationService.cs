using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public interface IPushNotificationService : INotificationService
    {
        void SetVapidSettings(IVapidSettings vapidSettings);
        void SetVapidSettings(string subject, string privateKey, string publicKey);
        void SetPushSubscription(IPushSubscription pushSubscription);
        void SetPushSubscription(string endpoint, string key, string authSecret);
        bool SendPushNotification(INotification notification);
    }
}
