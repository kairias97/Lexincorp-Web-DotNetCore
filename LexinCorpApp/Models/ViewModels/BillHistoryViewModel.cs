using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class BillHistoryViewModel
    {
        public string CurrentFilter { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<BillHeader> BillHeaders { get; set; }
    }
}
