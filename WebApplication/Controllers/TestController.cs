using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Model;
using WebApplication.Model.Data;


namespace WebApplication.Controllers
{
    public class TestController : Controller
    {
        // GET
        public IActionResult Index()
        {
            var context = ContextBuilder.Context;
            var userData = SessionController.GetUserName(this);
            var id = int.Parse(Request.Query["id"]);
            var test = context.Tests.First(i => i.Id == id);

            ViewData["test"] = test;
            ViewData["Title"] = $"Тест - {test.Name}";

            ViewData["tAllow"] = false;
            ViewData["tries"] = 0;
            
            // Проверка на количество попыток
            var triesCount = context.Results.Count(i => i.UserId == userData.Id);
            ViewData["tries"] = triesCount;
            if (test.Tries > 0 && test.Tries - triesCount < 1) return View();
            
            // Проверка на наличие в группе
            var groupCount = context.GroupsList.Count(i => i.UserId == userData.Id && i.GroupId == test.Limit);
            if (groupCount < 1 && test.Limit > 0) return View();
            if (userData.Id < 0 && test.SaveResults > 0) return View();

            // Отображение body в случае прохождения проверок
            ViewData["tAllow"] = true;
            var asks = new List<Ask>();
            asks.AddRange(context.Asks.Where(i => i.TestId == test.Id));

            var answers = new List<Answer>();
            answers.AddRange(context.Answers.Where(i=> i.TestId == test.Id));

            ViewData["tAsks"] = asks;
            ViewData["tAnswers"] = answers;
            
            ViewData["Title"] = $"Тест - {test.Name}";
            
            return View();
        }

        public IActionResult Create()
        {
            var context = ContextBuilder.Context;
            ViewData["Title"] = "Редактирование теста";
            
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");

            if (!Request.Query.ContainsKey("id"))
            {
                ViewData["Title"] = "Создание теста";
                
                // Создание нового теста
                var r = new Random();
                var testname = r.Next(0, 10000).ToString();
                var newtest = new Test{Name = testname, Limit = 0, Info = "", SaveResults = 0, Show = 0, Tries = 0, OwnerId = userData.Id};
                context.Tests.Add(newtest);
                context.SaveChanges();
                
                // Получение нового теста из БД
                var test = context.Tests.First(i => i.Name == testname);
                test.Name = "Новый тест";
                context.Tests.Update(test);
                context.SaveChanges();
                
                return Redirect($"/Test/Create?id={test.Id}");
            }
            
            // Редактирование имеющейся записи
            var id = int.Parse(Request.Query["id"]);
            var tst = context.Tests.First(i => i.Id == id && i.OwnerId == userData.Id);
            
            var changed = false;
            if (Request.Query.ContainsKey("name")) { tst.Name = ((string)Request.Query["name"]).MakeSafe(); changed = true; }
            if (Request.Query.ContainsKey("text")) { tst.Info = ((string)Request.Query["text"]).MakeSafe(); changed = true; }
            if (Request.Query.ContainsKey("tries")) { tst.SaveResults = 1; tst.Tries = int.Parse(Request.Query["tries"]); changed = true; }
            if (Request.Query.ContainsKey("name"))
            {
                tst.SaveResults = Request.Query.ContainsKey("saveres") ? 1 : 0;
                tst.Show = Request.Query.ContainsKey("show") ? 1 : 0;
            }

            if (changed)
            {
                context.Tests.Update(tst);
                context.SaveChanges();
            }

            var asks = new List<Ask>(context.Asks.Where(i => i.TestId == tst.Id));
            var descrs = new List<ResultDescription>(context.Descriptions.Where(i => i.TestId == tst.Id));
            var results = new List<Result>(context.Results.Where(i => i.TestId == tst.Id));
            
            ViewData["asks"] = asks;
            ViewData["test"] = tst;
            ViewData["descrs"] = descrs;
            ViewData["results"] = results;
            ViewData["groupname"] = "";

            // Описание группы, для которой доступен тест
            if (tst.Limit != 0)
            {
                var group = context.Groups.First(i => i.Id == tst.Limit);
                ViewData["groupname"] = group.Name;
            }

            return View();
        }

        public IActionResult SetGroup()
        {
            var context = ContextBuilder.Context;
            ViewData["Title"] = "Сделать доступным для группы...";
            
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            
            var id = int.Parse(Request.Query["id"]);
            var tst = context.Tests.First(i => i.Id == id && i.OwnerId == userData.Id);

            // Вывод списка с доступными группами
            if (Request.Query.ContainsKey("gname"))
            {
                var gname = (string)Request.Query["gname"];
                var groups = new List<Group>(context.Groups.Where(i => i.Name == gname));
                ViewData["groups"] = groups;
                ViewData["test"] = tst;

                return View();
            }

            if (!Request.Query.ContainsKey("gid")) return Redirect($"/Test/Create?id={id}");
            
            // Установка новой группы для теста
            var gid = int.Parse(Request.Query["gid"]);
            tst.Limit = gid;
            context.Tests.Update(tst);
            context.SaveChanges();

            return Redirect($"/Test/Create?id={id}");
        }
        
        [HttpPost]
        public IActionResult LoadPhoto(ImageData imageData, string Id)
        {
            var context = ContextBuilder.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");

            var id = int.Parse(Id);
            var tst = context.Tests.First(i => i.Id == id && i.OwnerId == userData.Id);
            
            if (imageData.Image == null) return Redirect($"/Test/Create?id={id}");

            byte[] data = null;
            using (var binaryReader = new BinaryReader(imageData.Image.OpenReadStream()))
            {
                data = binaryReader.ReadBytes((int)imageData.Image.Length);
            }

            
            tst.Photo = data;
            context.Tests.Update(tst);
            context.SaveChanges();
            
            return Redirect($"/Test/Create?id={id}");
        }

        public IActionResult Remove()
        {
            var context = ContextBuilder.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            
            var id = int.Parse(Request.Query["id"]);
            var tst = context.Tests.First(i => i.Id == id && i.OwnerId == userData.Id);

            context.Tests.Remove(tst);
            context.SaveChanges();

            return Redirect("/User/Profile");
        }

        [HttpGet]
        public IActionResult AddDescription(int tid)
        {
            var context = ContextBuilder.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            
            var test = context.Tests.First(i => i.Id == tid && i.OwnerId == userData.Id);
            var descr = new ResultDescription{MinValue = 0, Text = "", TestId = test.Id};

            context.Descriptions.Add(descr);
            context.SaveChanges();

            return Redirect($"/Test/Create?id={tid}");
        }
        
        [HttpPost]
        public IActionResult RemoveDescription(int id)
        {
            var context = ContextBuilder.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");

            var resdescr = context.Descriptions.First(i => i.Id == id);
            if(context.Tests.Count(i=>i.OwnerId == userData.Id && i.Id == resdescr.TestId) < 1) 
                return Redirect($"/Test/Create?id={resdescr.TestId}");

            context.Descriptions.Remove(resdescr);
            context.SaveChanges();
            
            return Redirect($"/Test/Create?id={resdescr.TestId}");
        }

        [HttpPost]
        public IActionResult EditDescription(int id, string descr, int minv)
        {
            var context = ContextBuilder.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");

            var resdescr = context.Descriptions.First(i => i.Id == id);
            if(context.Tests.Count(i=>i.OwnerId == userData.Id && i.Id == resdescr.TestId) < 1) 
                return Redirect($"/Test/Create?id={resdescr.TestId}");

            resdescr.Text = descr.MakeSafe();
            resdescr.MinValue = minv;
            context.Descriptions.Update(resdescr);
            context.SaveChanges();
            
            return Redirect($"/Test/Create?id={resdescr.TestId}");
        }
        
        [HttpPost]
        public IActionResult Send(int[] AskId, int[] AnswerId)
        {
            var context = ContextBuilder.Context;
            ViewData["error"] = true;
            ViewData["Title"] = "Результаты тестирования";
            
            var userData = SessionController.GetUserName(this);
            if(AskId.Length < 1 || AnswerId.Length < 1) return View();
            
            // Получение теста
            var firstAsk = context.Asks.First(i => i.Id == AskId[0]);
            var test = context.Tests.First(i => i.Id == firstAsk.TestId);
            
            // Контроль доступа
            if(userData.Id < 0 && test.SaveResults > 0) return View();
            if(context.GroupsList.Count(i => i.GroupId == test.Limit && i.UserId == userData.Id) < 0) return View();

            var asks = new List<Ask>(context.Asks.Where(i => i.TestId == test.Id));
            var answers = new List<Answer>(context.Answers.Where(i => i.TestId == test.Id));
            var userAnswers = new Dictionary<int, int>();

            if (asks.Count != AskId.Length) return View();

            // Максимально возможный результат
            var maxResult = 0;
            foreach (var ask in asks)
            {
                var maxImpact = int.MinValue;
                foreach (var a in answers.Where(a => a.AskId == ask.Id && a.Impact > maxImpact))  maxImpact = a.Impact;
                maxResult += maxImpact;
            }
            
            // Результат пользователя
            foreach (var ask in asks)
            {
                int j;
                for (j = 0; j < AskId.Length && AskId[j] != ask.Id; j++);
                var answ = answers.First(i => i.Id == AnswerId[j]);
                userAnswers[ask.Id] = answ.Impact;
            }
            var result = userAnswers.Values.Sum();

            // Отображение описания результата
            var resultText = "";
            var descriptions = context.Descriptions.Where(i => i.TestId == test.Id);
            var maxDescrImpact = int.MinValue;
            foreach (var d in descriptions)
            {
                if (d.MinValue <= maxDescrImpact || result <= d.MinValue) continue;
                maxDescrImpact = d.MinValue;
                resultText = d.Text;
            }

            ViewData["error"] = false;
            ViewData["rText"] = resultText;
            ViewData["res"] = result;
            ViewData["maxres"] = maxResult;

            if (test.SaveResults <= 0) return View();
            
            var record = new Result{TestId = test.Id, UserId = userData.Id, Value = result};
            context.Results.Add(record);
            context.SaveChanges();

            return View();
        }
    }
}