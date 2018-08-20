using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IClientRepository
    {
        IQueryable<Client> Clients { get; }
        void Save(Client client);
        bool VerifyTributaryId(string tributaryId);
    }
}
