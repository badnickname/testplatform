namespace WebApplication.Database
{
    public static class ContextBuilder
    {
        private static ApplicationContext _context;
        private static string _ip, _user, _pass, _database;

        public static ApplicationContext Context
        {
            get => new ApplicationContext(_ip, _user, _pass, _database);
            private set => _context = value;
        }

        public static void Init(string ip, string user, string pass, string database)
        {
            _ip = ip;
            _user = user;
            _pass = pass;
            _database = database;
            Context = new ApplicationContext(ip, user, pass, database, true);
        }
        
    }
}