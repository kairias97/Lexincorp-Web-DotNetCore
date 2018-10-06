using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models { 
    public class NewPreBillViewModel
    {
        public IEnumerable<Item> Items { get; set; }
        public bool IsEnglish { get; set; }
        public bool IsClientSelected { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
    }
}
