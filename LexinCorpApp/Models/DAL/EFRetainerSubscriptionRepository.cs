﻿using System;
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

        public IQueryable<RetainerSubscription> Subscriptions { get => context.RetainerSubscriptions; }

        public void Delete(int subscriptionId)
        {
            var subscription = new RetainerSubscription { Id = subscriptionId };
            context.RetainerSubscriptions.Attach(subscription);
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
                context.Entry(subscription).Property(rs => rs.CreatorId).IsModified = false;
                context.Entry(subscription).Property(rs => rs.ClientId).IsModified = false;
                context.Entry(subscription).Property(rs => rs.RetainerId).IsModified = false;
            }
            context.SaveChanges();
        }
        
    }
}
