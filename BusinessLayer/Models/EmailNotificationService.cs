using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IdentityModel.Tokens.Jwt;

namespace BusinessLayer.Models
{
    public class EmailNotificationService : INotificationService
    {
        private string _smtpClient;
        private string _username;
        private string _password;
        private string _toAddress;
        private string _fromAddress;
        private string _subject;
        private string _body;
        private string _jwtSecret;

        public EmailNotificationService(string smtpClient, string username, string password, string toAddress, string fromAddress, string jwtSecret)
        {
            _smtpClient = smtpClient;
            _toAddress = toAddress;
            _fromAddress = fromAddress;
            _username = username;
            _password = password;
            _jwtSecret = jwtSecret;
        }

        public void SetEmailRecipients(string RecipientEmailAddress)
        {
            _toAddress = RecipientEmailAddress;
        }

        public bool SendReportMatchNotification(INotification notification)
        {
            JWTTokenService tokenService = new JWTTokenService(_jwtSecret);
            var secToken = tokenService.GetSecurityToken(notification.Data);

            StringBuilder message = new StringBuilder();
            message.AppendLine(notification.Title);
            message.AppendLine(notification.Message);
            message.AppendLine("<br /><a href=" + notification.RedirectURL + secToken + "\">See Report Results</a>");

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient(_smtpClient);
                mail.From = new MailAddress(_fromAddress);
                mail.To.Add(_toAddress);
                mail.Subject = notification.Title;
                mail.IsBodyHtml = true;
                mail.Body = message.ToString();

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(_username, _password);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                return true;
            }
            catch(Exception e)
            {
                //TODO: log to logger
                return false;
            }
        }
    }
}
