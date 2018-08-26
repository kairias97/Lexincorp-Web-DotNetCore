using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class VacationsRequestListViewModel
    {
        public bool? CurrentFilter { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<VacationsRequest> VacationsRequests { get; set; }
        public string CurrentFilterText { get; set; }
    }
}
