namespace WebApplication.Models.Collections
{
    public class SessionContent
    {
        public static readonly SessionContent Error = new SessionContent {Name = "Error"};
        public string Name { get; set; }
        public string SessionKey { get; set; }
    }
}