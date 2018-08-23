using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ItemListViewModel
    {
        public string CurrentFilter { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
