using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ActivityExpense
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }
        public int Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime RealizationDate { get; set; }
        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
