using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ServiceListViewModel
    {
        public string CurrentFilter { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
