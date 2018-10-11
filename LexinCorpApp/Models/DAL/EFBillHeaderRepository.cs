using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFBillHeaderRepository : IBillHeaderRepository
    {
        private ApplicationDbContext context;
        public EFBillHeaderRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<BillHeader> BillHeaders { get => context.BillHeaders; }

        public void Save(BillHeader billHeader, List<Package> packagesToBill, List<BillableRetainer>retainersToBill)
        {
            var activities = billHeader.BillDetails.Where(d => d.BillDetailType != BillDetailTypeEnum.PackageHeader && d.BillDetailType != BillDetailTypeEnum.RetainerHeader).Select(d => new Activity
            {
                Id = Convert.ToInt32(d.ActivityId),
                IsBilled = true
            }).ToList();
            context.AttachRange(activities);
            context.AttachRange(packagesToBill);
            context.AttachRange(retainersToBill);
            foreach(var a in activities)
            {
                context.Entry<Activity>(a).Property(x => x.IsBilled).IsModified = true;
            }
            foreach(var p in packagesToBill)
            {
                p.IsBilled = true;
                context.Entry<Package>(p).Property(x => x.IsBilled).IsModified = true;
            }
            foreach(var r in retainersToBill)
            {
                r.IsBilled = true;
                context.Entry<BillableRetainer>(r).Property(x => x.IsBilled).IsModified = true;
            }
            context.Add(billHeader);
            context.SaveChanges();
        }
    }
}
