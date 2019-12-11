using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public abstract class PushNotificationServiceBase : IPushNotificationService
    {
        private IVapidSettings _vapidSettings;
        private IPushSubscription _pushSubscription;

        public IVapidSettings VapidSettings { get { return _vapidSettings; } }
        public IPushSubscription PushSubscription { get { return _pushSubscription; } }

        public PushNotificationServiceBase(IVapidSettings vapidSettings, IPushSubscription pushNotification)
        {
            _vapidSettings = vapidSettings;
            _pushSubscription = pushNotification;
        }

        public virtual void SetVapidSettings(IVapidSettings vapidSettings)
        {
            _vapidSettings = vapidSettings;
        }

        public virtual void SetVapidSettings(string subject, string privateKey, string publicKey)
        {
            _vapidSettings.Subject = subject;
            _vapidSettings.PrivateKey = privateKey;
            _vapidSettings.PublicKey = publicKey;
        }

        public virtual void SetPushSubscription(IPushSubscription pushSubscription)
        {
            _pushSubscription = pushSubscription;
        }

        public virtual void SetPushSubscription(string endpoint, string key, string authSecret)
        {
            _pushSubscription.Endpoint = endpoint;
            _pushSubscription.Key = key;
            _pushSubscription.AuthSecret = authSecret;
        }

        public abstract bool SendPushNotification(INotification notification);

        public abstract bool SendReportMatchNotification(INotification notification);
    }
}
