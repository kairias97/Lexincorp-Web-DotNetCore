using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFActivityRepository : IActivityRepository
    {
        private ApplicationDbContext context;
        private IConfiguration configuration;
        public EFActivityRepository(ApplicationDbContext ctx, IConfiguration configuration)
        {
            this.context = ctx;
            this.configuration = configuration;
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
                retainer.IsBilled = false;
            }
            else if(newActivityRequest.ActivityType == ActivityTypeEnum.Item)
            {
                activity.BillableQuantity = Convert.ToDecimal(newActivityRequest.ItemQuantity);
                activity.BillableRate = Convert.ToDecimal(newActivityRequest.ItemUnitPrice);
                activity.Subtotal = Convert.ToDecimal(newActivityRequest.ItemSubTotal);
                //activity.ItemId = newActivityRequest.ItemId;
            }
            else if(newActivityRequest.ActivityType == ActivityTypeEnum.Hourly)
            {
                activity.BillableQuantity = Convert.ToDecimal(newActivityRequest.HoursWorked);
                activity.BillableRate = Convert.ToDecimal(newActivityRequest.HourlyRate);
                activity.Subtotal = Convert.ToDecimal(newActivityRequest.HourlySubtotal);
            }
            else if(newActivityRequest.ActivityType == ActivityTypeEnum.NoBillable)
            {
                activity.BillableQuantity = 0;
                activity.BillableRate = 0;
                activity.Subtotal = activity.BillableQuantity * activity.BillableRate;
            }
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            var client = context.Clients.Where(c => c.Id == newActivityRequest.ClientId).FirstOrDefault();
            if (client.PayTaxes)
            {
                activity.TaxesAmount = activity.Subtotal * ivaValue;
                activity.PayTaxes = true;
                activity.TotalAmount = activity.Subtotal + activity.TaxesAmount;
            }
            else
            {
                activity.TaxesAmount = 0;
                activity.PayTaxes = false;
                activity.TotalAmount = activity.Subtotal;
            }
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
        public void Update(UpdateActivityRequest updateActivityRequest)
        {
            var activity = context.Activities.Where(a => a.Id == updateActivityRequest.Id).FirstOrDefault();
            var client = context.Clients.Where(c => c.Id == activity.ClientId).FirstOrDefault();
            activity.HoursWorked = updateActivityRequest.HoursWorked;
            activity.Description = updateActivityRequest.Description;
            if(activity.ActivityType == ActivityTypeEnum.Hourly)
            {
                activity.BillableQuantity = Convert.ToDecimal(updateActivityRequest.HoursWorked);
                activity.BillableRate = Convert.ToDecimal(updateActivityRequest.HourlyRate);
                activity.Subtotal = activity.BillableQuantity * activity.BillableRate;
                if (client.PayTaxes)
                {
                    decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
                    activity.TaxesAmount = activity.Subtotal * ivaValue;
                    activity.PayTaxes = true;
                    activity.TotalAmount = activity.Subtotal + activity.TaxesAmount;
                }
                else
                {
                    activity.TaxesAmount = 0;
                    activity.PayTaxes = false;
                    activity.TotalAmount = activity.Subtotal;
                }
            }
            else if(activity.ActivityType == ActivityTypeEnum.Retainer)
            {
                var activities = context.Activities.Where(a => a.BillableRetainerId == activity.BillableRetainerId).OrderBy(a => a.RealizationDate).ToList();
                var retainer = context.BillableRetainers.Where(r => r.Id == activity.BillableRetainerId).FirstOrDefault();
                retainer.ConsumedHours = 0;
                var availablHours = retainer.AgreedHours - retainer.ConsumedHours;
                foreach(var r in activities)
                {
                    if (r.Id == updateActivityRequest.Id)
                    {
                        if (updateActivityRequest.HoursWorked > availablHours)
                        {
                            activity.BillableQuantity = updateActivityRequest.HoursWorked - availablHours;
                            activity.BillableRate = retainer.AdditionalFeePerHour;
                            activity.Subtotal = activity.BillableQuantity * activity.BillableRate;
                            retainer.ConsumedHours = retainer.AgreedHours;
                        }
                        else
                        {
                            activity.BillableQuantity = 0;
                            activity.BillableRate = 0;
                            activity.Subtotal = activity.BillableQuantity * activity.BillableRate;
                            retainer.ConsumedHours += updateActivityRequest.HoursWorked;
                        }

                        if (client.PayTaxes)
                        {
                            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
                            activity.TaxesAmount = activity.Subtotal * ivaValue;
                            activity.PayTaxes = true;
                            activity.TotalAmount = activity.Subtotal + activity.TaxesAmount;
                        }
                        else
                        {
                            activity.TaxesAmount = 0;
                            activity.PayTaxes = false;
                            activity.TotalAmount = activity.Subtotal;
                        }
                    }
                    else
                    {
                        if (r.HoursWorked > availablHours)
                        {
                            r.BillableQuantity = r.HoursWorked - availablHours;
                            r.BillableRate = retainer.AdditionalFeePerHour;
                            r.Subtotal = r.BillableQuantity * r.BillableRate;
                            retainer.ConsumedHours = retainer.AgreedHours;
                        }
                        else
                        {
                            r.BillableQuantity = 0;
                            r.BillableRate = 0;
                            r.Subtotal = r.BillableQuantity * r.BillableRate;
                            retainer.ConsumedHours += r.HoursWorked;
                        }
                        if (client.PayTaxes)
                        {
                            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
                            r.TaxesAmount = r.Subtotal * ivaValue;
                            r.PayTaxes = true;
                            r.TotalAmount = r.Subtotal + r.TaxesAmount;
                        }
                        else
                        {
                            r.TaxesAmount = 0;
                            r.PayTaxes = false;
                            r.TotalAmount = r.Subtotal;
                        }
                    }
                    availablHours = retainer.AgreedHours - retainer.ConsumedHours;
                }
            }
            else if(activity.ActivityType == ActivityTypeEnum.Item)
            {
                activity.BillableQuantity = Convert.ToDecimal(updateActivityRequest.ItemQuantity);
                activity.BillableRate = Convert.ToDecimal(updateActivityRequest.ItemUnitPrice);
                activity.Subtotal = Convert.ToDecimal(updateActivityRequest.ItemSubTotal);
                if (client.PayTaxes)
                {
                    decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
                    activity.TaxesAmount = activity.Subtotal * ivaValue;
                    activity.PayTaxes = true;
                    activity.TotalAmount = activity.Subtotal + activity.TaxesAmount;
                }
                else
                {
                    activity.TaxesAmount = 0;
                    activity.PayTaxes = false;
                    activity.TotalAmount = activity.Subtotal;
                }
            }
            foreach(var expense in activity.ActivityExpenses)
            {
                context.ActivityExpenses.Remove(expense);
            }
            activity.ActivityExpenses = new List<ActivityExpense>();
            foreach (var expense in updateActivityRequest.Expenses)
            {
                var e = new ActivityExpense();
                e.ExpenseId = expense.ExpenseID;
                e.Quantity = expense.Quantity;
                e.UnitAmount = expense.UnitCost;
                e.TotalAmount = expense.SubTotal;
                e.RealizationDate = activity.RealizationDate;

                activity.ActivityExpenses.Add(e);
            }
            context.SaveChanges();
        }
        public IQueryable<ActivityExpense> Expenses { get => context.ActivityExpenses; }
    }
}
