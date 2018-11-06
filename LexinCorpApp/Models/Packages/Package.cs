using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Package
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El título del paquete es requerido")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La descripción es requerida")]
        public string Description { get; set; }
        [Required(ErrorMessage = "La fecha del paquete es requerida")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RealizationDate { get; set; }
        [Required(ErrorMessage = "El monto acordado de honorarios es requerido")]
        public decimal Amount { get; set; }
        public bool IsFinished { get; set; } = false;
        [Required(ErrorMessage = "No se ha seleccionado al cliente al cual se asociará el paquete")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public bool IsBilled { get; set; }
        [Required(ErrorMessage = "Se debe indicar un monto de gastos acordado en USD")]
        public decimal AgreedExpensesAmount { get; set; }
    }
}
