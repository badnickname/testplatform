using System;
using System.Net;
using System.Net.Mail;

namespace WebApplication.Database.Register
{
    public class MailSender : ICloneable
    {
        public string From { get; set; }
        public string Body { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public string Address { get; set; }
        public string DisplayName { get; set; }
        
        public void Send(string mail, string code)
        {
            var from = new MailAddress(From, DisplayName);
            var to = new MailAddress(mail);
            var m = new MailMessage(from, to)
            {
                Subject = Title,
                IsBodyHtml = true,
                Body = Body + $"<a href={Address}?code={code}>{Address}?code={code}</a>"
            };

            var smtp = new SmtpClient(Host, Port)
            {
                Credentials = new NetworkCredential(UserName, Password), EnableSsl = true
            };
            smtp.Send(m);
        }

        public object Clone()
        {
            return new MailSender
            {
                From = From, 
                Body = Body, 
                Host = Host, 
                Port = Port, 
                Title = Title, 
                UserName = UserName, 
                Password = Password, 
                Address = Address, 
                DisplayName = DisplayName
            };
        }
    }
}