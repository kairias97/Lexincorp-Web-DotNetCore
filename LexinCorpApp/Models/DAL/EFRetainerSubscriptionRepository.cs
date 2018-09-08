using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFRetainerSubscriptionRepository : IRetainerSubscriptionRepository
    {
        private ApplicationDbContext context;

        public EFRetainerSubscriptionRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<RetainerSubscription> Subscriptions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Delete(int subscriptionId)
        {
            var subscription = new RetainerSubscription { Id = subscriptionId };
            context.RetainerSubscriptions.Remove(subscription);
            context.SaveChanges();
        }

        public void Save(RetainerSubscription subscription)
        {
            if (subscription.Id == 0)
            {
                context.RetainerSubscriptions.Add(subscription);
            } else
            {
                context.Entry(subscription).State = Microsoft.EntityFrameworkCore.EntityState.Modified; 
            }
            context.SaveChanges();
        }
    }
}
