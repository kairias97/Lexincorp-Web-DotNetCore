using LexincorpApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ClientDepositsViewModel
    {
        public string CurrentFilter { get; set; }
        public List<ClientDeposit> ClientDeposits { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
