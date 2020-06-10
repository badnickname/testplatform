using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Database;
using WebApplication.Database.Register;
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

        public IActionResult Register()
        {
            ViewData["Title"] = "Регистрация";
            ViewData["HideBar"] = true;
            ViewBag.Completed = false;

            if (!Request.Query.ContainsKey("usr")) return View();
            ViewBag.Successfull = false;
            ViewBag.Mail = Request.Query["mail"];
                
            Regex mail = new Regex("^.+@.+[.].+$");
            string name = StringExpansion.TrimName(Request.Query["usr"]).MakeSafe();
            string email = ((string)Request.Query["mail"]).MakeSafe();
            string pass = Request.Query["pss"];
            if (pass != null && email != null && name != null &&
                mail.IsMatch(ViewBag.Mail) && UserList.Add(name, email, pass))
            {
                ViewBag.Successfull = true;
            }

            ViewBag.Completed = true;

            return View();
        }

        public IActionResult ConfirmRegister()
        {
            ViewData["Title"] = "Подтверждение регистрации";
            ViewData["HideBar"] = true;
            string code = Request.Query["code"];
            if (code == null)
            {
                ViewBag.Successfull = false;
                return View();
            }

            ViewBag.Successfull = UserList.Confirm(code);
            return View();
        }
        
        public IActionResult Login()
        {
            ViewData["Title"] = "Вход в профиль";
            ViewData["HideBar"] = true;
            string name = Request.Query["name"];
            string pass = Request.Query["pass"];
            if (pass == null || name == null)
            {
                return View();
            }

            var code = SessionList.AddSession(name, pass);
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

        public IActionResult Logout()
        {
            var name = Request.Cookies["Name"];
            var code = Request.Cookies["SessionKey"];
            Response.Cookies.Delete("Name");
            Response.Cookies.Delete("SessionKey");

            if (name == null || code == null)
            {
                return Redirect("/Home/Index");
            }
            
            SessionList.StopSession(name, code);
            return Redirect("/Home/Index");
        }
    }
}