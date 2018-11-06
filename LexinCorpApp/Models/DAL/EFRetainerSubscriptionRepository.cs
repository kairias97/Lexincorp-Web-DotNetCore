using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LexincorpApp.Models
{
    public class EFRetainerSubscriptionRepository : IRetainerSubscriptionRepository
    {
        private ApplicationDbContext context;

        public EFRetainerSubscriptionRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<RetainerSubscription> Subscriptions { get => context.RetainerSubscriptions; }

        public void Apply(out bool success, out string message)
        {
            //Take out later the false
            if (false && DateTime.UtcNow.Day != 1)
            {
                success = false;
                message = "Se ejecutó el cron job en una fecha que no es el primero del mes";
                Log.Information("Se ejecutó el cron job en una fecha que no es el primero del mes");
                return;
            }
            try
            {
                string spanishMonth = System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("es").DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
                string englishMonth = System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("en").DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
                if (!context.BillableRetainers.Any(br => br.Month == DateTime.UtcNow.Month && br.Year == DateTime.UtcNow.Year))
                {
                    var newBillableRetainers = context.RetainerSubscriptions
                    .Include(rs => rs.Client)
                    .Include(rs => rs.Retainer)
                    .Select(rs => new BillableRetainer
                    {
                        AdditionalFeePerHour = rs.AdditionalFeePerHour,
                        RetainerId = rs.RetainerId,
                        IsBilled = false,
                        AgreedFee = rs.AgreedFee,
                        AgreedHours = rs.AgreedHours,
                        ClientId = rs.ClientId,
                        CreatorId = rs.CreatorId,
                        Name = $"{rs.Retainer.SpanishName} - {spanishMonth} {DateTime.Now.Year}",
                        BillingDescription = rs.Client.BillingInEnglish ? $"{rs.Retainer.EnglishName} - {englishMonth} {DateTime.Now.Year}" : $"{rs.Retainer.SpanishName} - {spanishMonth} {DateTime.Now.Year}",
                        Month = DateTime.Now.Month,
                        Year = DateTime.Now.Year,
                        IsVisibleForActivities = rs.IsVisibleForActivities
                    }).ToList();
                    context.BillableRetainers.AddRange(newBillableRetainers);
                    //To clean up the subscriptions
                    var subscriptionsToWipe = context.RetainerSubscriptions.ToList();
                    context.RetainerSubscriptions.RemoveRange(subscriptionsToWipe);

                    context.SaveChanges();
                    var count = newBillableRetainers.Count;
                    message = $"Se aplicaron exitosamente {count} suscripciones de retainers activas y se convirtieron en billable retainers";
                    Log.Information("Se aplicaron exitosamente {Count} suscripciones de retainers activas y se convirtieron en billable retainers", count);
                }
                else
                {
                    message = "No hubo suscripciones de retainers para ser aplicadas este mes";
                    Log.Information("No hubo suscripciones de retainers para ser aplicadas este mes o ya fueron aplicadas previamente");
                }
                success = true;

            }
            catch (Exception ex)
            {
                success = false;
                message = "Ocurrió un error al ejecutar el proceso programado para las suscripciones";
                Log.Error(ex, "Error al ejecutar cronjob de BillableRetainers");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public void Delete(int subscriptionId)
        {
            var subscription = new RetainerSubscription { Id = subscriptionId };
            context.RetainerSubscriptions.Attach(subscription);
            context.RetainerSubscriptions.Remove(subscription);
            context.SaveChanges();
        }

        public void Save(RetainerSubscription subscription)
        {
            if (subscription.Id == 0)
            {
                context.RetainerSubscriptions.Add(subscription);
            } else
            {
                context.Entry(subscription).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.Entry(subscription).Property(rs => rs.CreatorId).IsModified = false;
                context.Entry(subscription).Property(rs => rs.ClientId).IsModified = false;
                context.Entry(subscription).Property(rs => rs.RetainerId).IsModified = false;
            }
            context.SaveChanges();
        }
        
    }
}
