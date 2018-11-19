using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IBillRepository
    {
        IQueryable<BillHeader> BillHeaders { get; }
        //void Save(BillHeader billHeader, List<Package> packagesToBill, List<BillableRetainer> retainersToBill);
        PreBill GeneratePreBill(PreBillRequest preBillingRequest, out bool IsEnglishBill);
        BillHeader GenerateBill(BillRequest billingRequest, int userId, out bool IsEnglishBill);
    }
}
