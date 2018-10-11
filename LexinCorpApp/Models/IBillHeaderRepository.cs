using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IBillHeaderRepository
    {
        IQueryable<BillHeader> BillHeaders { get; }
        void Save(BillHeader billHeader, List<Package> packagesToBill, List<BillableRetainer> retainersToBill);
    }
}
