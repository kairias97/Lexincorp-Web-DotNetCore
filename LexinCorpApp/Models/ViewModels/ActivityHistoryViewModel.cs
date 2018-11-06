using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ActivityHistoryViewModel
    {
        public string CurrentFilter { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public string CurrentStartDate { get; set; }
        public string CurrentEndDate { get; set; }
    }
}
