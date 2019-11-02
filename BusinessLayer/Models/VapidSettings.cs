using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class VapidSettings
    {
        public string Subject { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

    }
}
