using Microsoft.Extensions.Configuration;

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

        public static void Init(IConfiguration configuration)
        {
            Context = new ApplicationContext(configuration["Settings:IP"],
                configuration["Settings:User"],
                configuration["Settings:Pass"], 
                configuration["Settings:Database"], true);
        }
        
    }
}