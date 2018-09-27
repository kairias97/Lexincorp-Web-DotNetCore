using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFActivityRepository : IActivityRepository
    {
        private ApplicationDbContext context;
        public EFActivityRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<Activity> Activities { get => context.Activities; }
        public void Save(NewActivityRequest newActivityRequest, int creatorId)
        {
            var activity = new Activity();
            activity.Description = newActivityRequest.Description;
            activity.HoursWorked = newActivityRequest.HoursWorked;
            activity.ClientId = newActivityRequest.ClientId;
            activity.RealizationDate = newActivityRequest.ActivityDate;
            activity.ServiceId = newActivityRequest.ServiceId;
            if(newActivityRequest.ActivityType == ActivityTypeEnum.Package)
            {
                activity.PackageId = newActivityRequest.PackageId;
                activity.BillableQuantity = 0;
                activity.BillableRate = 0;
                activity.Subtotal = 0;
            }
            else if(newActivityRequest.ActivityType == ActivityTypeEnum.Retainer)
            {
                var retainer = context.BillableRetainers.Where(r => r.Id == newActivityRequest.BillableRetainerId).FirstOrDefault();
                var availablHours = retainer.AgreedHours - retainer.ConsumedHours;
                if (newActivityRequest.HoursWorked > availablHours)
                {
                    activity.BillableQuantity = newActivityRequest.HoursWorked - availablHours;
                    activity.BillableRate = retainer.AdditionalFeePerHour;
                    activity.Subtotal = activity.BillableQuantity * activity.BillableRate;
                    retainer.ConsumedHours = retainer.AgreedHours;
                    activity.BillableRetainerId = newActivityRequest.BillableRetainerId;
                }
                else
                {
                    activity.BillableQuantity = 0;
                    activity.BillableRate = 0;
                    activity.Subtotal = activity.BillableQuantity * activity.BillableRate;
                    retainer.ConsumedHours += newActivityRequest.HoursWorked;
                    activity.BillableRetainerId = newActivityRequest.BillableRetainerId;
                }
            }
            else if(newActivityRequest.ActivityType == ActivityTypeEnum.Item)
            {
                activity.BillableQuantity = Convert.ToDecimal(newActivityRequest.ItemQuantity);
                activity.BillableRate = Convert.ToDecimal(newActivityRequest.ItemUnitPrice);
                activity.Subtotal = Convert.ToDecimal(newActivityRequest.ItemSubTotal);
                activity.ItemId = newActivityRequest.ItemId;
            }
            else if(newActivityRequest.ActivityType == ActivityTypeEnum.Hourly)
            {
                activity.BillableQuantity = Convert.ToDecimal(newActivityRequest.HoursWorked);
                activity.BillableRate = Convert.ToDecimal(newActivityRequest.HourlyRate);
                activity.Subtotal = Convert.ToDecimal(newActivityRequest.HourlySubtotal);
            }
            activity.TaxesAmount = 0;
            activity.PayTaxes = false;
            activity.TotalAmount = 0;
            activity.IsBilled = false;
            activity.ActivityType = newActivityRequest.ActivityType;
            activity.CreatorId = creatorId;

            activity.ActivityExpenses = new List<ActivityExpense>();

            foreach(var expense in newActivityRequest.Expenses)
            {
                var e = new ActivityExpense();
                e.ExpenseId = expense.ExpenseID;
                e.Quantity = expense.Quantity;
                e.UnitAmount = expense.UnitCost;
                e.TotalAmount = expense.SubTotal;
                e.RealizationDate = newActivityRequest.ActivityDate;

                activity.ActivityExpenses.Add(e);
            }
            context.Add(activity);
            context.SaveChanges();
        }
    }
}
