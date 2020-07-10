namespace WebApplication.Database.Utility
{
    public static class StringExpansion
    {
        public static string MakeSafe(this string str)
        {
            return str.Replace("<", "&lt;").Replace(">", "&gt;");
        }
        
        public static string TrimName(this string name)
        {
            name = name.Trim();
            name = name.Replace("<", "");
            name = name.Replace(">", "");
            if (name.Length < 1) name = "0";
            return name;
        }
    }
}