using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class VacationsRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se debe indicar una cantidad de días")]
        public decimal Quantity { get; set; }
        [Required(ErrorMessage = "Se debe indicar una razón para la solicitud")]
        public string Reason { get; set; }
        public bool? IsApproved { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Created { get; set; }
        [Required(ErrorMessage = "Se debe indicar una fecha de inicio para la solicitud")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }
        public int AttorneyId { get; set; }
        public virtual Attorney Attorney { get; set; }
        public bool IsApplied { get; set; } = false;
        public string RejectReason { get; set; }
        public virtual ICollection<VacationsRequestAnswer> VacationsRequestAnswers { get; set; }
    }
}
