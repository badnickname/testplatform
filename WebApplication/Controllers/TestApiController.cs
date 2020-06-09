using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplication.Database;
using WebApplication.Database.Register;
using WebApplication.Models.Collections;

namespace WebApplication.Controllers
{
    //[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class TestApiController : ApiController
    {
        public TestContent GetTest(int id, string user, string sessionkey)
        {
            try
            {
                var usrData = new List<Session>(SessionList.Sessions.Where(i => i.Name == user && i.Key == sessionkey));
                var uid = usrData.Count > 0 ? usrData.First().Id : -1;
                var test = FreqRequests.GetTestContent(id, uid);
                return test;
            }
            catch
            {
                return TestContent.Error;
            }
        }

        [HttpPost]
        public ResultContent SendTest(int tid, int[] AskId, int[] AnswerId, string user, string sessionkey)
        {
            try
            {
                var usrData = new List<Session>(SessionList.Sessions.Where(i => i.Name == user && i.Key == sessionkey));
                var uid = usrData.Count > 0 ? usrData.First().Id : -1;
                var result = FreqRequests.GetResultContent(tid, AskId, AnswerId, uid);
                return result;
            }
            catch
            {
                return ResultContent.Error;
            }
        }

        public SessionContent CreateSession(string name, string password)
        {
            try
            {
                var code = SessionList.AddSession(name, password);
                return code == null ? SessionContent.Error : new SessionContent {Name = name, SessionKey = code};
            }
            catch
            {
                return SessionContent.Error;
            }
        }

        public void StopSession(string name, string key)
        {
            SessionList.StopSession(name, key);
        }

        public bool IsSessionValid(string name, string key)
        {
            var pass = SessionList.GetPassword(name, key);
            return !(pass is null);
        }
    }
}