using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexinCorpApp.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public string ContactJobName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ReferredBy { get; set; }
        [Required]
        public string TributaryId { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string CellPhoneNumber { get; set; }
        public string Email { get; set; }
        [Required]
        public int ClientTypeId { get; set; }
        public virtual ClientType ClientType { get; set; }
        [Required]
        //Facturación en
        public bool BillingInEnglish { get; set; }
        [Required]
        //Facturar en
        public int BillingModeId { get; set; }
        public virtual BillingMode BillingMode {get; set;}
        [Required]
        //Enviar documentos vía
        public int DocumentDeliveryMethodId { get; set; }
        public virtual DocumentDeliveryMethod DocumentDeliveryMethod { get; set; }
        //For client origin
        [Required]
        public bool IsInternational { get; set; }
        [Required]
        public bool PayTaxes { get; set; }
        [Required]
        public decimal? FixedCostPerHour { get; set; }
    }
}
