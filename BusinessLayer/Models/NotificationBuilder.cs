using System;

namespace BusinessLayer.Models
{
    public static class NotificationBuilder
    {
        public static INotification BuildMessage(ReportMissingMsg model, bool canSendReporterLocation)
        {
            //TODO: create Notification object based off of model and user permission to use location
            return new Notification("This is a sweet message", model);
        }
    }
}