using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFClientDepositRepository : IClientDepositRepository
    {
        private ApplicationDbContext context;

        public EFClientDepositRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<ClientDeposit> ClientDeposits => context.ClientDeposits;

        public bool IsDepositValid(int packageId, decimal amount)
        {
            decimal packagePayableAmount = context.Packages.Where(p => p.Id == packageId).Select(p => p.Amount + p.AgreedExpensesAmount).FirstOrDefault();
            decimal packagePaidAmount = context.ClientDeposits.Where(cd => cd.PackageId == packageId).Sum(cd => cd.Amount);

            return (packagePaidAmount + amount) <= packagePayableAmount && amount > 0;
        }

        public bool IsDepositValid(int clientDepositId, int packageId, decimal amount)
        {
            decimal packagePayableAmount = context.Packages.Where(p => p.Id == packageId).Select(p => p.Amount + p.AgreedExpensesAmount).FirstOrDefault();
            decimal packagePaidAmount = context.ClientDeposits.Where(cd => cd.PackageId == packageId && cd.Id != clientDepositId).Sum(cd => cd.Amount);

            return (packagePaidAmount + amount) <= packagePayableAmount && amount > 0;
        }

        public void Save(ClientDeposit clientDeposits)
        {
            if (clientDeposits.Id == 0)
            {
                context.ClientDeposits.Add(clientDeposits);
            } else
            {
                clientDeposits.Client = null;
                clientDeposits.Package = null;
                context.ClientDeposits.Attach(clientDeposits);
                context.Entry(clientDeposits).Property(cd => cd.Amount).IsModified = true;
                context.Entry(clientDeposits).Property(cd => cd.ReceivedDate).IsModified = true;
            }
            context.SaveChanges();
        }
    }
}
