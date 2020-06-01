namespace WebApplication.Model.Data
{
    public class Answer
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int AskId { get; set; }
        public string Value { get; set; }
        public int Impact { get; set; }
    }
}