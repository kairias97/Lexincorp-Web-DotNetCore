using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal HoursWorked { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public DateTime RealizationDate { get; set; }
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxesAmount { get; set; }
        public bool PayTaxes { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsBilled { get; set; }
        public decimal BillableRate { get; set; }
        public decimal BillableQuantity { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public int? BillableRetainerId { get; set; }
        public virtual BillableRetainer BillableRetainer { get; set; }
        public int? PackageId { get; set; }
        public virtual Package Package { get; set; }
        public int? ItemId { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<ActivityExpense> ActivityExpenses { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
        public bool IsBillable { get; set; }
    }
}
