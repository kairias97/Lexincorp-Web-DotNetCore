using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Models.ViewModels;
using LexincorpApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models.ExternalServices;
using System.Security.Claims;
using Syncfusion.Report;
using Syncfusion.ReportWriter;
using System.IO;
using System.Globalization;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador, Regular")]
    public class ActivityController : Controller
    {
        private readonly IItemRepository _itemsRepo;
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IPackageRepository _packageRepo;
        private readonly IRetainerRepository _retainerRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IActivityRepository _activityRepo;
        private readonly IAttorneyRepository _attorneyRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IBillableRetainerRepository _billableRetainerRepo;
        public int PageSize = 5;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public ActivityController(IItemRepository _itemsRepo, IExpenseRepository _expenseRepo, ICategoryRepository _categoryRepo,
            IServiceRepository _serviceRepo, IPackageRepository _packageRepo, IRetainerRepository _retainerRepo, IClientRepository _clientRepo,
            IActivityRepository _activityRepo, IAttorneyRepository attorneyRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            IAttorneyRepository _attorneysRepo, INotificationRepository notificationRepository, IBillableRetainerRepository billableRetainerRepository)
        {
            this._itemsRepo = _itemsRepo;
            this._attorneysRepo = _attorneysRepo;
            this._expenseRepo = _expenseRepo;
            this._categoryRepo = _categoryRepo;
            this._serviceRepo = _serviceRepo;
            this._packageRepo = _packageRepo;
            this._retainerRepo = _retainerRepo;
            this._clientRepo = _clientRepo;
            this._activityRepo = _activityRepo;
            this._attorneyRepo = attorneyRepository;
            _hostingEnvironment = hostingEnvironment;
            this._notificationRepo = notificationRepository;
            this._billableRetainerRepo = billableRetainerRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Register()
        {
            RegisterActivityViewModel viewModel = new RegisterActivityViewModel
            {
                Clients = _clientRepo.Clients,
                Expenses = _expenseRepo.Expenses,
                Categories = _categoryRepo.Categories,
                Services = _serviceRepo.Services,
                Attorneys = _attorneysRepo.Attorneys
                .Where(a => a.User.Active)
                .Select(a => new AttorneySelection
                {
                    UserId = a.UserId,
                    AttorneyName = a.Name
                }).OrderBy(a => a.AttorneyName),
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public JsonResult New(NewActivityRequest body, bool packageClosed)
        {
            var user = HttpContext.User;
            int id;
            bool wasClosed;
            if (User.IsInRole("Administrador"))
            {
                id = body.UserId ?? Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            else
            {
                id = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            if (packageClosed && body.PackageId != null)
            {
                var u = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _notificationRepo.RequestPackageClosure(body.PackageId ?? 0, u, out wasClosed);
            }

            _activityRepo.Save(body, id);
            return Json(new { message = "Actividad ingresada exitosamente", success = true });
        }
        [Authorize]
        public IActionResult History(string filter, string dateStart, string dateEnd, int pageNumber = 1)
        {
            Func<Activity, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Client.Name.Contains(filter);
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime date1 = new DateTime();
            DateTime date2 = new DateTime();
            if (dateStart != null && dateStart != "")
            {
               date1 = DateTime.ParseExact(dateStart, "dd/MM/yyyy", provider);
            }
            if(dateEnd != null && dateEnd != "")
            {
                date2 = DateTime.ParseExact(dateEnd, "dd/MM/yyyy", provider);
            }
            Func<Activity, bool> filterFunctionDate = a => a.RealizationDate >= date1 && a.RealizationDate <= date2;
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            ActivityHistoryViewModel viewModel = new ActivityHistoryViewModel();
            viewModel.CurrentFilter = filter;
            if(!String.IsNullOrEmpty(dateStart) && !String.IsNullOrEmpty(dateEnd))
            {
                viewModel.Activities = _activityRepo.Activities
                .Where(a => a.CreatorId == Convert.ToInt32(id))
                .Where(filterFunction)
                .Where(filterFunctionDate)
                .OrderByDescending(a => a.RealizationDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
                viewModel.CurrentStartDate = date1.ToString("dd/MM/yyyy");
                viewModel.CurrentEndDate = date2.ToString("dd/MM/yyyy");
            }
            else
            {
                viewModel.Activities = _activityRepo.Activities
                .Where(a => a.CreatorId == Convert.ToInt32(id))
                .Where(filterFunction)
                .OrderByDescending(a => a.RealizationDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            }
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _activityRepo.Activities.Where(a => a.CreatorId == Convert.ToInt32(id)).Count(filterFunction)
            };
            viewModel.Expenses = _expenseRepo.Expenses;
            return View(viewModel);
        }
        [Authorize(Policy = "CanReviewBillDetail")]
        public IActionResult DetailCheck(string dateStart, string dateEnd, int id, bool ByRange = false, bool checkItem = false, bool checkHours = false, bool checkAll = false)
        {
            CultureInfo esNI = new CultureInfo("es-NI");
            Func<Activity, bool> clientFilter = c =>  c.ClientId == id;
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime date1 = new DateTime();
            DateTime date2 = new DateTime();
            if (dateStart != null && dateStart != "")
            {
                var parsedDate1 = DateTime.TryParseExact(dateStart,"dd/MM/yyyy", esNI, DateTimeStyles.None, out date1);
                if (!parsedDate1)
                {
                    dateStart = "";
                }
            }
            if (dateEnd != null && dateEnd != "")
            {
                var parsedDate2 = DateTime.TryParseExact(dateEnd, "dd/MM/yyyy", esNI, DateTimeStyles.None, out date2);
                if (!parsedDate2)
                {
                    dateEnd = "";
                }
            }
            Func<Activity, bool> filterFunctionDate = a => a.RealizationDate >= date1 && a.RealizationDate <= date2;
            ActivityDetailCheckViewModel viewModel = new ActivityDetailCheckViewModel();
            var targetClient = _clientRepo.Clients.Where(c => c.Id == id).FirstOrDefault();
            viewModel.CurrentFilter = targetClient?.Name ?? "";
            viewModel.CurrentId = id;
            viewModel.ByRange = ByRange;
            viewModel.checkAll = checkAll;
            viewModel.checkHours = checkHours;
            viewModel.checkItem = checkItem;
            if (!String.IsNullOrEmpty(viewModel.CurrentFilter))
            {
                if (!String.IsNullOrEmpty(dateStart) && !String.IsNullOrEmpty(dateEnd))
                {
                    List<Activity> activities = _activityRepo.Activities
                    .Include(a=> a.Creator)
                    .ThenInclude(u => u.Attorney)
                    .Where(a => 
                        (a.IsBillable == false && a.IsBilled == false)
                        && 
                        ((a.ActivityType == ActivityTypeEnum.Package && a.Package.IsFinished) 
                            || (a.ActivityType != ActivityTypeEnum.Package && a.ActivityType != ActivityTypeEnum.NoBillable))
                            
                        && clientFilter(a)
                        && filterFunctionDate(a)
                        
                        )
                    .OrderBy(a => a.RealizationDate).ToList();
                    List<Package> packages = new List<Package>();
                    List<BillableRetainer> billableRetainers = new List<BillableRetainer>();
                    foreach(var a in activities)
                    {
                        if(a.PackageId != null)
                        {
                            var package = _packageRepo.Packages.Where(p => p.Id == a.PackageId).FirstOrDefault();
                            bool contains = packages.Any(pa => pa.Id == package.Id);
                            if(contains == false)
                            {
                                packages.Add(package);
                            }
                        }
                        else if(a.BillableRetainerId != null)
                        {
                            var retainer = _billableRetainerRepo.BillableRetainers.Where(b => b.Id == a.BillableRetainerId).FirstOrDefault();
                            bool contains = billableRetainers.Any(bi => bi.Id == retainer.Id);
                            if(contains == false)
                            {
                                billableRetainers.Add(retainer);
                            }
                        }
                        
                    }
                    viewModel.Activities = activities;
                    viewModel.CurrentStartDate = date1.ToString("dd/MM/yyyy");
                    viewModel.CurrentEndDate = date2.ToString("dd/MM/yyyy");
                    viewModel.Packages = packages;
                    viewModel.BillableRetainers = billableRetainers;
                }
                else
                {
                    List<Activity> activities = _activityRepo.Activities
                    .Where(a =>
                        (a.IsBillable == false && a.IsBilled == false)
                        &&
                        ((a.ActivityType == ActivityTypeEnum.Package && a.Package.IsFinished)
                            || (a.ActivityType != ActivityTypeEnum.Package && a.ActivityType != ActivityTypeEnum.NoBillable))

                        && clientFilter(a)

                        )
                    .OrderBy(a => a.RealizationDate).ToList();
                    List<Package> packages = new List<Package>();
                    List<BillableRetainer> billableRetainers = new List<BillableRetainer>();
                    foreach (var a in activities)
                    {
                        if (a.PackageId != null)
                        {
                            var package = _packageRepo.Packages.Where(p => p.Id == a.PackageId).FirstOrDefault();
                            bool contains = packages.Any(pa => pa.Id == package.Id);
                            if (contains == false)
                            {
                                packages.Add(package);
                            }
                        }
                        else if (a.BillableRetainerId != null)
                        {
                            var retainer = _billableRetainerRepo.BillableRetainers.Where(b => b.Id == a.BillableRetainerId).FirstOrDefault();
                            bool contains = billableRetainers.Any(bi => bi.Id == retainer.Id);
                            if (contains == false)
                            {
                                billableRetainers.Add(retainer);
                            }
                        }

                    }
                    viewModel.Activities = activities;
                    viewModel.Packages = packages;
                    viewModel.BillableRetainers = billableRetainers;
                }
            }
            else
            {
                viewModel.Activities = new List<Activity>();
                viewModel.Packages = new List<Package>();
                viewModel.BillableRetainers = new List<BillableRetainer>();
            }
            viewModel.Expenses = _expenseRepo.Expenses;
            viewModel.CurrentId = id;
            return View(viewModel);
        }
        [Authorize]
        public IActionResult Report()
        {
            ActivityReportViewModel viewModel = new ActivityReportViewModel();
            viewModel.Attorneys = _attorneyRepo.Attorneys;
            viewModel.NewActivityReport = new NewActivityReport();
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Report(NewActivityReport newActivityReport)
        {
            Func<Activity, decimal, decimal> GetFeeByActivity = (activity, ratePerHour) =>
            {
                switch (activity.ActivityType)
                {
                    case ActivityTypeEnum.Hourly:
                        return activity.Subtotal;
                    case ActivityTypeEnum.Item:
                        return activity.Subtotal;
                    case ActivityTypeEnum.NoBillable:
                        return (decimal)(0.00);
                    case ActivityTypeEnum.Package:
                        if (activity.Package.IsFinished)
                        {
                            return ratePerHour * activity.HoursWorked;
                        }
                        else
                        {
                            return (decimal)(0.00);
                        }
                    case ActivityTypeEnum.Retainer:
                        //Under working the hours
                        if (activity.BillableQuantity == 0)
                        {
                            return ratePerHour * activity.HoursWorked;
                        }
                        //Is normal billed as excedent of the agreed hours
                        else if (activity.BillableQuantity == activity.HoursWorked)
                        {
                            return activity.Subtotal;
                        }
                        else
                        {
                            return activity.Subtotal + ((activity.HoursWorked - activity.BillableQuantity) * activity.BillableRate);
                        }
                    default:
                        return (decimal)(0.00);

                }
            };
            var list = new List<Activity>();
            list = _activityRepo.Activities.Include(a => a.Service).ThenInclude(s => s.Category)
                .Include(a => a.Client)
                .Include(a => a.Package)
                .Include(a => a.BillableRetainer)
                .Where(a => a.RealizationDate >= newActivityReport.InitialDate && a.RealizationDate <= newActivityReport.FinalDate
                    && (newActivityReport.UserId == null || a.CreatorId == newActivityReport.UserId)
                    && (newActivityReport.ActivityType == null || a.ActivityType == newActivityReport.ActivityType)
                ).ToList();

            //To get all the packages amount involved
            var involvedPackagesRates = list.Where(a => a.ActivityType == ActivityTypeEnum.Package
            && a.Package.IsFinished).Select(a => a.Package)
                .Distinct()
                .Select(package =>
                    new { package.Id, rate = (package.Amount / (_activityRepo.Activities.Where(act => act.PackageId == package.Id).Sum(act => act.HoursWorked))) })
                    .ToList();

            var activities = list.Select(a => new
            {
                activityClient = a.Client.Name,
                activityService = a.Service.Name,
                activityCategory = a.Service.Category.Name,
                activityHoursWorked = Math.Round(a.HoursWorked, 2),
                activityAssociatedTo = a.ActivityType == ActivityTypeEnum.Hourly ? "Horario" : a.ActivityType == ActivityTypeEnum.Item ?
                $"Ítem" : a.ActivityType == ActivityTypeEnum.Package ? $"Paquete - {a.Package?.Name}" : a.ActivityType == ActivityTypeEnum.Retainer ?
                $"Retainer - {a?.BillableRetainer?.Name}" : a.ActivityType == ActivityTypeEnum.NoBillable ?"No cobrable" :"",
                activityDate = a.RealizationDate,
                activityTotalFee = GetFeeByActivity(a, a.ActivityType == ActivityTypeEnum.Package ?
                    (involvedPackagesRates.Where(ip => ip.Id == a.PackageId).FirstOrDefault()?.rate ?? (decimal)0.00)
                    : a.ActivityType == ActivityTypeEnum.Retainer ?
                    a.BillableRetainer.AgreedFee / a.BillableRetainer.AgreedHours
                    : (decimal)0.00)
            }).ToList();

            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Activities.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "ActivitiesDataSet", Value = activities });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            writer.ReportProcessingMode = ProcessingMode.Local;
            //Setting up the parameters
            List<ReportParameter> parameters = new List<ReportParameter>();
            //StartDate
            ReportParameter startDateParam = new ReportParameter();
            startDateParam.Name = "StartDate";
            startDateParam.Values = new List<string>() { newActivityReport.InitialDate.ToString("dd/MM/yyyy") };
            //EndDate
            ReportParameter endDateParam = new ReportParameter();
            endDateParam.Name = "EndDate";
            endDateParam.Values = new List<string>() { newActivityReport.FinalDate.ToString("dd/MM/yyyy") };
            //AttorneyName
            ReportParameter attorneyParam = new ReportParameter();
            attorneyParam.Name = "AttorneyName";
            attorneyParam.Values = new List<string>() { newActivityReport.UserId ==null? "Todos"
                : _attorneyRepo.Attorneys.Where(a => a.UserId == ((int)newActivityReport.UserId)).Select(a => a.Name).First() };
            //ActivityType
            ReportParameter activityTypeParam = new ReportParameter();
            activityTypeParam.Name = "ActivityType";
            activityTypeParam.Values = new List<string>() { newActivityReport.ActivityType == null ? "0" : ((int)newActivityReport.ActivityType).ToString() };
            //TotalHours
            ReportParameter totalHoursParam = new ReportParameter();
            totalHoursParam.Name = "TotalHours";
            totalHoursParam.Values = new List<string>() { activities.Sum(a => a.activityHoursWorked).ToString() };
            //Total Amount
            ReportParameter totalAmountParam = new ReportParameter();
            totalAmountParam.Name = "TotalAmount";
            totalAmountParam.Values = new List<string>() { activities.Sum(a => a.activityTotalFee).ToString() };


            parameters.Add(startDateParam);
            parameters.Add(endDateParam);
            parameters.Add(attorneyParam);
            parameters.Add(activityTypeParam);
            parameters.Add(totalHoursParam);
            parameters.Add(totalAmountParam);
            writer.SetParameters(parameters);
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"ReporteDeActividadesDel{newActivityReport.InitialDate.ToString("ddMMyyyy")}Al{newActivityReport.FinalDate.ToString("ddMMyyyy")}.pdf";
            return fileStreamResult;
        }
        [Authorize]
        public JsonResult GetActivitiesByClient(int id, int attorneyId)
        {
            //To get the current identity in case it would be an admin
            if (!User.IsInRole("Administrador"))
            {
                attorneyId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            var results = _activityRepo.Activities.Where(a => a.ClientId == id && a.CreatorId == attorneyId).OrderByDescending(a => a.RealizationDate).Take(10)
                .Select(a => new
                {
                    description = a.Description,
                    serviceName = a.Service.Name,
                    hoursWorked = Math.Round(a.HoursWorked, 2),
                    activityType = a.ActivityType,
                    packageName = a.Package != null ? a.Package.Name : "-",
                    billableRatainerName = a.BillableRetainer != null ? a.BillableRetainer.Name : "-",
                    itemName = a.Item != null ? a.Item.Name : "-",
                    date = a.RealizationDate.ToString("dd/MM/yyyy")
                });            
            return Json(results);
        }
        [Authorize]
        public JsonResult GetActivityDataById(int id, bool? finishedPackage)
        {
            var result = _activityRepo.Activities.Where(a => a.Id == id).FirstOrDefault();
            var packages = _packageRepo
                .Packages
                .Include(p => p.Client)
                .Where(p => p.ClientId == result.ClientId 
                    && (finishedPackage == null || p.IsFinished == (bool)finishedPackage) && p.IsBilled == false)
                .OrderBy(p => p.Name).ToList();
            var retainers = _billableRetainerRepo.BillableRetainers
                .Where(b => b.ClientId == result.ClientId 
                    && b.IsVisibleForActivities && b.Year 
                    == result.RealizationDate.Year 
                    && b.Month == result.RealizationDate.Month)
                .ToList();
            var activity = new
            {
                id = result.Id,
                date = result.RealizationDate.ToString("dd/MM/yyyy"),
                type = result.ActivityType,
                package = result.PackageId != null ? result.Package.Name : "",
                retainer = result.BillableRetainerId != null ? result.BillableRetainer.Name : "",
                description = result.Description,
                hoursWorked = result.HoursWorked,
                feePerHour = result.Client.FixedCostPerHour,
                clientName = result.Client.Name,
                isHourBilled = result.Client.IsHourBilled ? "hora" : "",
                service = result.Service.Name,
                quantity = result.BillableQuantity,
                rate = result.BillableRate,
                packageId = result.PackageId,
                billableRetainerId = result.BillableRetainerId,
                expenses = result.ActivityExpenses.Select(e => new
                {
                    id = e.ExpenseId,
                    spanishDescription = e.Expense.SpanishDescription,
                    englishDescription = e.Expense.EnglishDescription,
                    quantity = e.Quantity,
                    unitAmount = e.UnitAmount,
                    totalAmount = e.TotalAmount,
                }),
                packages = packages.Select(p => new
                {
                    name = p.Name,
                    id = p.Id
                }),
               retainers = retainers.Select(b => new
               {
                   id = b.Id,
                   name = b.Name
               }),
            };
            return Json(activity);
        }
        [Authorize]
        [HttpPost]
        public JsonResult Update(UpdateActivityRequest body)
        {
            _activityRepo.Update(body);
            return Json(new { message = "Actividad actualizada exitosamente", success = true });
        }
        [Authorize]
        [HttpPost]
        public JsonResult MarkActivities(List<int> body)
        {
            _activityRepo.MarkActivitiesAsBillable(body);
            return Json(new { message = "Actividades marcadas como facturables exitosamente", success = true });
        }
    }
}
