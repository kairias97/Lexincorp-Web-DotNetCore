using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Models.ViewModels;
using LexincorpApp.Models;
using Microsoft.EntityFrameworkCore;
using LexincorpApp.Infrastructure;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    [Authorize(Roles ="Administrador,Regular")]
    public class VacationsRequestController : Controller
    {
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IVacationsRequestRepository _vacationsRequestRepo;
        public int PageSize = 5;
        public VacationsRequestController(IAttorneyRepository _attorneysRepo, IVacationsRequestRepository _vacationsRequestRepo)
        {
            this._attorneysRepo = _attorneysRepo;
            this._vacationsRequestRepo = _vacationsRequestRepo;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult New()
        {
            var user = HttpContext.User;
            var userId = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel
            {
                DaysAvailable = _vacationsRequestRepo.GetAvailableVacationCount(userId),
                VacationsRequest = new VacationsRequest()
            };
            ViewBag.AddedRequest = TempData["added"];
            ViewBag.DaysInvalid = false;
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult New(VacationsRequest vacationsRequest)
        {
            if (!ModelState.IsValid)
            {
                var user = HttpContext.User;
                var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var attorney = _attorneysRepo.Attorneys.Where(x => x.UserId == Convert.ToInt32(id)).FirstOrDefault();
                NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel
                {
                    DaysAvailable = attorney.VacationCount,
                    VacationsRequest = vacationsRequest
                };
                return View(viewModel);
            }
            else
            {
                var user = HttpContext.User;
                var userId = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                
                if (_vacationsRequestRepo.ValidateRequest(userId, vacationsRequest.Quantity))
                {
                    //vacationsRequest.AttorneyId = attorney.AttorneyId;
                    _vacationsRequestRepo.Save(vacationsRequest, userId);
                    ViewBag.DaysInvalid = false;
                    TempData["added"] = true;
                    return RedirectToAction("New");
                }
                else
                {
                    //Refactorizar acá para agregar manualmente el error al model view state
                    ViewBag.DaysInvalid = true;
                    NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel
                    {
                        DaysAvailable = _vacationsRequestRepo.GetAvailableVacationCount(userId),
                        VacationsRequest = vacationsRequest
                    };
                    return View(viewModel);
                }
            }
        }
        [Authorize]
        public IActionResult History(bool? filter, string filterText, int pageNumber = 1)
        {
            var user = HttpContext.User;
            var id = user.Claims.Where(claim => claim.Type == ClaimTypes.NameIdentifier).First().Value;
            var attorney = _attorneysRepo.Attorneys.Where(x => x.UserId == Convert.ToInt32(id)).FirstOrDefault();

            Func<VacationsRequest, bool> filterFunction = c => c.IsApproved == filter;

            Func<VacationsRequest, bool> filterFunctionText = c => String.IsNullOrEmpty(filterText) || c.Reason.CaseInsensitiveContains(filterText) || c.StartDate.ToString("dd/MM/yyyy").Contains(filterText);
            VacationsRequestListViewModel viewModel = new VacationsRequestListViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.CurrentFilterText = filterText;
            viewModel.VacationsRequests = _vacationsRequestRepo.GetVacationsRequests(attorney.Id)
                .Where(filterFunction)
                .Where(filterFunctionText)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _vacationsRequestRepo.GetVacationsRequests(attorney.Id).Count(filterFunction)
            };
            return View(viewModel);
        }
        [Authorize(Roles = "Administrador,Regular")]
        public IActionResult Admin(bool? filter, string filterText, int pageNumber = 1)
        {
            int currentUserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Func<VacationsRequest, bool> filterFunction = r =>(r.IsApproved == filter && filter == null && r.VacationsRequestAnswers.Any(vra => vra.ApproverId == currentUserId && vra.IsApproved == null)) ||  (r.IsApproved == filter && filter != null);
            Func<VacationsRequest, bool> filterFunctionText = c => String.IsNullOrEmpty(filterText)  || c.Attorney.Name.CaseInsensitiveContains(filterText) || c.StartDate.ToString("dd/MM/yyyy").Contains(filterText);
            VacationsRequestListViewModel viewModel = new VacationsRequestListViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.CurrentFilterText = filterText;
            viewModel.VacationsRequests = _vacationsRequestRepo.VacationsRequests()
                .Include(v => v.Attorney)
                .Where(filterFunction)
                .Where(filterFunctionText)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _vacationsRequestRepo.VacationsRequests().Count(filterFunction)
            };
            ViewBag.Answered = TempData["answered"];
            ViewBag.Message = TempData["message"];
            return View(viewModel);
        }
        [Authorize(Policy = "CanApproveVacations")]
        public IActionResult Answer(int id)
        {
            ViewBag.UpdatedVacationRequest = TempData["updated"];
            NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel();
            var vacations = _vacationsRequestRepo.VacationsRequests().Include(v => v.Attorney).Where(v => v.Id == id).FirstOrDefault();
            viewModel.VacationsRequest = vacations;
            viewModel.AttorneyName = vacations.Attorney.Name;
            if (viewModel.VacationsRequest == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }
        [Authorize(Policy = "CanApproveVacations")]
        [HttpPost]
        public IActionResult Answer(VacationsRequest vacationsRequest)
        {
            if (vacationsRequest.IsApproved == null)
            {
                ModelState.AddModelError("notAnswered", "No ha respondido aún a la solicitud");
            }
            if (!ModelState.IsValid)
            {
                var attorney = _attorneysRepo.Attorneys.Where(a => a.Id == vacationsRequest.AttorneyId).FirstOrDefault();
                NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel()
                {
                    VacationsRequest = vacationsRequest,
                    AttorneyName = attorney.Name
                };
                return View(viewModel);
            }
            else
            {
                int currentUserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                string message;
                _vacationsRequestRepo.Approve(vacationsRequest, currentUserId, out message);
                TempData["answered"] = true;
                TempData["message"] = message;
                return RedirectToAction("Admin");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ApplyMonthlyCredits()
        {
            string message;
            bool success;
            _vacationsRequestRepo.ApplyMonthlyVacationsCredit(out message, out success);
   
            return Json(new { success, message });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ApplyApprovals()
        {
            string message;
            bool success;

            _vacationsRequestRepo.ApplyApprovedVacationsRequests(out message, out success);
            return Json(new { success, message });
        }
    }
}
