using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class RetainerSubscription
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El cliente es requerido")]
        public int ClientId { get; set; } 
        public virtual Client Client { get; set; }
        [Required(ErrorMessage = "El tipo de retainer es requerido")]
        public int RetainerId { get; set; }
        public virtual Retainer Retainer { get; set; }
        [Required(ErrorMessage = "El monto acordado es requerido")]
        public decimal AgreedFee { get; set; }
        [Required(ErrorMessage = "La cantidad de horas acordadas es requerida")]
        public decimal AgreedHours { get; set; }
        [Required(ErrorMessage = "La tarifa adicional por hora fuera de las acordadas es requerida")]
        public decimal AdditionalFeePerHour { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

    }
}
