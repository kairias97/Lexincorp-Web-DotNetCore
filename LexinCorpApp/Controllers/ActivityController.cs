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
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IItemRepository _itemsRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IPackageRepository _packageRepo;
        private readonly IRetainerRepository _retainerRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IActivityRepository _activityRepo;
        public int PageSize = 5;
        public ActivityController(IItemRepository _itemsRepo, IExpenseRepository _expenseRepo, ICategoryRepository _categoryRepo,
            IServiceRepository _serviceRepo, IPackageRepository _packageRepo, IRetainerRepository _retainerRepo, IClientRepository _clientRepo,
            IActivityRepository _activityRepo)
        {
            this._itemsRepo = _itemsRepo;
            this._expenseRepo = _expenseRepo;
            this._categoryRepo = _categoryRepo;
            this._serviceRepo = _serviceRepo;
            this._packageRepo = _packageRepo;
            this._retainerRepo = _retainerRepo;
            this._clientRepo = _clientRepo;
            this._activityRepo = _activityRepo;
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
                //Packages = _packageRepo.Packages,
                Retainers = _retainerRepo.Retainers,
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public JsonResult New(NewActivityRequest body)
        {
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            _activityRepo.Save(body, Convert.ToInt32(id));
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
    }
}
