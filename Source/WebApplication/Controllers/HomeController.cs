using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Database;
using WebApplication.Database.Register;
using WebApplication.Database.Session;
using WebApplication.Database.Utility;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private const int MainTestsCount = 26;
        public IActionResult Index()
        {
            SessionKeeper.Get(this);
            using var context = ContextBuilder.Context;
            var tests = new List<Test>(context.Tests.Where(r=> r.Show > 0).Take(MainTestsCount));
            
            return View(tests);
        }

        public IActionResult Register(string usr, string mail, string pss)
        {
            ViewBag.Completed = false;

            if (usr == null) return View();
            ViewBag.Successfull = false;
            ViewBag.Mail = mail;
                
            var mailRegex = new Regex("^.+@.+[.].+$");
            usr = usr.TrimName().MakeSafe();
            mail = mail.MakeSafe();
            
            // Add to Registrator list
            if (pss != null && mail != null && usr != null &&
                mailRegex.IsMatch(ViewBag.Mail) && 
                Registrator.Add(usr, mail, pss))
            {
                ViewBag.Successfull = true;
            }

            ViewBag.Completed = true;

            return View();
        }

        public IActionResult ConfirmRegister(string code)
        {
            if (code == null)
            {
                ViewBag.Successfull = false;
                return View();
            }

            ViewBag.Successfull = Registrator.Confirm(code);
            return View();
        }
        
        public IActionResult Login(string name, string pass)
        {
            if (pass == null || name == null)
            {
                return View();
            }

            var code = SessionKeeper.AddSession(name, pass);
            if(code != null) {
                Response.Cookies.Append("Name", name);
                Response.Cookies.Append("SessionKey", code);
            }
            else
            {
                return View();
            }
            
            return Redirect("/Home/Index");
        }

        public IActionResult Logout(string name, string code)
        {
            Response.Cookies.Delete("Name");
            Response.Cookies.Delete("SessionKey");

            if (name == null || code == null)
            {
                return Redirect("/Home/Index");
            }
            
            SessionKeeper.StopSession(name, code);
            return Redirect("/Home/Index");
        }
    }
}