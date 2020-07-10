using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public byte[] Photo { get; set; }
        
        public int OwnerId { get; set; }
        
        public int Limit { get; set; }
        public int Show { get; set; }
        
        public int SaveResults { get; set; }
        public int Tries { get; set; }
        
        [Column(TypeName="Date")]
        public DateTime Time { get; set; }
    }
}