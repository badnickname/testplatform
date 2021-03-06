﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Database;
using WebApplication.Database.Session;
using WebApplication.Database.Utility;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class GroupController : Controller
    {
        // GET
        public IActionResult Remove(int? gid)
        {
            using var context = ContextBuilder.Context;
            var userData = SessionKeeper.Get(this);
            if (userData.Id<0 || gid != null) return Redirect("/Home/Index");
            
            var record = context.GroupsList.First(i => i.Id == gid);
            if (record.Owner <= 0) return Redirect("/User/Profile");
            
            var order0 = context.GroupsList.Where(i => i.GroupId == gid);
            var order1 = context.Groups.Where(i => i.Id == gid);
            foreach (var g in order0) context.GroupsList.Remove(g);
            foreach (var g in order1) context.Groups.Remove(g);
            context.SaveChanges();
            
            return Redirect("/User/Profile");
        }

        public IActionResult Create(int? gid, string name, string pass)
        {
            using var context = ContextBuilder.Context;
            SessionKeeper.Get(this, false);
            ViewData["Title"] = "Редактирование группы";

            var userData = SessionKeeper.Get(this);
            if (gid == null)
            {
                ViewData["Title"] = "Создание группы";
                
                var r = new Random();
                var g = r.Next(0, 10000000).ToString();
                context.Groups.Add(new Group{Name=g});
                context.SaveChanges();

                var created = context.Groups.First(i => i.Name == g);
                created.Name = "Новая группа";
                context.Groups.Update(created);

                context.GroupsList.Add(new GroupList{GroupId = created.Id, Owner = 1, UserId = userData.Id});
                context.SaveChanges();

                ViewData["gid"] = created.Id;
                ViewData["gName"] = created.Name;
                ViewData["gPass"] = created.Pass;
            }
            else
            {
                var group = context.Groups.First(i => i.Id == gid);

                var allowed =
                    context.GroupsList.Count(i => i.GroupId == gid && i.UserId == userData.Id && i.Owner > 0);
                if (allowed < 1) return Redirect("/Home/Index");
                
                // Изменение полей при запросе
                if(name!=null && pass!=null) {
                    group.Name = name.TrimName().MakeSafe();
                    group.Pass = pass;
                
                    context.Groups.Update(group);
                    context.SaveChanges();
                }
                
                ViewData["gid"] = gid;
                ViewData["gName"] = group.Name;
                ViewData["gPass"] = group.Pass;
            }
            
            return View();
        }

        public IActionResult Find(string name)
        {
            using var context = ContextBuilder.Context;
            SessionKeeper.Get(this, false);

            ViewBag.Groups = null;
            if (name is null) return View();
            
            var groups = new List<Group>(context.Groups.Where(i => i.Name == name));
            ViewBag.Groups = groups;
            return View();
        }

        public IActionResult Index(int? gid)
        {
            using var context = ContextBuilder.Context;
            SessionKeeper.Get(this, false);
            
            if (gid is null) return Redirect("/Home/Index");
            var group = context.Groups.First(i => i.Id == gid);

            var needReg = 1;
            try
            {
                var usr = context.Users.First(i => i.Name == (string) ViewData["Name"]);
                var contains = context.GroupsList.Count(i => i.UserId == usr.Id && i.GroupId == gid);
                if (contains > 0) needReg = 2;
            }
            catch
            {
                needReg = 0;
            }

            var glists = new List<GroupList>(context.GroupsList.Where(i => i.GroupId == gid));
            var users = glists.Select(u => context.Users.First(i => i.Id == u.UserId)).ToList();
            var tests = new List<Test>(context.Tests.Where(i => i.Limit == gid));

            ViewData["needReg"] = needReg;
            ViewData["Title"] = group.Name;
            ViewData["gName"] = group.Name;
            ViewData["gUsers"] = users;
            ViewData["gid"] = gid;
            ViewData["tests"] = tests;
            
            return View();
        }

        public IActionResult Login(int gid, string pass)
        {
            using var context = ContextBuilder.Context;
            var userData = SessionKeeper.Get(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            
            var count = context.Groups.Count(i => i.Id == gid && i.Pass == pass);
            if (count > 0)
            {
                var countInList = context.GroupsList.Count(i => i.GroupId == gid && i.UserId == userData.Id);
                if (countInList >= 1) return Redirect($"/Group/Index?gid={gid}");
                
                var record = new GroupList{GroupId = gid, Owner = 0, UserId = userData.Id};
                context.GroupsList.Add(record);
                context.SaveChanges();
                return Redirect($"/Group/Index?gid={gid}");
            }
            
            return View();
        }
        
        public IActionResult Kick(int? gid, int usr)
        {
            using var context = ContextBuilder.Context;
            var userData = SessionKeeper.Get(this);
            if (userData.Id < 0 || gid is null) return Redirect("/Home/Index");

            if(usr == userData.Id) return Redirect($"/Group/Create?gid={gid}");
            
            var countInList = context.GroupsList.Count(i => i.GroupId == gid && i.UserId == userData.Id && i.Owner > 0);
            if (countInList < 1) return Redirect($"/Group/Create?gid={gid}");

            var record = context.GroupsList.First(i => i.UserId == usr && i.UserId != userData.Id);
            context.GroupsList.Remove(record);
            context.SaveChanges();
            
            return Redirect($"/Group/Create?gid={gid}");
        }

        public IActionResult KickSelf(int gid)
        {
            using var context = ContextBuilder.Context;
            try
            {
                var userData = SessionKeeper.Get(this);
                if (userData.Id < 0) return Redirect("/Home/Index");

                var record =
                    context.GroupsList.First(i => i.UserId == userData.Id && i.Owner < 1 && i.GroupId == gid);
                context.GroupsList.Remove(record);
                context.SaveChanges();
                return Redirect($"/Group/Index?gid={gid}");
            }
            catch
            {
                return Redirect($"/User/Profile");
            }
        }
    }
}