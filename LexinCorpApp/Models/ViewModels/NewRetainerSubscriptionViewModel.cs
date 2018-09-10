using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class NewRetainerSubscriptionViewModel
    {
        public IEnumerable<Retainer> Retainers { get; set; }
        public RetainerSubscription RetainerSubscription { get; set; }
        public string ClientName { get; set; }
        public bool IsEnglish { get; set; }
        public bool IsClientSelected { get; set; }
    }
}
