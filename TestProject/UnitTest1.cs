using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using WebApplication.Controllers;
using WebApplication.Database;
using WebApplication.Models.Collections;

namespace TestProject
{
    public class Tests
    {
        private TestApiController Controller;
        
        [SetUp]
        public void Setup()
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Controller = new TestApiController();
            ContextBuilder.Init(conf["Settings:IP"],conf["Settings:User"], conf["Settings:Pass"],conf["Settings:Database"]);
        }

        [Test]
        public void CreateSession()
        {
            var session = Controller.CreateSession("sanya", "123");
            Assert.AreEqual("sanya", session.Name);
            Assert.IsNotNull(session.SessionKey);
        }
        
        [Test]
        public void FailedCreateSession()
        {
            var session = Controller.CreateSession("sanya123", "123");
            Assert.IsNotNull(session);
            Assert.AreEqual(SessionContent.Error, session);
        }
        
        [Test]
        public void StopSession()
        {
            var session = Controller.CreateSession("sanya", "123");
            Assert.IsTrue(Controller.IsSessionValid(session.Name, session.SessionKey));
            Controller.StopSession(session.Name, session.SessionKey);
            Assert.IsFalse(Controller.IsSessionValid(session.Name, session.SessionKey));
        }
        
        [Test]
        public void FailedStopSession()
        {
            var session = Controller.CreateSession("sanya", "123");
            Assert.IsTrue(Controller.IsSessionValid(session.Name, session.SessionKey));
            Controller.StopSession(session.Name, "randomkey");
            Assert.IsTrue(Controller.IsSessionValid(session.Name, session.SessionKey));
        }
        
        [Test]
        public void DeniedAcceptTest()
        {
            var test0 = Controller.GetTest(7, "", "");
            var test1 = Controller.GetTest(7, "sanya", "213");
            var test2 = Controller.GetTest(7, "sanya123", "213");
            Assert.AreEqual(TestContent.Error, test0);
            Assert.AreEqual(TestContent.Error, test1);
            Assert.AreEqual(TestContent.Error, test2);
        }
        
        [Test]
        public void AllowAcceptTest()
        {
            var session = Controller.CreateSession("sanya", "123");
            var test = Controller.GetTest(7, session.Name, session.SessionKey);
            Assert.AreNotEqual(TestContent.Error, test);
            Assert.IsTrue(test.Asks.Length == 3);
        }

        [Test]
        public void CheckTestContent()
        {
            var test = Controller.GetTest(6, "", "");
            Assert.AreEqual("Пример", test.Name);
            Assert.AreEqual("демонстрация функционала платформы", test.Description);
            var asks = new[] {16, 17, 18};
            var answers = new[] {46, 47, 48, 49, 50, 51, 56};
            foreach (var ask in test.Asks)
            {
                Assert.IsTrue(asks.Contains(ask.Id));
                foreach (var answer in ask.Answers)
                {
                    Assert.IsTrue(answers.Contains(answer.Id));
                }
            }
            Assert.IsTrue(asks.Length == test.Asks.Length);
        }

        [Test]
        public void CheckSendTest()
        {
            var asks = new[] {16, 17, 18};
            var answers = new[] {46, 48, 51};
            const int tid = 6;
            const string user = "";
            const string key = "";
            var result = Controller.SendTest(tid, asks, answers, user, key);
            Assert.AreNotEqual(ResultContent.Error, result);
            Assert.AreEqual(0, result.MaxValue);
            Assert.AreEqual(tid, result.TestId);
            Assert.AreEqual(0, result.Value);
            Assert.AreEqual("", result.Description);
        }
    }
}