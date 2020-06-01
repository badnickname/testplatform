namespace WebApplication.Modules.Register
{
    public class User
    {
        public string name, pass, mail, code;

        public User(string name, string pass, string mail, string code)
        {
            this.name = name;
            this.pass = pass;
            this.mail = mail;
            this.code = code;
        }
    }
}