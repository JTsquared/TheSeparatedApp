using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public interface IPushSubscription
    {
        string Endpoint { get; set; }
        string Key { get; set; }
        string AuthSecret { get; set; }
    }
}
