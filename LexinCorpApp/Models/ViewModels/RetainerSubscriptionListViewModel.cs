using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class RetainerSubscriptionListViewModel
    {
        public string CurrentFilter { get; set; }
        public IEnumerable<RetainerSubscription> Subscriptions { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
