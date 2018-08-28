using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set;}
        [Required]
        public string EnglishDescription { get; set; }
        [Required]
        public string SpanishDescription { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public bool? Active { get; set; }
    }
}
