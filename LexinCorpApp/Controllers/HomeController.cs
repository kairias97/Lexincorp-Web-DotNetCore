using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IActivityRepository _activityRepo;
        private readonly IAttorneyRepository _attorneyRepo;
        public HomeController(IActivityRepository _activityRepo, IAttorneyRepository _attorneyRepo)
        {
            this._activityRepo = _activityRepo;
            this._attorneyRepo = _attorneyRepo;
        }
        [Authorize]
        public IActionResult Index()
        {
            DashboardViewModel viewModel = new DashboardViewModel();
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var list = _activityRepo.Activities.OrderByDescending(a => a.RealizationDate).Where(a => a.CreatorId == Convert.ToInt32(id)).Take(5).ToList();
            var vacations = _attorneyRepo.Attorneys.Where(a => a.UserId == Convert.ToInt32(id)).FirstOrDefault().VacationCount;
            var listHours = _activityRepo.Activities.Where(a => a.RealizationDate.Year == DateTime.Now.Year && a.RealizationDate.Month == DateTime.Now.Month && a.CreatorId == Convert.ToInt32(id)).ToList();
            var hoursWorked = listHours.Sum(h => h.HoursWorked);
            viewModel.Vacations = Math.Round(vacations,2);
            viewModel.Activities = list;
            viewModel.HoursWorked = Math.Round(hoursWorked);
            return View(viewModel);
        }
    }
}
