using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ClientDeposit
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El monto es requerido")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "La fecha de recepción es requerida")]
        public DateTime ReceivedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage ="El cliente es requerido")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        [Required(ErrorMessage = "El paquete es requerido")]
        public int PackageId { get; set; }
        public virtual Package Package {get; set;}
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }



    }
}
