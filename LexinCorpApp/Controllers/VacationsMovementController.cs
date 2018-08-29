using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Models.ViewModels;
using LexincorpApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class VacationsMovementController : Controller
    {
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IVacationsMovementRepository _vacationsMovementRepo;
        public VacationsMovementController(IAttorneyRepository _attorneysRepo, IVacationsMovementRepository _vacationsMovementRepo)
        {
            this._attorneysRepo = _attorneysRepo;
            this._vacationsMovementRepo = _vacationsMovementRepo;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Add()
        {
            NewVacationsMovementViewModel viewModel = new NewVacationsMovementViewModel
            {
                Attorneys = _attorneysRepo.Attorneys,
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
                    _vacationsMovementRepo.Save(vacationsMovement);
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
