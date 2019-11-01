using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class PushNotificationModel
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string AuthSecret { get; set; }

        PushNotificationModel(string endpoint, string key, string authSecret)
        {
            this.Endpoint = endpoint;
            this.Key = key;
            this.AuthSecret = authSecret;
        }
    }
}
