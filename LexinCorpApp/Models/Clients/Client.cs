using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El nombre del cliente es requerido")]
        public string Name { get; set; }
        [Required(ErrorMessage ="El nombre del contacto es requerido")]
        public string Contact { get; set; }
        [EmailAddress(ErrorMessage ="El formato de correo es inválido")]
        public string ContactEmail { get; set; }
        [Phone(ErrorMessage = "El formato de teléfono del contacto es inválido")]
        public string ContactPhone { get; set; }
        public string ContactJobName { get; set; }
        public string Address { get; set; }
        public string ReferredBy { get; set; }
        [Required(ErrorMessage ="La identificación tributaria del cliente es requerida")]
        public string TributaryId { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        [Phone]
        public string CellPhoneNumber { get; set; }
        [EmailAddress(ErrorMessage ="El formato de correo del cliente es inválido")]
        public string Email { get; set; }
        public int ClientTypeId { get; set; }
        public virtual ClientType ClientType { get; set; }
        //Facturación en
        public bool BillingInEnglish { get; set; }
        //Facturar en
        public int BillingModeId { get; set; }
        public virtual BillingMode BillingMode {get; set;}
        //Enviar documentos vía
        public int DocumentDeliveryMethodId { get; set; }
        public virtual DocumentDeliveryMethod DocumentDeliveryMethod { get; set; }
        //For client origin
        public bool IsInternational { get; set; }
        public bool PayTaxes { get; set; }
        public decimal? FixedCostPerHour { get; set; }
        public bool Active { get; set; } = true;
        public virtual ICollection<Package> Packages { get; set; }
    }
}
