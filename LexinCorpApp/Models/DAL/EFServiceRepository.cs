using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFServiceRepository : IServiceRepository
    {
        private ApplicationDbContext context;
        public EFServiceRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Service> Services => context.Services;

        public void Save(Service service)
        {
            
            if (service.Id == 0)
            {
                context.Services.Add(service);
            }
            else
            {
                context.Entry(service).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
