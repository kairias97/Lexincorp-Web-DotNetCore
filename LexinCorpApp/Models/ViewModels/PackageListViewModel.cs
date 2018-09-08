using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class PackageListViewModel
    {
        public string CurrentFilter { get; set; }
        public IEnumerable<Package> Packages { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
