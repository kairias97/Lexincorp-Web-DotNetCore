using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFBillRepository : IBillRepository
    {
        private ApplicationDbContext context;
        private readonly IConfiguration configuration;
        public EFBillRepository(ApplicationDbContext ctx, IConfiguration config)
        {
            this.context = ctx;
            configuration = config;
        }
        public IQueryable<BillHeader> BillHeaders { get => context.BillHeaders; }

        public BillHeader GenerateBill(BillRequest billRequest, int userId, out bool IsEnglishBill)
        {
            BillHeader billHeader = new BillHeader
            {
                BillDate = billRequest.BillDate,
                BillDiscountType = billRequest.BillDiscountType,
                BillMonth = billRequest.BillMonth,
                ClientId = billRequest.ClientId,
                BillYear = billRequest.BillYear,
                BillName = billRequest.BillName,
                CreatorId = userId,
                BillDetails = new List<BillDetail>()
            };
            List<Package> packagesToMark = new List<Package>();
            List<BillableRetainer> retainersToMark = new List<BillableRetainer>();
            var details = new List<BillDetail>();
            var client = context.Clients.Where(c => c.Id == billHeader.ClientId).FirstOrDefault();
            IsEnglishBill = client.BillingInEnglish;
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            if (billRequest.Hours ?? false)
            {
                List<BillDetail> hourlyDetails = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Hourly && a.ClientId == billHeader.ClientId
                && !a.IsBilled && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                    .Select(a => new BillDetail
                    {
                        ActivityId = a.Id,
                        BillDetailType = BillDetailTypeEnum.Hours,
                        FixedAmount = 0,
                        UnitRate = a.BillableRate,
                        Quantity = a.BillableQuantity,
                        Subtotal = a.Subtotal,
                        Description = a.Description,
                        TaxesAmount = a.TaxesAmount
                    }).ToList();
                details.AddRange(hourlyDetails);
            }
            if (billRequest.IncludeItems ?? false)
            {
                List<BillDetail> itemsDetails = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Item && a.ClientId == billHeader.ClientId
                 && !a.IsBilled && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                    .Select(a => new BillDetail
                    {
                        ActivityId = a.Id,
                        BillDetailType = BillDetailTypeEnum.Item,
                        FixedAmount = 0,
                        UnitRate = a.BillableRate,
                        Quantity = a.BillableQuantity,
                        Subtotal = a.Subtotal,
                        Description = a.Description,
                        TaxesAmount = a.TaxesAmount
                    }).ToList();
                details.AddRange(itemsDetails);
            }
            if (billRequest.PackageId != null)
            {
                var packages = context.Packages.Where(p => (billRequest.PackageId == -1 || p.Id == billRequest.PackageId) && p.IsFinished == true && p.IsBilled == false && p.ClientId == billHeader.ClientId).ToList();
                var result = new List<BillDetail>();
                foreach (var p in packages)
                {
                    var detail = new BillDetail();
                    packagesToMark.Add(p);
                    detail.BillDetailType = BillDetailTypeEnum.PackageHeader;
                    detail.Description = p.Description;
                    detail.FixedAmount = p.Amount;
                    detail.Subtotal = p.Amount;
                    detail.Quantity = 1;
                    detail.TaxesAmount = client.PayTaxes ? p.Amount * ivaValue : 0;
                    detail.UnitRate = 0;
                    result.Add(detail);
                    var packagesDetails = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Package && a.ClientId == billHeader.ClientId
                    && !a.IsBilled && a.PackageId == p.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.PackageDetail,
                            FixedAmount = (decimal)0.00,
                            UnitRate = a.BillableRate,
                            Quantity = a.BillableQuantity,
                            Subtotal = a.Subtotal,
                            Description = a.Description,
                            TaxesAmount = a.TaxesAmount
                        }).ToList();
                    result.AddRange(packagesDetails);
                }
                details.AddRange(result);
            }
            if (billRequest.BillableRetainerId != null)
            {
                var retainers = context.BillableRetainers.Where(r => (billRequest.BillableRetainerId == -1 || r.Id == billRequest.BillableRetainerId) 
                && r.IsBilled == false && r.ClientId == billHeader.ClientId
                && r.Month == billRequest.BillMonth && r.Year == billRequest.BillYear).ToList();
                var result = new List<BillDetail>();
                foreach (var r in retainers)
                {
                    var detail = new BillDetail();
                    retainersToMark.Add(r);
                    detail.BillDetailType = BillDetailTypeEnum.RetainerHeader;
                    detail.Description = r.BillingDescription;
                    detail.FixedAmount = r.AgreedFee;
                    detail.Subtotal = r.AgreedFee;
                    detail.Quantity = 1;
                    detail.TaxesAmount = client.PayTaxes ? r.AgreedFee * ivaValue : 0;
                    detail.UnitRate = 0;
                    result.Add(detail);
                    var retainerDetails = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Retainer && a.ClientId == billHeader.ClientId
                    && !a.IsBilled  && a.BillableRetainerId == r.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.RetainerDetail,
                            FixedAmount = (decimal)0.00,
                            UnitRate = a.BillableRate,
                            //Quantity = a.BillableRate == (decimal)0.00 ? a.HoursWorked : a.BillableQuantity,
                            Quantity = a.BillableQuantity,
                            Subtotal = a.Subtotal,
                            Description = a.Description,
                            TaxesAmount = a.TaxesAmount
                        }).ToList();
                    result.AddRange(retainerDetails);
                }
                details.AddRange(result);
            }
            
            billHeader.BillDetails = details;
            billHeader.BillSubtotal = billHeader.BillDetails.Sum(d => d.Subtotal);
            billHeader.Taxes = billHeader.BillDetails.Sum(d => d.TaxesAmount);
            var discount = billRequest.BillDiscountType == null ? 0 : billRequest.BillDiscountType == BillDiscountEnum.Net ? billRequest.BillDiscount ?? 0 : billRequest.BillDiscountType == BillDiscountEnum.Percentage ? ((billRequest.BillDiscount ?? (decimal)0.00) / 100) * billHeader.BillSubtotal : 0;
            billHeader.BillDiscount = discount;
            billHeader.Total = billHeader.BillSubtotal - discount + billHeader.Taxes;

            //Persisting the changes
            var activities = billHeader.BillDetails.Where(d => d.BillDetailType != BillDetailTypeEnum.PackageHeader && d.BillDetailType != BillDetailTypeEnum.RetainerHeader).Select(d => new Activity
            {
                Id = Convert.ToInt32(d.ActivityId),
                IsBilled = true
            }).ToList();
            context.AttachRange(activities);
            context.AttachRange(packagesToMark);
            context.AttachRange(retainersToMark);
            foreach (var a in activities)
            {
                context.Entry<Activity>(a).Property(x => x.IsBilled).IsModified = true;
            }
            foreach (var p in packagesToMark)
            {
                p.IsBilled = true;
                context.Entry<Package>(p).Property(x => x.IsBilled).IsModified = true;
            }
            foreach (var r in retainersToMark)
            {
                r.IsBilled = true;
                context.Entry<BillableRetainer>(r).Property(x => x.IsBilled).IsModified = true;
            }
            context.Add(billHeader);
            context.SaveChanges();
            //Save(billHeader, pack, ret);
            return billHeader;
        }

        public BillHeader GeneratePreBill(BillRequest preBillingRequest, out bool IsEnglishBill)
        {
            BillHeader billHeader = new BillHeader
            {
                BillDate = preBillingRequest.BillDate,
                BillDiscountType = preBillingRequest.BillDiscountType,
                BillMonth = preBillingRequest.BillMonth,
                ClientId = preBillingRequest.ClientId,
                BillYear = preBillingRequest.BillYear,
                BillName = preBillingRequest.BillName,
                BillDetails = new List<BillDetail>()
            };
            var details = new List<BillDetail>();
            var client = context.Clients.Where(c => c.Id == billHeader.ClientId).FirstOrDefault();
            IsEnglishBill = client.BillingInEnglish;
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            if (preBillingRequest.Hours ?? false)
            {
                List<BillDetail> result = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Hourly && a.ClientId == billHeader.ClientId
                    && !a.IsBilled && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                    .Select(a => new BillDetail
                    {
                        ActivityId = a.Id,
                        BillDetailType = BillDetailTypeEnum.Hours,
                        FixedAmount = 0,
                        UnitRate = a.BillableRate,
                        Quantity = a.BillableQuantity,
                        Subtotal = a.Subtotal,
                        Description = a.Description,
                        TaxesAmount = a.TaxesAmount
                    }).ToList();
                details.AddRange(result);
            }
            if (preBillingRequest.IncludeItems ?? false)
            {
                List<BillDetail> result = context.Activities
                    .OrderBy(a => a.RealizationDate)
                    .Where(a => a.ActivityType == ActivityTypeEnum.Item && a.ClientId == billHeader.ClientId && !a.IsBilled && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                    .Select(a => new BillDetail
                    {
                        ActivityId = a.Id,
                        BillDetailType = BillDetailTypeEnum.Item,
                        FixedAmount = 0,
                        UnitRate = a.BillableRate,
                        Quantity = a.BillableQuantity,
                        Subtotal = a.Subtotal,
                        Description = a.Description,
                        TaxesAmount = a.TaxesAmount
                    }).ToList();
                details.AddRange(result);
            }
            if (preBillingRequest.PackageId != null)
            {
                var packages = context.Packages.Where(p => (preBillingRequest.PackageId == -1 || p.Id == preBillingRequest.PackageId) && p.IsFinished == true && p.IsBilled == false && p.ClientId == billHeader.ClientId).ToList();
                var result = new List<BillDetail>();
                foreach (var p in packages)
                {
                    var detail = new BillDetail();
                    detail.BillDetailType = BillDetailTypeEnum.PackageHeader;
                    detail.Description = p.Description;
                    detail.FixedAmount = p.Amount;
                    detail.Subtotal = p.Amount;
                    detail.Quantity = 1;
                    detail.TaxesAmount = client.PayTaxes ? p.Amount * ivaValue : 0;
                    detail.UnitRate = 0;
                    result.Add(detail);
                    var activities = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Package && a.ClientId == billHeader.ClientId && a.PackageId == p.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.PackageDetail,
                            FixedAmount = (decimal)0.00,
                            UnitRate = a.BillableRate,
                            Quantity = a.BillableQuantity,
                            Subtotal = a.Subtotal,
                            Description = a.Description,
                            TaxesAmount = a.TaxesAmount
                        }).ToList();
                    result.AddRange(activities);
                }
                details.AddRange(result);
            }
            if (preBillingRequest.BillableRetainerId != null)
            {
                var retainers = context.BillableRetainers.Where(r => (preBillingRequest.BillableRetainerId == -1 || r.Id == preBillingRequest.BillableRetainerId) 
                && r.IsBilled == false && r.ClientId == billHeader.ClientId
                && r.Month == preBillingRequest.BillMonth && r.Year == preBillingRequest.BillYear).ToList();
                var result = new List<BillDetail>();
                foreach (var r in retainers)
                {
                    var detail = new BillDetail();
                    detail.BillDetailType = BillDetailTypeEnum.RetainerHeader;
                    detail.Description = r.BillingDescription;
                    detail.FixedAmount = r.AgreedFee;
                    detail.Subtotal = r.AgreedFee;
                    detail.Quantity = 1;
                    detail.TaxesAmount = client.PayTaxes ? r.AgreedFee * ivaValue : 0;
                    detail.UnitRate = 0;
                    result.Add(detail);
                    var activities = context.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Retainer && a.ClientId == billHeader.ClientId && !a.IsBilled && a.BillableRetainerId == r.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.RetainerDetail,
                            FixedAmount = (decimal)0.00,
                            UnitRate = a.BillableRate,
                            Quantity = a.BillableQuantity,
                            Subtotal = a.Subtotal,
                            Description = a.Description,
                            TaxesAmount = a.TaxesAmount
                        }).ToList();
                    result.AddRange(activities);
                }
                details.AddRange(result);
            }
            
            billHeader.BillDetails = details;
            billHeader.BillSubtotal = billHeader.BillDetails.Sum(d => d.Subtotal);
            billHeader.Taxes = billHeader.BillDetails.Sum(d => d.TaxesAmount);
            var discount = preBillingRequest.BillDiscountType == null ? 0 : preBillingRequest.BillDiscountType == BillDiscountEnum.Net ? preBillingRequest.BillDiscount ?? 0 : preBillingRequest.BillDiscountType == BillDiscountEnum.Percentage ? ((preBillingRequest.BillDiscount ?? (decimal)0.00) / 100)* billHeader.BillSubtotal : 0;
            billHeader.BillDiscount = discount;
            billHeader.Total = billHeader.BillSubtotal - discount + billHeader.Taxes;

            return billHeader;
        }

        //public void Save(BillHeader billHeader, List<Package> packagesToBill, List<BillableRetainer>retainersToBill)
        //{
        //    var activities = billHeader.BillDetails.Where(d => d.BillDetailType != BillDetailTypeEnum.PackageHeader && d.BillDetailType != BillDetailTypeEnum.RetainerHeader).Select(d => new Activity
        //    {
        //        Id = Convert.ToInt32(d.ActivityId),
        //        IsBilled = true
        //    }).ToList();
        //    context.AttachRange(activities);
        //    context.AttachRange(packagesToBill);
        //    context.AttachRange(retainersToBill);
        //    foreach(var a in activities)
        //    {
        //        context.Entry<Activity>(a).Property(x => x.IsBilled).IsModified = true;
        //    }
        //    foreach(var p in packagesToBill)
        //    {
        //        p.IsBilled = true;
        //        context.Entry<Package>(p).Property(x => x.IsBilled).IsModified = true;
        //    }
        //    foreach(var r in retainersToBill)
        //    {
        //        r.IsBilled = true;
        //        context.Entry<BillableRetainer>(r).Property(x => x.IsBilled).IsModified = true;
        //    }
        //    context.Add(billHeader);
        //    context.SaveChanges();
        //}
        //}
    }
}
