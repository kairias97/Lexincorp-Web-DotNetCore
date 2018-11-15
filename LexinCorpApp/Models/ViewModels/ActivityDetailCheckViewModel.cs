using LexincorpApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ActivityDetailCheckViewModel
    {
        public string CurrentFilter { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public string CurrentStartDate { get; set; }
        public string CurrentEndDate { get; set; }
        public IEnumerable<Expense> Expenses { get; set; }
        public IEnumerable<Package> Packages { get; set; }
        public IEnumerable<BillableRetainer> BillableRetainers { get; set; }
        public int CurrentId { get; set; }
        public bool ByRange { get; set; }
        public bool checkHours { get; set; }
        public bool checkItem { get; set; }
        public bool checkAll { get; set; }


    }
}
