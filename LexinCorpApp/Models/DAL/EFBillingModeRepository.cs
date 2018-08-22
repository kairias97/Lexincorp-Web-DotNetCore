using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFBillingModeRepository : IBillingModeRepository
    {
        private ApplicationDbContext context;

        public EFBillingModeRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<BillingMode> BillingModes => context.BillingModes;
    }
}
