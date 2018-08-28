using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Retainer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool? Active { get; set; }

    }
}
