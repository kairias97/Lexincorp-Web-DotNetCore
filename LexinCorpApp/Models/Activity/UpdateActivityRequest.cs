using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class UpdateActivityRequest
    {
        public int Id { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal? ItemUnitPrice { get; set; }
        public int? ItemQuantity { get; set; }
        public decimal? ItemSubTotal { get; set; }
        public ICollection<ExpenseDetail> Expenses { get; set; }
        public string Description { get; set; }
        public decimal HourlyRate { get; set; }
    }
}
