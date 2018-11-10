using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LexincorpApp.Infrastructure;
using System.IO;
using Syncfusion.ReportWriter;
using Syncfusion.Report;
using System.Security.Claims;
using LexincorpApp.Models.ViewModels;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    [Authorize(Roles ="Administrador, Regular")]
    public class BillController : Controller
    {
        private readonly IItemRepository _itemRepo;
        private readonly IActivityRepository _activityRepo;
        private readonly IPackageRepository _packageRepo;
        private readonly IBillableRetainerRepository _billableRetainerRepo;
        private readonly IClientRepository _clientRepo;
        private IConfiguration configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IBillRepository _billRepo;
        public int PageSize = 5;
        public BillController(IItemRepository itemRepository, IActivityRepository activityRepository, IPackageRepository packageRepository, IBillableRetainerRepository billableRetainerRepository,
            IClientRepository clientRepository, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IBillRepository billHeaderRepository)
        {
            this._itemRepo = itemRepository;
            this._activityRepo = activityRepository;
            this._packageRepo = packageRepository;
            this._billableRetainerRepo = billableRetainerRepository;
            this._clientRepo = clientRepository;
            this.configuration = configuration;
            this._hostingEnvironment = hostingEnvironment;
            this._billRepo = billHeaderRepository;
        }
        [Authorize(Policy = "CanPreBill")]
        public IActionResult PreBilling()
        {
            NewPreBillViewModel viewModel = new NewPreBillViewModel();
            viewModel.Items = _itemRepo.Items.ToList();
            return View(viewModel);
        }
        [HttpPost]
        [Authorize(Policy = "CanPreBill")]
        public JsonResult GeneratePreBill(BillRequest billRequest)
        {
            bool isEnglishBilling;
            var preBill = _billRepo.GeneratePreBill(billRequest, out isEnglishBilling);
            //Report setup
            var details = preBill.BillDetails.Select(d => new
            {
                BillDetailType = Convert.ToInt32(d.BillDetailType),
                FixedAmount = d.FixedAmount,
                UnitRate = d.UnitRate,
                Quantity = d.Quantity,
                Subtotal = d.Subtotal,
                Description = d.Description,
                TaxesAmount = d.TaxesAmount
            }).ToList();
            
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Bill.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = details });
            
            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();
            //IsEnglish
            ReportParameter isEnglishParam = new ReportParameter();
            isEnglishParam.Name = "IsEnglish";
            isEnglishParam.Values = new List<string>() { isEnglishBilling ? "1" : "0" };
            //IsBilled
            ReportParameter isBilledParam = new ReportParameter();
            isBilledParam.Name = "IsBilled";
            isBilledParam.Values = new List<string>() { "0" };
            //ClientName
            ReportParameter clientNameParam = new ReportParameter();
            clientNameParam.Name = "ClientName";
            clientNameParam.Values = new List<string>() { preBill.BillName };
            //BillingPeriod

            string culture = isEnglishBilling ? "en" : "es";
            string billingPeriod = $"{CultureInfo.GetCultureInfoByIetfLanguageTag(culture).DateTimeFormat.GetMonthName(DateTime.UtcNow.AddHours(-6).Month)} - {preBill.BillYear}";

            ReportParameter billingPeriodParam = new ReportParameter();
            billingPeriodParam.Name = "BillingPeriod";
            billingPeriodParam.Values = new List<string>() { billingPeriod };
            //Subtotal
            ReportParameter subTotalExpenseParam = new ReportParameter();
            subTotalExpenseParam.Name = "Subtotal";
            subTotalExpenseParam.Values = new List<string>() { preBill.BillSubtotal.ToString() };
            //Discount
            ReportParameter discountParam = new ReportParameter();
            discountParam.Name = "Discount";
            discountParam.Values = new List<string>() { preBill.BillDiscount.ToString() };
            //Taxes
            ReportParameter taxesParam = new ReportParameter();
            taxesParam.Name = "Taxes";
            taxesParam.Values = new List<string>() { preBill.Taxes.ToString() };
            //TotalExpense
            ReportParameter totalParam = new ReportParameter();
            totalParam.Name = "Total";
            totalParam.Values = new List<string>() { preBill.Total.ToString() };

            //TotalExpense
            ReportParameter billDateParam = new ReportParameter();
            billDateParam.Name = "BillDate";
            billDateParam.Values = new List<string>() { preBill.BillDate.ToString("dd/MM/yyyy") };

            parameters.Add(isEnglishParam);
            parameters.Add(isBilledParam);
            parameters.Add(clientNameParam);
            parameters.Add(billingPeriodParam);
            parameters.Add(subTotalExpenseParam);
            parameters.Add(discountParam);
            parameters.Add(taxesParam);
            parameters.Add(totalParam);
            parameters.Add(billDateParam);
            writer.SetParameters(parameters);
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{preBill.BillName}deMes{preBill.BillMonth}YAño{preBill.BillYear}.pdf";
            return Json(new { result = memoryStream.ConvertToBase64() });
        }

        [Authorize(Policy = "CanBill")]
        public IActionResult RenderBill(int id)
        {
            var bill = _billRepo.BillHeaders.Where(b => b.Id == id).Include(b => b.BillDetails).FirstOrDefault();
            var details = bill.BillDetails.Select(d => new
            {
                BillDetailType = Convert.ToInt32(d.BillDetailType),
                d.FixedAmount,
                d.UnitRate,
                d.Quantity,
                d.Subtotal,
                d.Description,
                d.TaxesAmount
            }).ToList();
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Bill.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = details });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();
            //IsEnglish
            bool isBillingInEnglish = _clientRepo.Clients.Where(c => c.Id == bill.ClientId).Select(c => c.BillingInEnglish).First();
            ReportParameter isEnglishParam = new ReportParameter();
            isEnglishParam.Name = "IsEnglish";
            isEnglishParam.Values = new List<string>() { isBillingInEnglish ? "1" : "0" };
            //IsBilled
            ReportParameter isBilledParam = new ReportParameter();
            isBilledParam.Name = "IsBilled";
            isBilledParam.Values = new List<string>() { "1" };
            //ClientName
            ReportParameter clientNameParam = new ReportParameter();
            clientNameParam.Name = "ClientName";
            clientNameParam.Values = new List<string>() { bill.BillName };
            //BillingPeriod

            string culture = isBillingInEnglish ? "en" : "es";
            string billingPeriod = $"{CultureInfo.GetCultureInfoByIetfLanguageTag(culture).DateTimeFormat.GetMonthName(DateTime.UtcNow.AddHours(-6).Month)} - {bill.BillYear}";

            ReportParameter billingPeriodParam = new ReportParameter();
            billingPeriodParam.Name = "BillingPeriod";
            billingPeriodParam.Values = new List<string>() { billingPeriod };
            //Subtotal
            ReportParameter subTotalExpenseParam = new ReportParameter();
            subTotalExpenseParam.Name = "Subtotal";
            subTotalExpenseParam.Values = new List<string>() { bill.BillSubtotal.ToString() };
            //Discount
            ReportParameter discountParam = new ReportParameter();
            discountParam.Name = "Discount";
            discountParam.Values = new List<string>() { bill.BillDiscount.ToString() };
            //Taxes
            ReportParameter taxesParam = new ReportParameter();
            taxesParam.Name = "Taxes";
            taxesParam.Values = new List<string>() { bill.Taxes.ToString() };
            //TotalExpense
            ReportParameter totalParam = new ReportParameter();
            totalParam.Name = "Total";
            totalParam.Values = new List<string>() { bill.Total.ToString() };
            //TotalExpense
            ReportParameter billDateParam = new ReportParameter();
            billDateParam.Name = "BillDate";
            billDateParam.Values = new List<string>() { bill.BillDate.ToString() };

            parameters.Add(isEnglishParam);
            parameters.Add(isBilledParam);
            parameters.Add(clientNameParam);
            parameters.Add(billingPeriodParam);
            parameters.Add(subTotalExpenseParam);
            parameters.Add(discountParam);
            parameters.Add(taxesParam);
            parameters.Add(totalParam);
            parameters.Add(billDateParam);
            writer.SetParameters(parameters);
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{bill.BillName}deMes{bill.BillMonth}YAño{bill.BillYear}.pdf";
            return fileStreamResult;
        }
        [Authorize(Policy = "CanBill")]
        public IActionResult Billing()
        {
            NewPreBillViewModel viewModel = new NewPreBillViewModel();
            viewModel.Items = _itemRepo.Items.ToList();
            return View(viewModel);
        }
        [Authorize(Policy = "CanBill")]
        public JsonResult GenerateBill(BillRequest billRequest)
        {
            var user = HttpContext.User;
            var id = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            bool isBillingInEnglish;
            var generatedBill = _billRepo.GenerateBill(billRequest, id, out isBillingInEnglish);
            //RenderBill(billHeader);

            var details = generatedBill.BillDetails.Select(d => new
            {
                BillDetailType = Convert.ToInt32(d.BillDetailType),
                d.FixedAmount,
                d.UnitRate,
                d.Quantity,
                d.Subtotal,
                d.Description,
                d.TaxesAmount
            }).ToList();
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Bill.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = details });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();
            //IsEnglish
            ReportParameter isEnglishParam = new ReportParameter();
            isEnglishParam.Name = "IsEnglish";
            isEnglishParam.Values = new List<string>() { isBillingInEnglish ? "1" : "0" };
            //IsBilled
            ReportParameter isBilledParam = new ReportParameter();
            isBilledParam.Name = "IsBilled";
            isBilledParam.Values = new List<string>() { "1" };
            //ClientName
            ReportParameter clientNameParam = new ReportParameter();
            clientNameParam.Name = "ClientName";
            clientNameParam.Values = new List<string>() { generatedBill.BillName};
            //BillingPeriod
            
            string culture = isBillingInEnglish ? "en" : "es";
            string billingPeriod = $"{CultureInfo.GetCultureInfoByIetfLanguageTag(culture).DateTimeFormat.GetMonthName(DateTime.UtcNow.AddHours(-6).Month)} - {generatedBill.BillYear}"; 
            
            ReportParameter billingPeriodParam = new ReportParameter();
            billingPeriodParam.Name = "BillingPeriod";
            billingPeriodParam.Values = new List<string>() { billingPeriod};
            //Subtotal
            ReportParameter subTotalExpenseParam = new ReportParameter();
            subTotalExpenseParam.Name = "Subtotal";
            subTotalExpenseParam.Values = new List<string>() { generatedBill.BillSubtotal.ToString() };
            //Discount
            ReportParameter discountParam = new ReportParameter();
            discountParam.Name = "Discount";
            discountParam.Values = new List<string>() { generatedBill.BillDiscount.ToString() };
            //Taxes
            ReportParameter taxesParam = new ReportParameter();
            taxesParam.Name = "Taxes";
            taxesParam.Values = new List<string>() { generatedBill.Taxes.ToString() };
            //TotalExpense
            ReportParameter totalParam = new ReportParameter();
            totalParam.Name = "Total";
            totalParam.Values = new List<string>() { generatedBill.Total.ToString() };
            //TotalExpense
            ReportParameter billDateParam = new ReportParameter();
            billDateParam.Name = "BillDate";
            billDateParam.Values = new List<string>() { generatedBill.BillDate.ToString() };
            parameters.Add(isEnglishParam);
            parameters.Add(isBilledParam);
            parameters.Add(clientNameParam);
            parameters.Add(billingPeriodParam);
            parameters.Add(subTotalExpenseParam);
            parameters.Add(discountParam);
            parameters.Add(taxesParam);
            parameters.Add(totalParam);
            parameters.Add(billDateParam);
            writer.SetParameters(parameters);

            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{generatedBill.BillName}deMes{generatedBill.BillMonth}YAño{generatedBill.BillYear}.pdf";
            return Json(new { result = memoryStream.ConvertToBase64() });
        }
        [Authorize(Policy = "CanBill")]
        public IActionResult History(string filter, int pageNumber = 1)
        {
            Func<BillHeader, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Client.Name.Contains(filter) || c.BillName.Contains(filter);
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            BillHistoryViewModel viewModel = new BillHistoryViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.BillHeaders = _billRepo.BillHeaders.Include(b => b.Client).Where(b => b.CreatorId == Convert.ToInt32(id))
                .Where(filterFunction)
                .OrderByDescending(b => b.BillDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _billRepo.BillHeaders.Where(a => a.CreatorId == Convert.ToInt32(id)).Count(filterFunction)
            };
            return View(viewModel);
        }
    }
}
