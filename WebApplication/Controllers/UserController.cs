using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Register;
using WebApplication.Model;
using WebApplication.Model.Data;

namespace WebApplication.Controllers
{
    public class UserController : Controller
    {
        // GET
        public IActionResult Profile()
        {
            var context = DatabaseWrapper.Context;
            var user = SessionController.GetUserName(this);
            
            if (!Request.Cookies.ContainsKey("Name")) return Redirect("/Home/Index");
            var pass = SessionList.GetPassword(Request.Cookies["Name"], Request.Cookies["SessionKey"]);
            if (pass == null)
            {
                return Redirect("/Home/Index");
            }
            
            ViewData["Password"] = pass;
            ViewData["HideBar"] = true;
            
            ViewData["Title"] = $"Профиль пользователя {Request.Cookies["Name"]}";
            ViewData["Tests"] = new List<Test>(context.Tests.Where(i => i.OwnerId == user.Id));
            
            return View();
        }

        [HttpPost]
        public IActionResult LoadAvatar(ImageData imageData)
        {
            var context = DatabaseWrapper.Context;
            if (imageData.Image == null) return Redirect("/User/Profile");

            byte[] data = null;
            using (var binaryReader = new BinaryReader(imageData.Image.OpenReadStream()))
            {
                data = binaryReader.ReadBytes((int)imageData.Image.Length);
            }

            string name = Request.Cookies["Name"];
            string pass = SessionList.GetPassword(Request.Cookies["Name"], Request.Cookies["SessionKey"]);
            var usr = context.Users.First(i => i.Name == name && i.Pass == pass);
            usr.Photo = data;
            context.Users.Update(usr);
            context.SaveChanges();
            
            return Redirect("/User/Profile");
        }

        public IActionResult Index()
        {
            var context = DatabaseWrapper.Context;
            SessionController.GetUserName(this);
            if (!Request.Query.ContainsKey("id")) return Redirect("/User/Profile");

            var id = int.Parse(Request.Query["id"]);
            var user = context.Users.First(i=>i.Id == id);

            var groupslist = new List<GroupList>();
            
            var glist = context.GroupsList.Where(i => i.UserId == id);
            groupslist.AddRange(glist);

            var groups = groupslist.Select(g => context.Groups.First(i => i.Id == g.GroupId)).ToList();

            ViewData["user"] = user;
            ViewData["groups"] = groups;
            
            return View();
        }
    }
}