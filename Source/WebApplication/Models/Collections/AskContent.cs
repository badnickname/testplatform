﻿namespace WebApplication.Models.Collections
{
    public class AskContent
    {
        public int Id { get; set; }
        
        public string Value { get; set; }
        
        public AnswerContent[] Answers { get; set; }
    }
}