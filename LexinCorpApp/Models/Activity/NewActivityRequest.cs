using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class NewActivityRequest
    {
        [Required(ErrorMessage = "Es necesario seleccionar un cliente")]
        public int ClientId { get; set; }
        [Required(ErrorMessage = "Es requerido indicar una fecha")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ActivityDate { get; set; }
        [Required(ErrorMessage = "Es necesario indicar una descripcion")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Es requerido seleccionar un servicio")]
        public int ServiceId { get; set; }
        public ICollection<ExpenseDetail> Expenses { get; set; }
        [Required(ErrorMessage = "Es necesario indicar las horas trabajadas")]
        public decimal HoursWorked { get; set; }
        public int? PackageId { get; set; }
        public int? BillableRetainerId { get; set; }
        public decimal? ItemUnitPrice { get; set; }
        public int? ItemQuantity { get; set; }
        public decimal? ItemSubTotal { get; set; }
        public int? UserId { get; set; } 
        [Required(ErrorMessage = "Es requerido indicar una tasa horaria")]
        public decimal HourlyRate { get; set; }
        public decimal HourlySubtotal { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
    }
}
