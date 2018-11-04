using LexincorpApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ClientDetailsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<RetainerSubscription> RetainerSubscriptions { get; set; }
        public IEnumerable<Package> Packages { get; set; }
        public IEnumerable<ClientType> ClientTypes { get; set; }
        public IEnumerable<BillingMode> BillingModes { get; set; }
        public IEnumerable<DocumentDeliveryMethod> DocumentDeliveryMethods { get; set; }
    }
}
