using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFBillableRetainerRepository : IBillableRetainerRepository
    {
        private ApplicationDbContext context;
        public EFBillableRetainerRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<BillableRetainer> BillableRetainers { get => context.BillableRetainers; }
    }
}
