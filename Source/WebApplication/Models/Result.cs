using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Value { get; set; }
        public int TestId { get; set; }
        
        [Column(TypeName="Date")]
        public DateTime Time { get; set; }
    }
}