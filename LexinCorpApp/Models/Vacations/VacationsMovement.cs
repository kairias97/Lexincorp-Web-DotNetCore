using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class VacationsMovement
    {        
        public int VacationsMovementId { get; set; }
        [Required(ErrorMessage = "Se debe indicar una razón")]
        public string Reason { get; set; }
        [Required(ErrorMessage = "Se debe indicar una cantidad")]
        public decimal Quantity { get; set; }
        [Required(ErrorMessage = "Se debe indicar un tipo")]
        public VacationMovementEnum MovementType { get; set; }
        public DateTime Created { get; set; }
        [Required(ErrorMessage = "Se debe indicar un usuario")]
        public int AttorneyId { get; set; }
        public virtual Attorney Attorney { get; set; }
    }
}
