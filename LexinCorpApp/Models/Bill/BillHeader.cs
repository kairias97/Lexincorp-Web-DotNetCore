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
        public BillDiscountEnum? BillDiscountType { get; set; }
        public decimal? BillDiscount { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public string BillName { get; set; }
        public decimal BillSubtotal { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
    }
}
