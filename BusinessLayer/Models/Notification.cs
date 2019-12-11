using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class Notification : INotification
    {
        public Notification(string message, ReportMissingMsg model)
        {
            Title = "We found a match!";
            Message = message;
            Data = JsonConvert.SerializeObject(model);
            Model = model;
        }

        public Notification(string title, string message, ReportMissingMsg model)
        {
            Title = title;
            Message = message;
            Data = JsonConvert.SerializeObject(model);
            Model = model;
        }

        public string Title { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public ReportMissingMsg Model { get; set; }
    }
}
