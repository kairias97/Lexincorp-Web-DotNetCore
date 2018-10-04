using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ExpenseReportViewModel
    {
        public IEnumerable<Attorney> Attorneys { get; set; }
        public NewExpenseReport NewExpenseReport { get; set; }
    }
}
