using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFRetainerRepository : IRetainerRepository
    {
        private ApplicationDbContext context;
        public EFRetainerRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Retainer> Retainers => context.Retainers;

        public void Save(Retainer retainer)
        {
            if (retainer.Id == 0)
            {
                context.Retainers.Add(retainer);
            } else
            {
                context.Entry(retainer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
