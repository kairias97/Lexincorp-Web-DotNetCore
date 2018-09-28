using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillableRetainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BillingDescription { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int RetainerId { get; set; }
        public virtual Retainer Retainer { get; set; }
        public decimal AgreedFee { get; set; }
        public decimal AgreedHours { get; set; }
        public decimal AdditionalFeePerHour { get; set; }
        public decimal ConsumedHours { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
        public bool IsBilled { get; set; } = false;
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
