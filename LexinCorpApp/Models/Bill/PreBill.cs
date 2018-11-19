using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class PreBill
    {
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Total { get; set; }
        public List<PreBillDetail> Details { get; set; }
    }
}
