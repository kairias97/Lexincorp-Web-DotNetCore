using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        public string Name { get; set; }
        public bool Active { get; set; } = true;
        public virtual ICollection<Service> Services { get; set; }
    }
}
