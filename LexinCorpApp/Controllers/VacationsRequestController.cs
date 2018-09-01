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
            var id = user.Identity.Name;
            var attorney = _attorneysRepo.Attorneys.Where(x => x.UserId == Convert.ToInt32(id)).FirstOrDefault();
            NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel
            {
                DaysAvailable = attorney.VacationCount,
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
                var id = user.Identity.Name;
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
                var id = user.Identity.Name;
                var attorney = _attorneysRepo.Attorneys.Where(x => x.UserId == Convert.ToInt32(id)).FirstOrDefault();
                vacationsRequest.AttorneyId = attorney.Id;
                if (_vacationsRequestRepo.ValidateRequest(vacationsRequest))
                {
                    //vacationsRequest.AttorneyId = attorney.AttorneyId;
                    _vacationsRequestRepo.Save(vacationsRequest);
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
                        DaysAvailable = attorney.VacationCount,
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
            var id = user.Claims.Where(claim => claim.Type == ClaimTypes.NameIdentifier).First();
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
        [Authorize]
        public IActionResult Admin(bool? filter, string filterText, int pageNumber = 1)
        {
            Func<VacationsRequest, bool> filterFunction = c => c.IsApproved == filter;
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
            return View(viewModel);
        }
        [Authorize]
        public IActionResult Edit(int id)
        {
            ViewBag.UpdatedVacationRequest = TempData["updated"];
            NewVacationsRequestViewModel viewModel = new NewVacationsRequestViewModel();
            var vacations = _vacationsRequestRepo.VacationsRequests().Include(v => v.Attorney).Where(v => v.VacationsRequestId == id).FirstOrDefault();
            viewModel.VacationsRequest = vacations;
            viewModel.AttorneyName = vacations.Attorney.Name;
            if (viewModel.VacationsRequest == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(VacationsRequest vacationsRequest)
        {
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
                _vacationsRequestRepo.Update(vacationsRequest);
                return RedirectToAction("Admin");
            }
        }
    }
}
