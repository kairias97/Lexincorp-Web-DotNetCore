using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFClientRepository : IClientRepository
    {
        private ApplicationDbContext context;

        public EFClientRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<Client> Clients { get => context.Clients; }

        public void Save(Client client)
        {

            if (client.Id == 0)
            {
                context.Clients.Add(client);
            }
            context.SaveChanges();
        }

        public bool VerifyTributaryId(string tributaryId)
        {
            return !context.Clients.Any(c => c.TributaryId == tributaryId);
        }
    }
}
