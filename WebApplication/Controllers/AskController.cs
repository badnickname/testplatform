using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Model;
using WebApplication.Model.Data;

namespace WebApplication.Controllers
{
    public class AskController : Controller
    {
        [HttpGet]
        public IActionResult Edit(int id, string value)
        {
            var context = DatabaseWrapper.Context;
            
            // Проверка на доступность пользователю
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var ask = context.Asks.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == ask.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            ask.Value = value;
            context.Asks.Update(ask);
            context.SaveChanges();

            return Redirect($"/Ask/Index?id={id}");
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            var context = DatabaseWrapper.Context;
            
            // Проверка на доступность пользователю
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var ask = context.Asks.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == ask.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            var answers = new List<Answer>(context.Answers.Where(i => i.AskId == ask.Id));
            ViewData["ask"] = ask;
            ViewData["answers"] = answers;

            return View();
        }

        [HttpGet]
        public IActionResult Create(int tid)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");

            var test = context.Tests.First(i => i.Id == tid && i.OwnerId == userData.Id);
            var ask = new Ask {TestId = test.Id, Value = "Empty"};
            context.Asks.Add(ask);
            context.SaveChanges();

            return Redirect($"/Test/Create?id={tid}");
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var ask = context.Asks.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == ask.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            context.Asks.Remove(ask);
            context.SaveChanges();
            
            return Redirect($"/Test/Create?id={ask.TestId}");
        }

        [HttpGet]
        public IActionResult AnswerAdd(int id)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var ask = context.Asks.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == ask.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            var answer = new Answer {Value = "Вариант ответа", Impact = 0, AskId = ask.Id, TestId = ask.TestId};
            context.Answers.Add(answer);
            context.SaveChanges();

            return Redirect($"/Ask/Index?id={id}");
        }

        [HttpPost]
        public IActionResult AnswerRemove(int id)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var answer = context.Answers.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == answer.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            context.Answers.Remove(answer);
            context.SaveChanges();

            return Redirect($"/Ask/Index?id={answer.AskId}");
        }

        [HttpPost]
        public IActionResult AnswerEdit(int id, string value, int impact)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var answer = context.Answers.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == answer.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            answer.Impact = impact;
            answer.Value = value;
            context.Answers.Update(answer);
            context.SaveChanges();
            
            return Redirect($"/Ask/Index?id={answer.AskId}");
        }
        
        [HttpPost]
        public IActionResult LoadPhoto(ImageData imageData, int id)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var ask = context.Asks.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == ask.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");
            
            if (imageData.Image == null) return Redirect($"/Ask/Index?id={ask.Id}");

            byte[] data = null;
            using (var binaryReader = new BinaryReader(imageData.Image.OpenReadStream()))
            {
                data = binaryReader.ReadBytes((int)imageData.Image.Length);
            }

            ask.Photo = data;
            context.Asks.Update(ask);
            context.SaveChanges();

            return Redirect($"/Ask/Index?id={ask.Id}");
        }
        
        [HttpGet]
        public IActionResult RemovePhoto(int id)
        {
            var context = DatabaseWrapper.Context;
            var userData = SessionController.GetUserName(this);
            if (userData.Id < 0) return Redirect("/Home/Index");
            var ask = context.Asks.First(i => i.Id == id);
            if (context.Tests.Count(i => i.Id == ask.TestId && i.OwnerId == userData.Id) < 1)
                return Redirect("/Home/Index");

            ask.Photo = null;
            context.Asks.Update(ask);
            context.SaveChanges();

            return Redirect($"/Ask/Index?id={id}");
        }
    }
}