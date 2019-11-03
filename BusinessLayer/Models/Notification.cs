using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class Notification : INotification
    {
        public string Message { get; set; }
    }
}
