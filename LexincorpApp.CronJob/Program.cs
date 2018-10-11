using System;
using System.IO;
using System.Linq;
using LexincorpApp.CronJob.Models;
//using LexincorpApp.CronJob.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace LexincorpApp.CronJob
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setting up config file
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            DbContextOptionsBuilder<LexincorpAdminContext> optionsBuilder = new DbContextOptionsBuilder<LexincorpAdminContext>()
                .UseSqlServer(config["cnLexincorpDB"]);
            

            using (var context = new LexincorpAdminContext(optionsBuilder.Options))
            {

                string spanishMonth = System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("es").DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
                string englishMonth = System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag("en").DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
                if (!context.BillableRetainers.Any(br => br.Month == DateTime.UtcNow.Month && br.Year == DateTime.UtcNow.Year))
                {
                    var newBillableRetainers = context.RetainerSubscriptions
                    .Include(rs => rs.Client)
                    .Include(rs => rs.Retainer)
                    .Select(rs => new BillableRetainers
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
                        Year = DateTime.Now.Year
                    }).ToList();
                    context.BillableRetainers.AddRange(newBillableRetainers);
                    context.SaveChanges();
                }

            }

        }
    }
}
