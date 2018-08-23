using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFItemRepository : IItemRepository
    {
        private ApplicationDbContext context;

        public EFItemRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Item> Items => context.Items;

        public void Save(Item item)
        {
            if (item.Id == 0)
            {
                context.Items.Add(item);
            } else
            {
                context.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
