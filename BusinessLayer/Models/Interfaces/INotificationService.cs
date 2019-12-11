using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public interface INotificationService
    {
        bool SendReportMatchNotification(INotification notification);
    }
}
