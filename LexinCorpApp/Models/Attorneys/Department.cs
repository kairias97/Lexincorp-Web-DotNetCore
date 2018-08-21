using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Attorney> Attorneys { get; set; }
    }
}
