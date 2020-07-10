using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Database;
using WebApplication.Database.Session;
using WebApplication.Database.Utility;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class UserController : Controller
    {
        // GET
        public IActionResult Profile()
        {
            var context = ContextBuilder.Context;
            var user = SessionKeeper.Get(this);
            
            if (!Request.Cookies.ContainsKey("Name")) return Redirect("/Home/Index");
            if (user.Id < 0)
            {
                return Redirect("/Home/Index");
            }
            
            ViewData["Password"] = user.Password;
            ViewData["HideBar"] = true;
            
            ViewData["Title"] = $"Профиль пользователя {Request.Cookies["Name"]}";
            ViewData["Tests"] = new List<Test>(context.Tests.Where(i => i.OwnerId == user.Id));
            
            return View();
        }

        [HttpPost]
        public IActionResult LoadAvatar(ImageData imageData)
        {
            var context = ContextBuilder.Context;
            if (imageData.Image == null) return Redirect("/User/Profile");

            byte[] data = null;
            using (var binaryReader = new BinaryReader(imageData.Image.OpenReadStream()))
            {
                data = binaryReader.ReadBytes((int)imageData.Image.Length);
            }

            var userdata = SessionKeeper.Get(this);

            var name = userdata.Name;
            var pass = userdata.Password;
            var usr = context.Users.First(i => i.Name == name && i.Pass == pass);
            usr.Photo = data;
            context.Users.Update(usr);
            context.SaveChanges();
            
            return Redirect("/User/Profile");
        }

        public IActionResult Index(int? id)
        {
            var context = ContextBuilder.Context;
            SessionKeeper.Get(this);
            if (id is null) return Redirect("/User/Profile");
            var user = context.Users.First(i=>i.Id == id);

            var groupslist = new List<GroupList>(context.GroupsList.Where(i => i.UserId == id));
            var groups = groupslist.Select(g => context.Groups.First(i => i.Id == g.GroupId)).ToList();

            ViewData["user"] = user;
            ViewData["groups"] = groups;
            
            return View();
        }
    }
}