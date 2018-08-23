using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ExpenseListViewModel
    {
        public string CurrentFilter { get; set; }
        public IEnumerable<Expense> Expenses { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
