using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Models
{
    public class SmtpSettings
    {
        public string SMTPServer { get; set; }
        public string SMTPEmailAddress { get; set; }
        public string SMTPEmailPassword { get; set; }
        public string JWTSecret { get; set; }
    }
}
