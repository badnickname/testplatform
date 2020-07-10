namespace WebApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }
        public string Mail { get; set; }
        public byte[] Photo { get; set; }
    }
}