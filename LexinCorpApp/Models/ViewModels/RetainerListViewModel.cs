using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class RetainerListViewModel
    {
        public string CurrentFilter { get; set; }
        public IEnumerable<Retainer> Retainers { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
