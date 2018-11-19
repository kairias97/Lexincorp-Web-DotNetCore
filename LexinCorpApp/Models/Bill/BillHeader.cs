using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillHeader
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public DateTime BillDate { get; set; }
        public decimal BillSubtotal { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Total { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<BillSummary> BillSummaries { get; set; }
        public virtual ICollection<BillExpense> BillExpenses { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
    }
}
