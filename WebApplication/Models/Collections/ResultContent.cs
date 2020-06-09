namespace WebApplication.Models.Collections
{
    public class ResultContent
    {
        public static ResultContent Error = new ResultContent{TestId = -1, Description = "Error"};
        
        public int TestId { get; set; }
        
        public int Value { get; set; }
        
        public string Description { get; set; }
        
        public int MaxValue { get; set; }
    }
}