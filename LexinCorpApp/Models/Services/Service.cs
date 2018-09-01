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
        [Required(ErrorMessage = "El nombre del servicio es requerido")]
        public string Name { get; set;}
        [Required(ErrorMessage = "La descripción en inglés para plantilla es requerido")]
        public string EnglishDescription { get; set; }
        [Required(ErrorMessage = "La descripción en español para plantilla es requerido")]
        public string SpanishDescription { get; set; }
        [Required(ErrorMessage = "La categoría es requerida")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public bool Active { get; set; } = true;
    }
}
