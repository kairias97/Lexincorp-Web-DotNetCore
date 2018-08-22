using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFDocumentDeliveryMethodRepository : IDocumentDeliveryMethodRepository
    {
        private ApplicationDbContext context;

        public EFDocumentDeliveryMethodRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<DocumentDeliveryMethod> DocumentDeliveryMethods => context.DocumentDeliveryMethods;
    }
}
