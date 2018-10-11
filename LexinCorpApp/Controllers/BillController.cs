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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class BillController : Controller
    {
        private readonly IItemRepository _itemRepo;
        private readonly IActivityRepository _activityRepo;
        private readonly IPackageRepository _packageRepo;
        private readonly IBillableRetainerRepository _billableRetainerRepo;
        private readonly IClientRepository _clientRepo;
        private IConfiguration configuration;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IBillHeaderRepository _billRepo;
        public int PageSize = 5;
        public BillController(IItemRepository itemRepository, IActivityRepository activityRepository, IPackageRepository packageRepository, IBillableRetainerRepository billableRetainerRepository,
            IClientRepository clientRepository, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IBillHeaderRepository billHeaderRepository)
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
        [Authorize]
        public IActionResult PreBilling()
        {
            NewPreBillViewModel viewModel = new NewPreBillViewModel();
            viewModel.Items = _itemRepo.Items.ToList();
            return View(viewModel);
        }
        [Authorize]
        public JsonResult GenerateBill(BillHeader billHeader, BillRequest billRequest)
        {
            billHeader.BillDetails = new List<BillDetail>();
            var details = new List<BillDetail>();
            var client = _clientRepo.Clients.Where(c => c.Id == billHeader.ClientId).FirstOrDefault();
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            if (billRequest.Hours ?? false)
            {
                List<BillDetail> result = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Hourly && a.ClientId == billHeader.ClientId && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                    .Select(a => new BillDetail {
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
            if(billRequest.PackageId != null)
            {
                    var packages = _packageRepo.Packages.Where(p => (billRequest.PackageId == -1 || p.Id == billRequest.PackageId) && p.IsFinished == true && p.IsBilled == false && p.ClientId == billHeader.ClientId).ToList();
                    var result = new List<BillDetail>();
                    foreach(var p in packages)
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
                        var activities = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Package && a.ClientId == billHeader.ClientId && a.PackageId == p.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                            .Select(a => new BillDetail
                            {
                                ActivityId = a.Id,
                                BillDetailType = BillDetailTypeEnum.PackageDetail,
                                FixedAmount = p.Amount,
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
            if(billRequest.BillableRetainerId != null)
            {
                var retainers = _billableRetainerRepo.BillableRetainers.Where(r => (billRequest.BillableRetainerId == -1 || r.Id == billRequest.BillableRetainerId) && r.IsBilled == false && r.ClientId == billHeader.ClientId).ToList();
                var result = new List<BillDetail>();
                foreach(var r in retainers)
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
                    var activities = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Retainer && a.ClientId == billHeader.ClientId && a.BillableRetainerId == r.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.RetainerDetail,
                            FixedAmount = r.AgreedFee,
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
            if(billRequest.ItemId != null)
            {
                List<BillDetail> result = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Item && a.ClientId == billHeader.ClientId && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
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
            billHeader.BillDetails = details;
            billHeader.BillSubtotal = billHeader.BillDetails.Sum(d => d.Subtotal);
            billHeader.Taxes = billHeader.BillDetails.Sum(d => d.TaxesAmount);
            var discount = billHeader.BillDiscountType == null ? 0 : billHeader.BillDiscountType == BillDiscountEnum.Net ? billHeader.BillDiscount ?? 0 : billHeader.BillDiscountType == BillDiscountEnum.Percentage ? billHeader.BillDiscount ?? 0 / 100 : 0;
            billHeader.Total = billHeader.BillSubtotal - discount + billHeader.Taxes;
            //RenderBill(billHeader);
            var detalles = billHeader.BillDetails.Select(d => new
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
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = detalles });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{billHeader.BillName}deMes{billHeader.BillMonth}YAño{billHeader.BillYear}.pdf";
            return Json(new { result = memoryStream.ConvertToBase64() });
            //return Json(new { message = "Factura ingresada exitosamente", success = true });
        }
        [Authorize]
        public IActionResult RenderBill(int id)
        {
            var billHeader = _billRepo.BillHeaders.Where(b => b.Id == id).Include(b => b.BillDetails).FirstOrDefault();
            var detalles = billHeader.BillDetails.Select(d => new
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
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = detalles });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{billHeader.BillName}deMes{billHeader.BillMonth}YAño{billHeader.BillYear}.pdf";
            return fileStreamResult;
        }
        [Authorize]
        public IActionResult Billing()
        {
            NewPreBillViewModel viewModel = new NewPreBillViewModel();
            viewModel.Items = _itemRepo.Items.ToList();
            return View(viewModel);
        }
        [Authorize]
        public JsonResult GenerateAndSaveBill(BillHeader billHeader, BillRequest billRequest)
        {
            billHeader.BillDetails = new List<BillDetail>();
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            billHeader.CreatorId = Convert.ToInt32(id);
            List<Package> pack = new List<Package>();
            List<BillableRetainer> ret = new List<BillableRetainer>();
            var details = new List<BillDetail>();
            var client = _clientRepo.Clients.Where(c => c.Id == billHeader.ClientId).FirstOrDefault();
            decimal ivaValue = Convert.ToDecimal(configuration["LexincorpAdmin:IvaPercentage"]);
            if (billRequest.Hours ?? false)
            {
                List<BillDetail> result = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Hourly && a.ClientId == billHeader.ClientId && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
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
            if (billRequest.PackageId != null)
            {
                var packages = _packageRepo.Packages.Where(p => (billRequest.PackageId == -1 || p.Id == billRequest.PackageId) && p.IsFinished == true && p.IsBilled == false && p.ClientId == billHeader.ClientId).ToList();
                var result = new List<BillDetail>();
                foreach (var p in packages)
                {
                    var detail = new BillDetail();
                    pack.Add(p);
                    detail.BillDetailType = BillDetailTypeEnum.PackageHeader;
                    detail.Description = p.Description;
                    detail.FixedAmount = p.Amount;
                    detail.Subtotal = p.Amount;
                    detail.Quantity = 1;
                    detail.TaxesAmount = client.PayTaxes ? p.Amount * ivaValue : 0;
                    detail.UnitRate = 0;
                    result.Add(detail);
                    var activities = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Package && a.ClientId == billHeader.ClientId && a.PackageId == p.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.PackageDetail,
                            FixedAmount = p.Amount,
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
            if (billRequest.BillableRetainerId != null)
            {
                var retainers = _billableRetainerRepo.BillableRetainers.Where(r => (billRequest.BillableRetainerId == -1 || r.Id == billRequest.BillableRetainerId) && r.IsBilled == false && r.ClientId == billHeader.ClientId).ToList();
                var result = new List<BillDetail>();
                foreach (var r in retainers)
                {
                    var detail = new BillDetail();
                    ret.Add(r);
                    detail.BillDetailType = BillDetailTypeEnum.RetainerHeader;
                    detail.Description = r.BillingDescription;
                    detail.FixedAmount = r.AgreedFee;
                    detail.Subtotal = r.AgreedFee;
                    detail.Quantity = 1;
                    detail.TaxesAmount = client.PayTaxes ? r.AgreedFee * ivaValue : 0;
                    detail.UnitRate = 0;
                    result.Add(detail);
                    var activities = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Retainer && a.ClientId == billHeader.ClientId && a.BillableRetainerId == r.Id && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
                        .Select(a => new BillDetail
                        {
                            ActivityId = a.Id,
                            BillDetailType = BillDetailTypeEnum.RetainerDetail,
                            FixedAmount = r.AgreedFee,
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
            if (billRequest.ItemId != null)
            {
                List<BillDetail> result = _activityRepo.Activities.OrderBy(a => a.RealizationDate).Where(a => a.ActivityType == ActivityTypeEnum.Item && a.ClientId == billHeader.ClientId && a.RealizationDate.Year == billHeader.BillYear && a.RealizationDate.Month == billHeader.BillMonth)
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
            billHeader.BillDetails = details;
            billHeader.BillSubtotal = billHeader.BillDetails.Sum(d => d.Subtotal);
            billHeader.Taxes = billHeader.BillDetails.Sum(d => d.TaxesAmount);
            var discount = billHeader.BillDiscountType == null ? 0 : billHeader.BillDiscountType == BillDiscountEnum.Net ? billHeader.BillDiscount ?? 0 : billHeader.BillDiscountType == BillDiscountEnum.Percentage ? billHeader.BillDiscount ?? 0 / 100 : 0;
            billHeader.Total = billHeader.BillSubtotal - discount + billHeader.Taxes;

            _billRepo.Save(billHeader, pack, ret);
            //RenderBill(billHeader);
            var detalles = billHeader.BillDetails.Select(d => new
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
            dataSources.Add(new ReportDataSource { Name = "BillDataSet", Value = detalles });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"FacturaCliente{billHeader.BillName}deMes{billHeader.BillMonth}YAño{billHeader.BillYear}.pdf";
            return Json(new { result = memoryStream.ConvertToBase64() });
            //return Json(new { message = "Factura ingresada exitosamente", success = true });
        }
        [Authorize]
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
