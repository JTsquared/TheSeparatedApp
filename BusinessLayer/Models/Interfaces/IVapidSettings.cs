using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public interface IVapidSettings
    {
        string Subject { get; set; }
        string PublicKey { get; set; }
        string PrivateKey { get; set; }
    }
}
