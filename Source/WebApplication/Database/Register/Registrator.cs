using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace WebApplication.Database.Register
{
    public static class Registrator
    {
        private static readonly List<User> Users = new List<User>();
        private static MailSender _mailSender;
        public static void Init(IConfiguration mail)
        {
            _mailSender = new MailSender
            {
                Body = mail["Mail:Body"],
                From = mail["Mail:From"],
                Title = mail["Mail:Title"],
                Port = int.Parse(mail["Mail:Port"]),
                Host = mail["Mail:Host"],
                UserName = mail["Mail:UserName"],
                Password = mail["Mail:Password"],
                Address = mail["Mail:Address"],
                DisplayName = mail["Mail:DisplayName"]
            };
        }

        public static bool Add(string name, string mail, string pass)
        {
            using var context = ContextBuilder.Context;
            var count = Users.Count(i => i.Name.Equals(name));
            var countDb = context.Users.Count(i => i.Name.Equals(name));
            if (count != 0 || countDb != 0) return false;

            var code = GenerateCode();
            var usr = new User(name, pass, mail, code);
            Users.Add(usr);
            _mailSender.Send(mail, code);
            return true;

        }

        public static bool Confirm(string hash)
        {
            using var context = ContextBuilder.Context;
            try
            {
                var usr = Users.First(i => i.Code == hash);
                if (usr == null) return false;
                Users.Remove(usr);

                var entity = new Models.User
                {
                    Name = usr.Name,
                    Mail = usr.Mail,
                    Pass = usr.Pass
                };
                context.Users.Add(entity);
                context.SaveChanges();

                return true;
            } catch
            {
                return false;
            }
        }
        
        private static string GenerateCode()
        {
            var rnd = new Random();
            var g = rnd.Next(0, 100000);
            var md5Hash = MD5.Create();
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(g.ToString()));

            return g.ToString();
        }
    }
}