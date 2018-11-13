using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IClientDepositRepository
    {
        IQueryable<ClientDeposit> ClientDeposits { get; }
        void Save(ClientDeposit clientDeposits);
        bool IsDepositValid(int packageId, decimal amount);
        bool IsDepositValid(int clientDepositId, int packageId, decimal amount);
    }
}
