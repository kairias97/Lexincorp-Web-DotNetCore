using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ClientsListViewModel
    {
        public string CurrentFilter { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public IEnumerable<Client> Clients { get; set; }
    }
}
