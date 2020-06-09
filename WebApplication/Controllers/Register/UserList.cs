using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using WebApplication.Model;
using WebApplication.Modules.Register;

namespace WebApplication.Controllers.Register
{
    public static class UserList
    {
        private static readonly List<User> Users = new List<User>();

        private static string GenerateCode()
        {
            Random rnd = new Random();
            var g = rnd.Next(0, 100000);
            var md5Hash = MD5.Create();
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(g.ToString()));
            var hash = Encoding.Default.GetString(data);

            return g.ToString();
        }

        private static void SendMail(string mail, string code)
        {
            var from = new MailAddress("dtestprojectmail@mail.ru", "Online-Testing");
            var to = new MailAddress(mail);
            var m = new MailMessage(from, to)
            {
                Subject = "Регистрация на сайте",
                IsBodyHtml = true,
                Body = $"Для продолжения регистрации пройдите по ссылке: " +
                       $"<a href=https://localhost:5001/Home/ConfirmRegister?code={code}>https://localhost:5001/Home/ConfirmRegister?code={code}</a>"
            };

            var smtp = new SmtpClient("smtp.mail.ru", 25)
            {
                Credentials = new NetworkCredential("dtestprojectmail", "xLREskH28hL8EmT"), EnableSsl = true
            };
            smtp.Send(m);
        }
        
        public static bool Add(string name, string mail, string pass)
        {
            var context = ContextBuilder.Context;
            var count = Users.Count(i => i.name.Equals(name));
            var countDb = context.Users.Count(i => i.Name.Equals(name));
            if (count != 0 || countDb != 0) return false;

            var code = GenerateCode();
            var usr = new User(name, pass, mail, code);
            Users.Add(usr);
            SendMail(mail, code);
            return true;

        }

        public static bool Confirm(string hash)
        {
            var context = ContextBuilder.Context;
            try
            {
                var usr = Users.First(i => i.code == hash);
                if (usr == null) return false;
                Users.Remove(usr);

                var entity = new Model.Data.User
                {
                    Name = usr.name,
                    Mail = usr.mail,
                    Pass = usr.pass
                };
                context.Users.Add(entity);
                context.SaveChanges();

                return true;
            } catch
            {
                return false;
            }
        }
    }
}