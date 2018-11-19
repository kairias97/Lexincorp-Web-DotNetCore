using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillSummary
    {
        public int Id { get; set; }
        public int BillHeaderId { get; set; }
        public virtual BillHeader BillHeader { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Time { get; set; }
        public int Quantity { get; set; }
        public BillDetailTypeEnum TypeId { get; set; }
        public decimal Total { get; set; }
    }
}
