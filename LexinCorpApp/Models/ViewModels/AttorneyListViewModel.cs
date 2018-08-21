using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class AttorneyListViewModel
    {
        public string CurrentFilter { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Attorney> Attorneys { get; set; }
    }
}
