using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Models.ViewModels;
using LexincorpApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    [Authorize(Roles ="Administrador")]
    public class VacationsMovementController : Controller
    {
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IVacationsMovementRepository _vacationsMovementRepo;
        private readonly IVacationsRequestRepository _vacationsRequestRepo;
        public VacationsMovementController(IAttorneyRepository _attorneysRepo, IVacationsMovementRepository _vacationsMovementRepo,
            IVacationsRequestRepository vacationsRequestRepository)
        {
            this._attorneysRepo = _attorneysRepo;
            this._vacationsMovementRepo = _vacationsMovementRepo;
            this._vacationsRequestRepo = vacationsRequestRepository;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Add()
        {
            var attorneys = _attorneysRepo.Attorneys.Where(a => a.User.Active).ToList();
            foreach (var a in attorneys)
            {
                a.AvailableVacationCount = _vacationsRequestRepo.GetAvailableVacationCount(a.UserId);
            }
            NewVacationsMovementViewModel viewModel = new NewVacationsMovementViewModel
            {
                Attorneys = attorneys,
                VacationsMovement = new VacationsMovement()
            };
            ViewBag.AddedMovement = TempData["added"];
            ViewBag.DaysInvalid = false;
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Add(VacationsMovement vacationsMovement)
        {
            if (!ModelState.IsValid)
            {
                NewVacationsMovementViewModel viewModel = new NewVacationsMovementViewModel()
                {
                    Attorneys = _attorneysRepo.Attorneys
                };
                viewModel.VacationsMovement = vacationsMovement;
                return View(viewModel);
            }
            else
            {
                if (_vacationsMovementRepo.ValidateMovement(vacationsMovement))
                {
                    var user = HttpContext.User;
                    int userId = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    _vacationsMovementRepo.Save(vacationsMovement, userId);
                    ViewBag.DaysInvalid = false;
                    TempData["added"] = true;
                    return RedirectToAction("Add");
                }
                else
                {
                    ViewBag.DaysInvalid = true;
                    NewVacationsMovementViewModel viewModel = new NewVacationsMovementViewModel()
                    {
                        Attorneys = _attorneysRepo.Attorneys
                    };
                    viewModel.VacationsMovement = vacationsMovement;
                    return View(viewModel);
                }                
            }
        }
    }
}
