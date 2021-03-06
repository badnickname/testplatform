﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Database.Session
{
    public class SessionList
    {
        public readonly List<Session> Sessions = new List<Session>();

        private string GenerateCode()
        {
            var rnd = new Random();
            var g = rnd.Next(0, 99990000);
            return g.ToString();
        }
        
        public string AddSession(string name, string pass)
        {
            var context = ContextBuilder.Context;
            StopSession(name, "");
            
            try
            {
                var usr = context.Users.First(i => i.Name == name && i.Pass == pass);

                var code = GenerateCode();
                Sessions.Add(new Session {Id = usr.Id, Key = code, Name = usr.Name});

                return code;
            }
            catch
            {
                return null;
            }
        }

        public void StopSession(string name, string code)
        {
            Sessions.RemoveAll(i => i.Name == name && i.Key == code);
        }

        public string GetPassword(string name, string code)
        {
            using var context = ContextBuilder.Context;
            
            try
            {
                var record = Sessions.First(i => i.Name == name && i.Key == code);
                var usr = context.Users.First(i => i.Name == record.Name);
                return usr.Pass;
            }
            catch
            {
                return null;
            }
        }
    }
}