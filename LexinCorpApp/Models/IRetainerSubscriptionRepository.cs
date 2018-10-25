using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IRetainerSubscriptionRepository
    {
        IQueryable<RetainerSubscription> Subscriptions { get;}
        void Save(RetainerSubscription subscription);
        void Delete(int subscriptionId);
        void Apply(out bool success, out string message);
    }
}
