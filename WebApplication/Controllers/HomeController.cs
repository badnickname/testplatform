using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Register;
using WebApplication.Model;
using WebApplication.Model.Data;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var context = DatabaseWrapper.Context;
            SessionController.GetUserName(this);
            ViewData["Title"] = "Главная страница";

            var tests = new List<Test>();
            var records = context.Tests.Where(i=> i.Show > 0);
            int i = 0;
            foreach (var r in records)
            {
                if (i == 26) break;
                tests.Add(r);
                i++;
            }

            ViewData["Tests"] = tests;
            
            return View();
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
            string name = SessionController.TrimName(Request.Query["usr"]).MakeSafe();
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