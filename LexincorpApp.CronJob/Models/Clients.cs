using System;
using System.Collections.Generic;

namespace LexincorpApp.CronJob.Models
{
    public partial class Clients
    {
        public Clients()
        {
            BillableRetainers = new HashSet<BillableRetainers>();
            RetainerSubscriptions = new HashSet<RetainerSubscriptions>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactJobName { get; set; }
        public string Address { get; set; }
        public string ReferredBy { get; set; }
        public string TributaryId { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string CellPhoneNumber { get; set; }
        public string Email { get; set; }
        public int ClientTypeId { get; set; }
        public bool BillingInEnglish { get; set; }
        public int BillingModeId { get; set; }
        public int DocumentDeliveryMethodId { get; set; }
        public bool IsInternational { get; set; }
        public bool PayTaxes { get; set; }
        public decimal? FixedCostPerHour { get; set; }
        public bool? Active { get; set; }

        public ICollection<BillableRetainers> BillableRetainers { get; set; }
        public ICollection<RetainerSubscriptions> RetainerSubscriptions { get; set; }
    }
}
