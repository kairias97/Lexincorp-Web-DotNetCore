using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class NewClientDepositViewModel
    {
        public bool IsClientSelected {get; set;}
        public string ClientName { get; set; }
        public ClientDeposit ClientDeposit { get; set; }
        public List<Package> Packages { get; set; }
    }
}
