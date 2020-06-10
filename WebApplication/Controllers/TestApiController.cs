using System.Web.Http;
using WebApplication.Database.Methods;
using WebApplication.Database.Session;
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
                var usrData = SessionKeeper.Get(user, sessionkey);
                var test = FreqRequests.GetTestContent(id, usrData.Id);
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
                var usrData = SessionKeeper.Get(user, sessionkey);
                var result = FreqRequests.GetResultContent(tid, AskId, AnswerId, usrData.Id);
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
                var code = SessionKeeper.AddSession(name, password);
                return code == null ? SessionContent.Error : new SessionContent {Name = name, SessionKey = code};
            }
            catch
            {
                return SessionContent.Error;
            }
        }

        public void StopSession(string name, string key)
        {
            SessionKeeper.StopSession(name, key);
        }

        public bool IsSessionValid(string name, string key)
        {
            var usrData = SessionKeeper.Get(name, key);
            return usrData.Id > 0;
        }
    }
}