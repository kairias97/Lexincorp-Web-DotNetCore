using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del gasto es requerido")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La descripción en español es requerida")]
        public string SpanishDescription { get; set; }
        [Required(ErrorMessage = "La descripción en inglés es requerida")]
        public string EnglishDescription { get; set; }
        [Required(ErrorMessage = "El monto asociado al gasto es requerido")]
        public decimal Amount { get; set; }
        [NotMapped]
        public string DisplayName { get { return $"{EnglishDescription} / {SpanishDescription}"; } }
        public bool Active { get; set; } = true;
        public bool IsReadOnly { get; set; }
    }
}
