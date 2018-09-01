using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFPackageRepository : IPackageRepository
    {
        private ApplicationDbContext context;

        public EFPackageRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Package> Packages => context.Packages;

        public void Save(Package package)
        {
            if (package.Id == 0)
            {
                context.Packages.Add(package);
            } else
            {
                context.Entry(package).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
