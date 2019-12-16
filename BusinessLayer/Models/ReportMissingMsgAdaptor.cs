using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class ReportMissingMsgAdaptor : TableEntity
    {
        public string DependentName { get; set; }
        public string DependentImgURL { get; set; }
        public string ReporterName { get; set; }
        public string ReporterPhoneNumber { get; set; }
        public string ReporterEmailAddress { get; set; }
        public byte ReporterContactType { get; set; }
        public string NotificationTypePreference { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Geolocator Location { get; set; }
        public bool ShareLocation { get; set; }
        public string ReporterEndpoint { get; set; }
        public string ReporterKey { get; set; }
        public string ReporterAuthSecret { get; set; }

        public ReportMissingMsgAdaptor() { }

        public ReportMissingMsgAdaptor(ReportMissingMsg report)
        {
            DependentName = report?.DependentName ?? String.Empty;
            DependentImgURL = report?.DependentImgURL ?? String.Empty;
            ReporterName = report?.Reporter?.Name ?? String.Empty;
            ReporterPhoneNumber = report?.Reporter?.PhoneNumber ?? String.Empty;
            ReporterEmailAddress = report?.Reporter?.Email ?? String.Empty;
            ReporterContactType = (byte)report?.Reporter?.ContactType;
            NotificationTypePreference = report?.Reporter?.NotificationTypePreference.ToString();
            Longitude = (double)report?.Coordinates?.Longitude;
            Latitude = (double)report?.Coordinates?.Latitude;
            ShareLocation = (bool)report?.ShareLocation;
            ReporterEndpoint = report?.PushNotificationEndpoint;
            ReporterKey = report?.PushNotificationKey;
            ReporterAuthSecret = report?.PushNotificationAuthSecret;

            this.PartitionKey = ReporterContactType.ToString();
            this.RowKey = DependentImgURL;
        }
    }
}
