using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFClientTypeRepository : IClientTypeRepository
    {
        private ApplicationDbContext context;

        public EFClientTypeRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<ClientType> ClientTypes { get => context.ClientTypes; }
    }
}
