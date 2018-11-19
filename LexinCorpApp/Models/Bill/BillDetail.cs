using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillDetail
    {
        public int Id { get; set; }
        public int BillHeaderId { get; set; }
        public virtual BillHeader BillHeader { get; set; }
        public int? ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Time { get; set; }
        public BillDetailTypeEnum BillDetailType { get; set; }
        public string Attorney { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Subtotal { get; set; }
        public int? PackageId { get; set; }
        public virtual Package Package {get; set;}
        public int? BillableRetainerId { get; set; }
        public virtual BillableRetainer BillableRetainer { get; set; }
    }
}
