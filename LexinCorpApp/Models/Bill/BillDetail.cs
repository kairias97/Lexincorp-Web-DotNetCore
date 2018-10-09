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
        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
        public BillDetailTypeEnum BillDetailType { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? UnitRate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string Description { get; set; }
    }
}
