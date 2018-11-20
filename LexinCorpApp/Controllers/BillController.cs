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
        public JsonResult GeneratePreBill(PreBillRequest billRequest)
        {
            bool isEnglishBilling;
            var preBill = _billRepo.GeneratePreBill(billRequest, out isEnglishBilling);
            //Report setup
            var details = preBill.Details.Select(d => new
            {
                d.Quantity,
                d.Description,
                d.UnitPrice,
                d.Subtotal
            }).ToList();
            
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/PreBill.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "DSPreBill", Value = details });
            
            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();
            
            //ClientName
            ReportParameter clientNameParam = new ReportParameter();
            clientNameParam.Name = "ClientName";
            clientNameParam.Values = new List<string>() { preBill.ClientName };
            
            //Subtotal
            ReportParameter subTotalExpenseParam = new ReportParameter();
            subTotalExpenseParam.Name = "TotalFee";
            subTotalExpenseParam.Values = new List<string>() { preBill.TotalFee.ToString() };
            //Taxes
            ReportParameter taxesParam = new ReportParameter();
            taxesParam.Name = "Tax";
            taxesParam.Values = new List<string>() { preBill.Tax.ToString() };
            //TotalAmount
            ReportParameter totalParam = new ReportParameter();
            totalParam.Name = "TotalAmount";
            totalParam.Values = new List<string>() { preBill.Total.ToString() };
            //TotalExpense
            ReportParameter totalExpenseParam = new ReportParameter();
            totalExpenseParam.Name = "TotalExpense";
            totalExpenseParam.Values = new List<string>() { preBill.TotalExpenses.ToString() };
            //BillDate
            ReportParameter billDateParam = new ReportParameter();
            billDateParam.Name = "BillDate";
            billDateParam.Values = new List<string>() { preBill.Date.ToString("dd/MM/yyyy") };
            
            parameters.Add(clientNameParam);
            parameters.Add(subTotalExpenseParam);
            parameters.Add(taxesParam);
            parameters.Add(totalParam);
            parameters.Add(totalExpenseParam);
            parameters.Add(billDateParam);

            writer.SetParameters(parameters);
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"PreFacturaCliente{preBill.ClientName}de{preBill.Date.ToString("ddMMyyyy")}.pdf";
            return Json(new { result = memoryStream.ConvertToBase64(), name = fileStreamResult.FileDownloadName });
        }

        [Authorize(Policy = "CanBill")]
        public IActionResult RenderBill(int id)
        {
            var bill = _billRepo.BillHeaders
                .Include(b => b.BillDetails)
                .Include(b => b.BillSummaries)
                .Include(b => b.BillExpenses)
                .Include(b => b.Client)
                .Where(b => b.Id == id)
                .FirstOrDefault();
            bool isBillingInEnglish = bill.Client.BillingInEnglish;

            var details = bill.BillDetails.Select(d => new
            {
                TypeId = Convert.ToInt32(d.BillDetailType),
                Cost = d.UnitCost,
                Hours = d.Time,
                Quantity = Convert.ToInt32(d.Quantity),
                Subtotal = d.Subtotal,
                Description = d.Description,
                Date = d.Date.ToString("dd/MM/yyyy"),
                Alias = d.Attorney
            }).ToList();
            //Generate summary
            var summary = bill.BillSummaries.Select(d => new
            {
                TypeId = Convert.ToInt32(d.TypeId),
                Date = d.Date.ToString("dd/MM/yyyy"),
                Description = d.Description,
                Time = d.Time,
                Quantity = d.Quantity,
                Total = d.Total
            }).ToList();
            //Generate expenses
            var expenses = bill.BillExpenses.Select(d => new
            {
                Date = d.Date.ToString("dd/MM/yyyy"),
                Description = d.Description,
                Cost = d.Cost,
                Quantity = d.Quantity,
                Subtotal = d.Subtotal,
                Month = d.Month,
                Year = d.Year,
                SpanishMonth = d.SpanishMonth,
                EnglishMonth = d.EnglishMonth

            }).ToList();
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Bill2.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();

            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = details });
            dataSources.Add(new ReportDataSource { Name = "DSSummary", Value = summary });
            dataSources.Add(new ReportDataSource { Name = "DSExpenses", Value = expenses });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();

            //ClientName
            ReportParameter clientNameParam = new ReportParameter();
            clientNameParam.Name = "ClientName";
            clientNameParam.Values = new List<string>() { bill.Client.Name };
            //Subtotal
            ReportParameter subTotalExpenseParam = new ReportParameter();
            subTotalExpenseParam.Name = "SubtotalFee";
            subTotalExpenseParam.Values = new List<string>() { bill.BillSubtotal.ToString() };

            //Taxes
            ReportParameter taxesParam = new ReportParameter();
            taxesParam.Name = "Tax";
            taxesParam.Values = new List<string>() { bill.Taxes.ToString() };
            //TotalAmount
            ReportParameter totalParam = new ReportParameter();
            totalParam.Name = "TotalAmount";
            totalParam.Values = new List<string>() { bill.Total.ToString() };
            //BillDate
            ReportParameter billDateParam = new ReportParameter();
            billDateParam.Name = "BillDate";
            billDateParam.Values = new List<string>() { bill.BillDate.ToString("dd/MM/yyyy") };
            //TotalExpense
            ReportParameter expensesParam = new ReportParameter();
            expensesParam.Name = "TotalExpenses";
            expensesParam.Values = new List<string>() { bill.TotalExpenses.ToString() };
            //TotalPayments
            ReportParameter paymentsParams = new ReportParameter();
            paymentsParams.Name = "TotalPayments";
            paymentsParams.Values = new List<string>() { bill.TotalPayments.ToString() };
            //IsEnglish
            ReportParameter isEnglishParam = new ReportParameter();
            isEnglishParam.Name = "IsEnglish";
            isEnglishParam.Values = new List<string>() { isBillingInEnglish ? "1" : "0" };
            //IsEnglish
            ReportParameter numberParam = new ReportParameter();
            isEnglishParam.Name = "BillNumber";
            isEnglishParam.Values = new List<string>() { Convert.ToString(bill.Id) };

            parameters.Add(isEnglishParam);
            parameters.Add(clientNameParam);
            parameters.Add(subTotalExpenseParam);
            parameters.Add(taxesParam);
            parameters.Add(totalParam);
            parameters.Add(billDateParam);
            parameters.Add(expensesParam);
            parameters.Add(paymentsParams);
            parameters.Add(numberParam);

            writer.SetParameters(parameters);

            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{""}deMes{""}YAño{""}.pdf";
            return fileStreamResult;
        }
        [Authorize(Policy = "CanBill")] 
        public JsonResult GetAllBillableByClient(int clientId)
        {
            var activities = _activityRepo.Activities
                .Where(a => a.ClientId == clientId && a.IsBillable && a.ActivityType != ActivityTypeEnum.NoBillable)
                .OrderBy(a => a.RealizationDate)
                .Select(a => new
                {
                    Date = a.RealizationDate.ToString("dd/MM/yyyy"),
                    Description = a.Description,
                    Alias = a.Creator.Attorney.Alias,
                    Hours = a.HoursWorked,
                    Price = a.Subtotal,
                    TypeId = (int)a.ActivityType,
                    AssociatedTo = (a.ActivityType == ActivityTypeEnum.Hourly ? "Hora" :
                        a.ActivityType == ActivityTypeEnum.Item ? "Ítem" :
                        a.ActivityType == ActivityTypeEnum.Package ? "Paquete - " + a.Package.Name :
                        "Retainer - " + a.BillableRetainer.Name
                        )
                }).ToList();
            var packagesIds = _activityRepo.Activities
                .Where(a => a.ClientId == clientId && a.IsBillable && a.ActivityType != ActivityTypeEnum.NoBillable && a.ActivityType == ActivityTypeEnum.Package)
                .Select(a => a.PackageId).Distinct().ToList();
            var packages = _packageRepo.Packages.Where(p => packagesIds.Contains(p.Id)).Select(p => new { label = $"{p.Name} (${p.Amount})"}).ToList();
            var retainers = _billableRetainerRepo.BillableRetainers
                .Where(r => r.ClientId == clientId && r.IsBillable)
                .Select(r => new { label = $"{r.Name} (${r.AgreedFee})" })
                .ToList();
            return Json(new { details = activities, packages = packages, retainers = retainers });
        }
        [Authorize(Policy = "CanBill")]
        public IActionResult Billing()
        {
            NewPreBillViewModel viewModel = new NewPreBillViewModel();
            //viewModel.Items = _itemRepo.Items.ToList();
            viewModel.Items = new List<Item>();
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
                TypeId = Convert.ToInt32(d.BillDetailType),
                Cost = d.UnitCost,
                Hours = d.Time,
                Quantity =Convert.ToInt32(d.Quantity),
                Subtotal = d.Subtotal,
                Description = d.Description,
                Date = d.Date.ToString("dd/MM/yyyy"),
                Alias = d.Attorney
            }).ToList();
            //Generate summary
            var summary = generatedBill.BillSummaries.Select(d => new
            {
                TypeId = Convert.ToInt32(d.TypeId),
                Date = d.Date.ToString("dd/MM/yyyy"),
                Description = d.Description,
                Time = d.Time,
                Quantity = d.Quantity,
                Total = d.Total
            }).ToList();
            //Generate expenses
            var expenses = generatedBill.BillExpenses.Select(d => new
            {
                Date = d.Date.ToString("dd/MM/yyyy"),
                Description = d.Description,
                Cost = d.Cost,
                Quantity = d.Quantity,
                Subtotal = d.Subtotal,
                Month = d.Month,
                Year = d.Year,
                SpanishMonth = d.SpanishMonth,
                EnglishMonth = d.EnglishMonth
                
            }).ToList();
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Bill2.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = details });
            dataSources.Add(new ReportDataSource { Name = "DSSummary", Value = summary });
            dataSources.Add(new ReportDataSource { Name = "DSExpenses", Value = expenses });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();

            //ClientName
            ReportParameter clientNameParam = new ReportParameter();
            clientNameParam.Name = "ClientName";
            clientNameParam.Values = new List<string>() { generatedBill.Client.Name};
            //Subtotal
            ReportParameter subTotalExpenseParam = new ReportParameter();
            subTotalExpenseParam.Name = "SubtotalFee";
            subTotalExpenseParam.Values = new List<string>() { generatedBill.BillSubtotal.ToString() };

            //Taxes
            ReportParameter taxesParam = new ReportParameter();
            taxesParam.Name = "Tax";
            taxesParam.Values = new List<string>() { generatedBill.Taxes.ToString() };
            //TotalAmount
            ReportParameter totalParam = new ReportParameter();
            totalParam.Name = "TotalAmount";
            totalParam.Values = new List<string>() { generatedBill.Total.ToString() };
            //BillDate
            ReportParameter billDateParam = new ReportParameter();
            billDateParam.Name = "BillDate";
            billDateParam.Values = new List<string>() { generatedBill.BillDate.ToString("dd/MM/yyyy") };
            //TotalExpense
            ReportParameter expensesParam = new ReportParameter();
            expensesParam.Name = "TotalExpenses";
            expensesParam.Values = new List<string>() { generatedBill.TotalExpenses.ToString() };
            //TotalPayments
            ReportParameter paymentsParams = new ReportParameter();
            paymentsParams.Name = "TotalPayments";
            paymentsParams.Values = new List<string>() { generatedBill.TotalPayments.ToString() };
            //IsEnglish
            ReportParameter isEnglishParam = new ReportParameter();
            isEnglishParam.Name = "IsEnglish";
            isEnglishParam.Values = new List<string>() { isBillingInEnglish ? "1" : "0" };
            //IsEnglish
            ReportParameter numberParam = new ReportParameter();
            isEnglishParam.Name = "BillNumber";
            isEnglishParam.Values = new List<string>() { Convert.ToString(generatedBill.Id) };

            parameters.Add(isEnglishParam);
            parameters.Add(clientNameParam);
            parameters.Add(subTotalExpenseParam);
            parameters.Add(taxesParam);
            parameters.Add(totalParam);
            parameters.Add(billDateParam);
            parameters.Add(expensesParam);
            parameters.Add(paymentsParams);
            parameters.Add(numberParam);

            writer.SetParameters(parameters);

            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{generatedBill.Client.Name}Fecha{generatedBill.BillDate.ToString("dd/MM/yyyy")}.pdf";
            return Json(new { result = memoryStream.ConvertToBase64(), name = fileStreamResult.FileDownloadName });
        }
        [Authorize(Policy = "CanBill")]
        public IActionResult History(string filter, int pageNumber = 1)
        {
            ViewBag.Disabled = TempData["disabled"];
            TempData["filter"] = filter;
            Func<BillHeader, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Client.Name.Contains(filter);
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            BillHistoryViewModel viewModel = new BillHistoryViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.BillHeaders = _billRepo
                .BillHeaders
                .Include(b => b.Client)
                .Include(b => b.Creator)
                .ThenInclude(u => u.Attorney)
                .Where(filterFunction)
                .Where(b => b.Active)
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

        [Authorize(Policy = "CanBill")]
        public IActionResult Disable(int billId)
        {
            _billRepo.DisableBill(billId);
            TempData["disabled"] = true;
            return RedirectToAction(nameof(History), new { filter = TempData["filter"] });
        }
    }
}
