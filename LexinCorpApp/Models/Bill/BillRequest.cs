using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillRequest
    {
        public bool? Hours { get; set; }
        public int? PackageId { get; set; }
        public int? BillableRetainerId { get; set; }
        public bool? IncludeItems { get; set; }
        public int ClientId { get; set; }
        public DateTime BillDate { get; set; }
        public BillDiscountEnum? BillDiscountType { get; set; }
        public decimal? BillDiscount { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public string BillName { get; set; }
    }
}
