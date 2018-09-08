using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IRetainerSubscriptionRepository
    {
        IQueryable<RetainerSubscription> Subscriptions { get; set; }
        void Save(RetainerSubscription subscription);
        void Delete(int subscriptionId);
    }
}
