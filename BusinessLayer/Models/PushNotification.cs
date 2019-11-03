using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class PushNotification : IPushSubscription
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string AuthSecret { get; set; }

        public PushNotification() { }

        PushNotification(string endpoint, string key, string authSecret)
        {
            this.Endpoint = endpoint;
            this.Key = key;
            this.AuthSecret = authSecret;
        }
    }
}
