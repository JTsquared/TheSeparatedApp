using BusinessLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebPush;

namespace BusinessLayer.Models
{
    public class PushNotificationService : PushNotificationServiceBase
    {
        private readonly WebPushClient _webPushClient;
        private PushSubscription _pushSubscription;
        private VapidDetails _vapidDetails;
        public PushNotificationService(IVapidSettings vapidSettings, IPushSubscription pushSubscription) : base(vapidSettings, pushSubscription)
        {
            _webPushClient = new WebPushClient();
            SetVapidSettings(vapidSettings);
            SetPushSubscription(pushSubscription);
        }

        public override void SetVapidSettings(IVapidSettings vapidSettings)
        {
            _vapidDetails = new VapidDetails(vapidSettings.Subject, vapidSettings.PublicKey, vapidSettings.PrivateKey);
            _webPushClient.SetVapidDetails(_vapidDetails);
        }

        public override void SetVapidSettings(string subject, string privateKey, string publicKey)
        {
            _vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            _webPushClient.SetVapidDetails(_vapidDetails);
        }

        public override void SetPushSubscription(IPushSubscription pushSubscription)
        {
            _pushSubscription = new PushSubscription(pushSubscription.Endpoint, pushSubscription.Key, pushSubscription.AuthSecret);
        }

        public override void SetPushSubscription(string endpoint, string key, string authSecret)
        {
            _pushSubscription = new PushSubscription(endpoint, key, authSecret);
        }

        public override bool SendPushNotification(INotification notification)
        {
            var response = SendPushNotificationAsync(notification);
            return response.Result == 0 ? true : false;
        }

        public async Task<int> SendPushNotificationAsync(INotification notification)
        {
            try
            {
                await _webPushClient.SendNotificationAsync(_pushSubscription, JsonConvert.SerializeObject(notification), _vapidDetails);
            }
            catch(WebPushException exception)
            {
                var statusCode = exception.StatusCode;
                return (int)statusCode;
            }

            return 0;   
        }
    }
}
