namespace WebApplication.Model
{
    public static class StringExpansion
    {
        public static string MakeSafe(this string str)
        {
            return str.Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}