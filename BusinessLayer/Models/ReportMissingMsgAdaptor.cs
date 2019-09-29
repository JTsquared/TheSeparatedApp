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
        public byte ReporterContactType { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Location Location { get; set; }

        public ReportMissingMsgAdaptor() { }

        public ReportMissingMsgAdaptor(ReportMissingMsg report)
        {
            DependentName = report?.DependentName ?? String.Empty;
            DependentImgURL = report?.DependentImgURL ?? String.Empty;
            ReporterName = report?.Reporter?.Name ?? String.Empty;
            ReporterPhoneNumber = report?.Reporter?.PhoneNumber ?? String.Empty;
            ReporterContactType = (byte)report?.Reporter?.ContactType;
            Longitude = (double)report?.Location?.Coordinates?.Longitude;
            Latitude = (double)report?.Location?.Coordinates?.Latitude;

            this.PartitionKey = ReporterContactType.ToString();
            this.RowKey = DependentImgURL;
        }
    }
}
