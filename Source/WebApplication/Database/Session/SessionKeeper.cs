using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Database.Utility;

namespace WebApplication.Database.Session
{
    public static class SessionKeeper
    {
        private static readonly SessionList SessionList = new SessionList();

        public static UserData Get(Controller context, bool safe = true)
        {
            return safe ? SafeSession(context) : UnsafeSession(context);
        }
        
        public static UserData Get(string name, string code)
        {
            var usrData = new List<Session>(SessionList.Sessions.Where(i => i.Name == name && i.Key == code));
            var usr = usrData.Count > 0 ? usrData.First() : null;
            var password = SessionList.GetPassword(name, code);
            return usr != null ? new UserData {Id = usr.Id, Name = name, Password = password} : UserData.Empty;
        }
        
        private static UserData UnsafeSession(Controller context)
        {
            var name = context.Request.Cookies["Name"];
            context.ViewData["Name"] = name;
            return UserData.Empty;
        }
        private static UserData SafeSession(Controller ctxtController)
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

        public static void StopSession(string name, string code)
        {
            SessionList.StopSession(name, code);
        }

        public static string AddSession(string name, string pass)
        {
            return SessionList.AddSession(name, pass);
        }

        public static void Logout(Controller context)
        {
            context.Response.Cookies.Delete("Name");
            context.Response.Cookies.Delete("SessionKey");
        }
    }
}