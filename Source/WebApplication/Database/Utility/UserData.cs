namespace WebApplication.Database.Utility
{
    public struct UserData
    {
        public static readonly UserData Empty = new UserData{Id = -1};
        
        public string Name;
        public string Password;
        public int Id;
    }
}