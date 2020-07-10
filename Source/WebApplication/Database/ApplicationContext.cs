using System;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Database
{
    public sealed class ApplicationContext : DbContext, ICloneable
    {
        private readonly string _ip;
        private readonly string _user;
        private readonly string _pass;
        private readonly string _database;

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupList> GroupsList { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Ask> Asks { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<ResultDescription> Descriptions { get; set; }
        
        
        public ApplicationContext(string ip, string user, string pass, string database, bool check = false)
        {
            _ip = ip;
            _user = user;
            _pass = pass;
            _database = database;
            if(check) Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql($"server={_ip};UserId={_user};Password={_pass};database={_database};");
        }
        
        public object Clone()
        {
            return new ApplicationContext(_ip, _user, _pass, _database);
        }
    }
}