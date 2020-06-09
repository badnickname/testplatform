using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Models.Collections;

namespace WebApplication.Database
{
    public class FreqRequests
    {
        public static TestContent GetTestContent(int id, int uid)
        {
            using var context = ContextBuilder.Context;
            var test = context.Tests.First(i => i.Id == id);

            // Проверка доступности
            if (test.Limit > 0)
            {
                if (uid == -1) return TestContent.Error;
                if (context.GroupsList.Count(i => i.UserId == uid && i.GroupId == test.Limit) < 1) return TestContent.Error;
            }
            if(test.SaveResults > 0 && uid < 0) return TestContent.Error;

            var answers = new List<Answer>(context.Answers.Where(i => i.TestId == test.Id));
            var asks = new List<Ask>(context.Asks.Where(i => i.TestId == test.Id));

            var askContents = asks.Select(
                ask => new AskContent
                {
                    Id = ask.Id, 
                    Value = ask.Value,
                    Answers = answers.Where(i=>i.AskId == ask.Id)
                        .Select(answer => new AnswerContent
                        {
                            Id = answer.Id,
                            Value = answer.Value
                        }).ToArray()
                }).ToArray();

            return new TestContent {Asks = askContents, Description = test.Info, Id = test.Id, Name = test.Name};
        }

        public static ResultContent GetResultContent(int tid, int[] AskId, int[] AnswerId, int uid)
        {
            using var context = ContextBuilder.Context;
            var test = context.Tests.First(i => i.Id == tid);
            
            // Проверка доступности
            if (test.Limit > 0)
            {
                if (uid == -1) return ResultContent.Error;
                if (context.GroupsList.Count(i => i.UserId == uid && i.GroupId == test.Limit) < 1) return ResultContent.Error;
            }
            if(test.SaveResults > 0 && uid < 0) return ResultContent.Error;

            var asks = new List<Ask>(context.Asks.Where(i => i.TestId == test.Id));
            var answers = new List<Answer>(context.Answers.Where(i => i.TestId == test.Id));
            var userAnswers = new Dictionary<int, int>();
            if (asks.Count != AskId.Length) return ResultContent.Error;

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

            return new ResultContent {Description = resultText, TestId = test.Id, Value = result, MaxValue = maxResult};
        }
        
        public static List<T> Get<T>(IEnumerable<T> list, Func<T, bool> f)
        {
            return new List<T>(list.Where(f));
        }
        
        public static void Add<T>(DbSet<T> list, T obj) where T : class
        {
            list.Add(obj);
        }
        
        public static void Update<T>(DbSet<T> list, T obj) where T : class
        {
            list.Update(obj);
        }

        public static List<Answer> GetAnswer(Func<Answer, bool> f)
        {
            return Get(ContextBuilder.Context.Answers, f);
        }
        
        public static List<Ask> GetAsk(Func<Ask, bool> f)
        {
            return Get(ContextBuilder.Context.Asks, f);
        }
        
        public static List<Group> GetGroup(Func<Group, bool> f)
        {
            return Get(ContextBuilder.Context.Groups, f);
        }
        
        public static List<GroupList> GetGroupList(Func<GroupList, bool> f)
        {
            return Get(ContextBuilder.Context.GroupsList, f);
        }
        
        public static List<Result> GetResult(Func<Result, bool> f)
        {
            return Get(ContextBuilder.Context.Results, f);
        }
        
        public static List<ResultDescription> GetResultDescription(Func<ResultDescription, bool> f)
        {
            return Get(ContextBuilder.Context.Descriptions, f);
        }
        
        public static List<Test> GetResultDescription(Func<Test, bool> f)
        {
            return Get(ContextBuilder.Context.Tests, f);
        }
        
        public static List<User> GetUser(Func<User, bool> f)
        {
            return Get(ContextBuilder.Context.Users, f);
        }
    }
}