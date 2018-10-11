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
        public int PageSize = 5;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public ActivityController(IItemRepository _itemsRepo, IExpenseRepository _expenseRepo, ICategoryRepository _categoryRepo,
            IServiceRepository _serviceRepo, IPackageRepository _packageRepo, IRetainerRepository _retainerRepo, IClientRepository _clientRepo,
            IActivityRepository _activityRepo, IAttorneyRepository attorneyRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            IAttorneyRepository _attorneysRepo, INotificationRepository notificationRepository)
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
                Items = _itemsRepo.Items,
                Expenses = _expenseRepo.Expenses,
                Categories = _categoryRepo.Categories,
                Services = _serviceRepo.Services,
                Attorneys = _attorneysRepo.Attorneys
                .Where(a => a.User.Active)
                .Select(a => new AttorneySelection{
                    UserId = a.UserId,
                    AttorneyName = a.Name
                }).OrderBy(a => a.AttorneyName),
                //Packages = _packageRepo.Packages,
                Retainers = _retainerRepo.Retainers
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
            if(User.IsInRole("Administrador"))
            {
                id = body.UserId ?? Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            } else
            {
                id = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            }
            if(packageClosed && body.PackageId != null)
            {
                var u = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                _notificationRepo.RequestPackageClosure(body.PackageId ?? 0, u, out wasClosed);
            }
            
            _activityRepo.Save(body, id);
            return Json(new { message = "Actividad ingresada exitosamente", success = true });
        }
        [Authorize]
        public IActionResult History(string filter, int pageNumber = 1)
        {
            Func<Activity, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Client.Name.Contains(filter);
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            ActivityHistoryViewModel viewModel = new ActivityHistoryViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.Activities = _activityRepo.Activities
                .Where(a => a.CreatorId == Convert.ToInt32(id))
                .Where(filterFunction)
                .OrderByDescending(a => a.RealizationDate)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _activityRepo.Activities.Where(a => a.CreatorId == Convert.ToInt32(id)).Count(filterFunction)
            };
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
            var list = new List<Activity>();
            list = _activityRepo.Activities.Include(a => a.Service).ThenInclude(s => s.Category)
                .Include(a => a.Client)
                .Where(a => a.RealizationDate >= newActivityReport.InitialDate && a.RealizationDate <= newActivityReport.FinalDate
                    && (newActivityReport.UserId == null || a.CreatorId == newActivityReport.UserId)
                    && (newActivityReport.ActivityType == null || a.ActivityType == newActivityReport.ActivityType)
                ).ToList();
            var actividades = list.Select(a => new
            {
                activityClient = a.Client.Name,
                activityService = a.Service.Name,
                activityCategory = a.Service.Category.Name,
                activityHoursWorked = Math.Round(a.HoursWorked, 2),
                activityAssociatedTo = a.ActivityType == ActivityTypeEnum.Hourly ? "Horario" : a.ActivityType == ActivityTypeEnum.Item ?
                $"Ítem - {a.Item.Name}" : a.ActivityType == ActivityTypeEnum.Package ? $"Paquete - {a.Package.Name}" : a.ActivityType == ActivityTypeEnum.Retainer ?
                $"Retainer - {a.BillableRetainer.Name}" : "",
                activityDate = a.RealizationDate
            }).ToList();
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Activities.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "ActivitiesDataSet", Value = actividades });

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"ReporteDeActividadesDel{newActivityReport.InitialDate.ToString("ddMMyyyy")}Al{newActivityReport.FinalDate.ToString("ddMMyyyy")}.pdf";
            return fileStreamResult;
        }
    }
}
