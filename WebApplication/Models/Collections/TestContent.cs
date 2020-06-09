namespace WebApplication.Models.Collections
{
    public class TestContent
    {
        public static readonly TestContent Error = new TestContent{Id = -1, Name = "Error"};
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public AskContent[] Asks { get; set; }
    }
}