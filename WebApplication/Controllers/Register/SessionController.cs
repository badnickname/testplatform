using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Register;
using WebApplication.Model;

namespace WebApplication.Controllers
{
    public static class SessionController
    {
        public static void GetNotStrongName(Controller context)
        {
            string name = context.Request.Cookies["Name"];
            context.ViewData["Name"] = name;
        }
        public static UserData GetUserName(Controller ctxtController)
        {
            var context = ContextBuilder.Context;
            var name = ctxtController.Request.Cookies["Name"];
            var code = ctxtController.Request.Cookies["SessionKey"];
            try
            {
                var usr = SessionList.Sessions.First(i => i.Name == name && i.Key == code);
                var record = context.Users.First(i => i.Name == usr.Name);
                ctxtController.ViewData["Name"] = usr.Name;
                return new UserData{Name=record.Name, Password = record.Pass, Id = record.Id};
            }
            catch
            {
                ctxtController.Response.Cookies.Delete("Name");
                ctxtController.Response.Cookies.Delete("SessionKey");
                return UserData.Empty;
            }
        }

        public static string TrimName(string name)
        {
            name = name.Trim();
            name = name.Replace("<", "");
            name = name.Replace(">", "");
            if (name.Length < 1) name = "0";
            return name;
        }

        public static void Logout(Controller context)
        {
            context.Response.Cookies.Delete("Name");
            context.Response.Cookies.Delete("SessionKey");
        }
    }
}