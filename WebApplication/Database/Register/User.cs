namespace WebApplication.Database.Register
{
    public class User
    {
        public string Name { get; set; }
        public string Pass { get; set; }
        public string Mail { get; set; }
        public string Code { get; set; }

        public User(string name, string pass, string mail, string code)
        {
            Name = name;
            Pass = pass;
            Mail = mail;
            Code = code;
        }
    }
}