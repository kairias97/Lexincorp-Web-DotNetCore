﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IActivityRepository _activityRepo;
        private readonly IAttorneyRepository _attorneyRepo;
        private readonly IVacationsRequestRepository _vacationsRequestRepository;
        public HomeController(IActivityRepository _activityRepo, IAttorneyRepository _attorneyRepo, IVacationsRequestRepository vacationsRequestRepository)
        {
            this._activityRepo = _activityRepo;
            this._attorneyRepo = _attorneyRepo;
            this._vacationsRequestRepository = vacationsRequestRepository;
        }
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            string user = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.Identity.Name : "Anonymous";
            string path = "~"+exceptionHandlerPathFeature.Path;
            Log.Error(exceptionHandlerPathFeature.Error, "Error 500 - {User} - {Path}", user, path);
            ViewBag.returnUrl = path;
            return View();
        }
        [Authorize]
        public IActionResult Index()
        {
            DashboardViewModel viewModel = new DashboardViewModel();
            var user = HttpContext.User;
            var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var list = _activityRepo.Activities.OrderByDescending(a => a.RealizationDate).Where(a => a.CreatorId == Convert.ToInt32(id)).Take(5).ToList();
            var vacations = _attorneyRepo.Attorneys.Where(a => a.UserId == Convert.ToInt32(id)).FirstOrDefault()?.VacationCount ?? 0;
            var listHours = _activityRepo.Activities.Where(a => a.CreatorId == Convert.ToInt32(id) && a.RealizationDate.Year == DateTime.Now.Year && a.RealizationDate.Month == DateTime.Now.Month).ToList();
            var hoursWorked = listHours.Sum(h => h.HoursWorked);
            viewModel.Vacations = Math.Round(vacations,2);
            viewModel.Activities = list;
            viewModel.AvailableVacations = _vacationsRequestRepository.GetAvailableVacationCount(Convert.ToInt32(id));
            viewModel.ReservedVacations = _vacationsRequestRepository.GetReservedVacationCount(Convert.ToInt32(id));
            viewModel.HoursWorked = Math.Round(hoursWorked);
            return View(viewModel);
        }
        [HttpGet]
        public JsonResult GetDashboardChartInfo()
        {
            var attorneys = _attorneyRepo.Attorneys.Where(a => a.User.Active).Select(a => new { a.UserId, a.Name }).ToList();
            var namesArray = attorneys.Select(a => a.Name).ToList();
            var currentDate = DateTime.Now;
            var orderedWorkedHours = new List<decimal>();
            foreach (var a in attorneys)
            {
                var hoursWorked = _activityRepo.Activities.Where(act => act.CreatorId == a.UserId
                && (act.RealizationDate.Day >= 1 || act.RealizationDate.Day <= currentDate.Day)
                && act.RealizationDate.Month == currentDate.Month
                && act.RealizationDate.Year == currentDate.Year).Sum(act => act.HoursWorked);
                orderedWorkedHours.Add(hoursWorked);
            }
            return Json(new { labels= namesArray, data = orderedWorkedHours});
        }
    }
}
