using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private string GetHourlySummaryDescription(bool isEnglish)
        {
            string description = isEnglish ? "Legal advice - Hourly rate according to detail" : "Asesoría Legal - Tasa horaria según detalle";
            return description;
        }
        public BillHeader GenerateBill(BillRequest billRequest, int userId, out bool IsEnglishBill)
        {
            BillHeader billHeader = new BillHeader {
                CreatorId = userId,
                BillDate = billRequest.BillDate,
                ClientId = billRequest.ClientId,
                BillDetails = new List<BillDetail>(),
                BillSummaries = new List<BillSummary>(),
                BillExpenses = new List<BillExpense>(),
                TotalPayments = 0,
                BillSubtotal = 0, 
                Taxes = 0,
                Total = 0,
                TotalExpenses = 0
                
            };


            List<Package> packagesToMark = new List<Package>();
            List<BillableRetainer> retainersToMark = new List<BillableRetainer>();
            var details = new List<BillDetail>();
            var summary = new List<BillSummary>();
            var expenses = new List<BillExpense>();
            decimal totalPayments = 0;
            decimal totalExpenses = 0;
            var client = context.Clients.Where(c => c.Id == billHeader.ClientId).FirstOrDefault();
            
            IsEnglishBill = client.BillingInEnglish;
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            //Processing hourly activities and item activities
            var hourlyOrItemDetails = context.Activities
                .Where(a => a.ClientId == billHeader.ClientId
                && a.IsBillable && !a.IsBilled
                && (a.ActivityType == ActivityTypeEnum.Hourly || a.ActivityType == ActivityTypeEnum.Item))
                .Select(a => new BillDetail
                {
                    BillDetailType = a.ActivityType == ActivityTypeEnum.Hourly ? BillDetailTypeEnum.Hours : BillDetailTypeEnum.Item,
                    ActivityId = a.Id,
                    Description = a.Description,
                    Date = a.RealizationDate,
                    Time = a.HoursWorked,
                    Quantity = a.BillableQuantity ,
                    UnitCost = a.BillableRate,
                    Subtotal = a.Subtotal,
                    Attorney = a.Creator.Attorney.Alias
                }).ToList();
            string hourlyDescription = GetHourlySummaryDescription(IsEnglishBill);
            
            var hourlyOrItemSummary = hourlyOrItemDetails
                .Select(d => new BillSummary {
                    Date = d.Date,
                    TypeId = d.BillDetailType,
                    Quantity = Convert.ToInt32(d.Quantity),
                    Time = d.Time,
                    Total = d.Subtotal,
                    Description = d.BillDetailType == BillDetailTypeEnum.Hours ? hourlyDescription : d.Description
                }).ToList();
            if (hourlyOrItemDetails.Count > 0)
            {
                details.AddRange(hourlyOrItemDetails);
            }
            if (hourlyOrItemSummary.Count > 0)
            {
                summary.AddRange(hourlyOrItemSummary);
            }

            //Processing the billable retainers
            var retainersToBill = context.BillableRetainers
                .Include(r => r.Creator)
                .ThenInclude(u => u.Attorney)
                .Where(br => br.ClientId == billHeader.ClientId && !br.IsBilled && br.IsBillable)
                .ToList();
            foreach (var r in retainersToBill)
            {
                
                //Adding the summary
                var retainerSummary = new BillSummary
                {
                    Date = r.InitialDate,
                    Description = r.BillingDescription,
                    Quantity = 1,
                    Time = r.ConsumedHours,
                    TypeId =BillDetailTypeEnum.Retainer,
                    Total = r.ConsumedHours <= r.AgreedHours ? r.AgreedFee : r.AgreedFee + ((r.ConsumedHours - r.AgreedHours) * r.AdditionalFeePerHour) 
                };
                var retainerHeaderDetail = new BillDetail
                {
                    ActivityId = null,
                    Date = r.InitialDate,
                    Description = r.BillingDescription,
                    Quantity = 1,
                    Time = 0,
                    BillDetailType = BillDetailTypeEnum.Retainer,
                    UnitCost = r.AgreedFee,
                    Subtotal = r.AgreedFee,
                    BillableRetainerId = r.Id,
                    Attorney = r.Creator.Attorney.Alias

                };
                
                //Adding the detail
                var retainerDetails = context.Activities
                    .Where(a => a.ClientId == billHeader.ClientId && a.BillableRetainerId == r.Id && a.ActivityType == ActivityTypeEnum.Retainer)
                    .Select(a => new BillDetail
                    {
                        ActivityId = a.Id,
                        BillDetailType = BillDetailTypeEnum.RetainerDetail,
                        //Quantity = a.BillableRate == (decimal)0.00 ? a.HoursWorked : a.BillableQuantity,
                        Quantity = 0,
                        Date = a.RealizationDate,
                        Attorney = a.Creator.Attorney.Alias,
                        Time = a.HoursWorked,
                        Subtotal = a.Subtotal,
                        UnitCost = a.BillableQuantity == 0 ? 0 : r.AdditionalFeePerHour,
                        Description = a.Description
                    }).ToList();

                details.Add(retainerHeaderDetail);
                summary.Add(retainerSummary);
                details.AddRange(retainerDetails);
                retainersToMark.Add(r);

            }
            var packagesIds = context.Activities
                .Where(a => a.ClientId == billHeader.ClientId
                && a.IsBillable && !a.IsBilled
                && (a.ActivityType == ActivityTypeEnum.Package) && a.PackageId != null)
                .Select(a => a.PackageId).ToList();
            //Calculation of total payments with all the packages To Include
            totalPayments = context.ClientDeposits
                .Where(d => packagesIds.Contains(d.PackageId))
                .Sum(d => d.Amount);

            var packagesToBill = context.Packages
                .Include(p => p.CreatorUser)
                .ThenInclude(u => u.Attorney)
                .Where(p => packagesIds.Contains(p.Id))
                .ToList();
            foreach (var p in packagesToBill)
            {
                decimal timeWorked = context.Activities.Where(a => a.PackageId == p.Id).Sum(a => a.HoursWorked);

                var packageSummary = new BillSummary
                {
                    Date = (DateTime) p.RealizationDate,
                    Description = p.Description,
                    Quantity = 1,
                    Time = timeWorked,
                    TypeId = BillDetailTypeEnum.Package,
                    Total = p.Amount
                };
                var packageHeaderDetail = new BillDetail
                {
                    ActivityId = null,
                    Date = (DateTime) p.RealizationDate,
                    Description = p.Description,
                    Quantity = 1,
                    Time = timeWorked,
                    BillDetailType = BillDetailTypeEnum.Package,
                    UnitCost = p.Amount,
                    Subtotal = p.Amount,
                    PackageId = p.Id,
                    Attorney = p.CreatorUser.Attorney.Alias

                };
                //processing the package details
                var packageDetails = context.Activities
                    .Where(a => a.ClientId == billHeader.ClientId && a.PackageId == p.Id)
                    .Select(a => new BillDetail
                    {
                        ActivityId = a.Id,
                        BillDetailType = BillDetailTypeEnum.PackageDetail,
                        //Quantity = a.BillableRate == (decimal)0.00 ? a.HoursWorked : a.BillableQuantity,
                        Quantity = 0,
                        Date = a.RealizationDate,
                        Attorney = a.Creator.Attorney.Alias,
                        Time = a.HoursWorked,
                        Subtotal = a.Subtotal,
                        UnitCost = 0,
                        Description = a.Description
                    }).ToList();
                //Processing the expenses
                var realExpenses = context.ActivityExpenses
                    .Include(ae => ae.Expense)
                    .Where(ae => ae.Activity.ActivityType == ActivityTypeEnum.Package && ae.Activity.PackageId == p.Id)
                    .ToList();
               
                decimal realExpenseAmount = realExpenses.Sum(re => re.TotalAmount);
                if (realExpenseAmount <= p.AgreedExpensesAmount)
                {
                    string spanishMonth = CultureInfo.GetCultureInfoByIetfLanguageTag("es").DateTimeFormat.GetMonthName(((DateTime)p.RealizationDate).Month);
                    string englishMonth = CultureInfo.GetCultureInfoByIetfLanguageTag("en").DateTimeFormat.GetMonthName(((DateTime)p.RealizationDate).Month);
                    //only add one single expense
                    var expenseDescription = IsEnglishBill ? $"Agreed expenses for {p.Description.ToLower()}"
                        : $"Gastos acordados por {p.Description.ToLower()}";
                    BillExpense expense = new BillExpense
                    {
                        Date = (DateTime) p.RealizationDate,
                        Cost = p.AgreedExpensesAmount,
                        Description = expenseDescription,
                        Month = ((DateTime)p.RealizationDate).Month,
                        Year = ((DateTime)p.RealizationDate).Year,
                        Quantity = 1,
                        Subtotal = p.AgreedExpensesAmount,
                        EnglishMonth = englishMonth,
                        SpanishMonth = spanishMonth
                    };
                    expenses.Add(expense);
                } else {
                    foreach (var e in realExpenses)
                    {
                        var expenseToAdd = new BillExpense
                        {
                            Date = e.RealizationDate,
                            Cost = e.UnitAmount,
                            Subtotal = e.TotalAmount,
                            Quantity = e.Quantity,
                            Month = e.RealizationDate.Month,
                            Year = e.RealizationDate.Year,
                            EnglishMonth = CultureInfo.GetCultureInfoByIetfLanguageTag("en").DateTimeFormat.GetMonthName(e.RealizationDate.Month),
                            SpanishMonth = CultureInfo.GetCultureInfoByIetfLanguageTag("es").DateTimeFormat.GetMonthName(e.RealizationDate.Month),
                            Description = IsEnglishBill ? e.Expense.EnglishDescription : e.Expense.SpanishDescription


                        };


                    expenses.Add(expenseToAdd);
                    }
                    
                }
                details.Add(packageHeaderDetail);
                details.AddRange(packageDetails);
                summary.Add(packageSummary);
                packagesToMark.Add(p);
            }
            totalExpenses = expenses.Sum(e => e.Subtotal);




            billHeader.BillDetails = details.OrderBy(d => d.Date).ToList();
            billHeader.BillSummaries = summary.OrderBy(d => d.Date).ToList();
            billHeader.BillExpenses = expenses.OrderBy(e => e.Date).ToList();
            billHeader.BillSubtotal = billHeader.BillDetails.Sum(d => d.Subtotal);
            billHeader.Taxes = client.PayTaxes ? billHeader.BillSubtotal * ivaValue : 0;
            billHeader.TotalExpenses = totalExpenses;
            billHeader.TotalPayments = totalPayments;
            billHeader.Total = billHeader.BillSubtotal + billHeader.Taxes + billHeader.TotalExpenses - billHeader.TotalPayments;

            
            //Persisting the changes
            var activities = billHeader.BillDetails.Where(d => d.BillDetailType != BillDetailTypeEnum.Package && d.BillDetailType != BillDetailTypeEnum.Retainer)
                .Select(d => new Activity
            {
                Id = Convert.ToInt32(d.ActivityId),
                IsBilled = true,
                IsBillable = false
            }).ToList();
            context.AttachRange(activities);
            context.AttachRange(packagesToMark);
            context.AttachRange(retainersToMark);
            foreach (var a in activities)
            {
                context.Entry<Activity>(a).Property(x => x.IsBilled).IsModified = true;
                context.Entry<Activity>(a).Property(x => x.IsBillable).IsModified = true;
            }
            foreach (var p in packagesToMark)
            {

                p.IsBilled = true;
                context.Entry<Package>(p).Property(x => x.IsBilled).IsModified = true;
            }
            foreach (var r in retainersToMark)
            {
                r.IsBillable = false;
                r.IsBilled = true;
                context.Entry<BillableRetainer>(r).Property(x => x.IsBilled).IsModified = true;
                context.Entry<BillableRetainer>(r).Property(x => x.IsBillable).IsModified = true;
            }
            context.Add(billHeader);
            context.SaveChanges();
            billHeader.Client = client;
            return billHeader;
        }

        public PreBill GeneratePreBill(PreBillRequest preBillingRequest, out bool IsEnglishBill)
        {

            var client = context.Clients.Where(c => c.Id == preBillingRequest.ClientId).FirstOrDefault();
            IsEnglishBill = client.BillingInEnglish;
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            PreBill preBill = new PreBill
            {
                ClientName = client.Name,
                Date = preBillingRequest.BillDate,
                Details = new List<PreBillDetail>()
            };
            decimal totalFee = 0;
            decimal totalExpenses = 0;
            if(preBillingRequest.IncludeItems ?? false)
            {
                //Include the items not billed yet for the client
                var itemActivities = context.Activities
                    .Where(a => a.ClientId == preBillingRequest.ClientId && a.ActivityType == ActivityTypeEnum.Item && !a.IsBilled)
                    .OrderBy(a => a.RealizationDate)
                    .Select(a => new PreBillDetail
                    {
                        Quantity = Convert.ToInt32(a.BillableQuantity),
                        Description = a.Description,
                        Subtotal = a.Subtotal,
                        UnitPrice = a.BillableRate
                    }).ToList();
                preBill.Details.AddRange(itemActivities);
                totalFee += itemActivities.Sum(d => d.Subtotal);
            }
            
            
            
            if (preBillingRequest.PackageId != null)
            {
                var packages = context.Packages
                    .Where(p => (preBillingRequest.PackageId == -1 || 
                    p.Id == preBillingRequest.PackageId)
                    /*&& p.IsFinished == true */&& p.IsBilled == false && p.ClientId == preBillingRequest.ClientId)
                    .ToList();
                var result = new List<BillDetail>();
                foreach (var p in packages)
                {
                    var packagePreBillDetail = new PreBillDetail
                    {
                        Description = p.Description,
                        Quantity = 1,
                        Subtotal = p.Amount,
                        UnitPrice = p.Amount
                    };
                    totalFee += packagePreBillDetail.Subtotal;
                    var packageExpenseTemplate = IsEnglishBill ? "Expenses for {0}" : "Gastos por {0}";
                    var expensePreBillDetail = new PreBillDetail
                    {
                        Quantity = 1,
                        Description = String.Format(packageExpenseTemplate, p.Description.ToLower()),
                        Subtotal = p.AgreedExpensesAmount,
                        UnitPrice = p.AgreedExpensesAmount
                    };
                    totalExpenses += expensePreBillDetail.Subtotal;
                    preBill.Details.Add(packagePreBillDetail);
                    preBill.Details.Add(expensePreBillDetail);
                    
                }
            }
            if (preBillingRequest.BillableRetainerId != null)
            {
                var retainersPreBillDetails = context.BillableRetainers
                    .Where(r => (preBillingRequest.BillableRetainerId == -1 || r.Id == preBillingRequest.BillableRetainerId) 
                && r.IsBilled == false && r.ClientId == preBillingRequest.ClientId
                /*&& r.Month == preBillingRequest.BillMonth && r.Year == preBillingRequest.BillYear*/)
                .Select(r => new PreBillDetail {
                    Description = r.BillingDescription,
                    Quantity = 1,
                    Subtotal = r.AgreedFee,
                    UnitPrice = r.AgreedFee
                }).ToList();
                preBill.Details.AddRange(retainersPreBillDetails);
            }
            preBill.TotalFee = totalFee;
            preBill.TotalExpenses = totalExpenses;
            preBill.Tax = client.PayTaxes ? ivaValue * totalFee : 0;
            preBill.Total = preBill.TotalFee + preBill.TotalExpenses + preBill.Tax;
            
            return preBill;
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
