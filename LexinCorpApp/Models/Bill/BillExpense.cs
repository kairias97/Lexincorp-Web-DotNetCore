using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillExpense
    {
        public int Id { get; set; }
        public int BillHeaderId { get; set; }
        public virtual BillHeader BillHeader { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string SpanishMonth { get; set; }
        public string EnglishMonth { get; set; }

    }
}
