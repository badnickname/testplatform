namespace WebApplication.Database
{
    public static class ContextBuilder
    {
        private static ApplicationContext _context;

        public static ApplicationContext Context
        {
            get => _context.Clone() as ApplicationContext;
            private set => _context = value;
        }

        public static void Init(string ip, string user, string pass, string database)
        {
            Context = new ApplicationContext(ip, user, pass, database, true);
        }
        
    }
}