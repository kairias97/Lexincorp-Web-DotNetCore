using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Models.ViewModels;
using LexincorpApp.Models;
using System.Data.Entity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class AttorneyController : Controller
    {
        private readonly IDepartmentRepository _departmentsRepo;
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IUserRepository _usersRepo;
        public int PageSize = 5;
        public AttorneyController(IDepartmentRepository _departmentsRepo, IAttorneyRepository _attorneysRepo, IUserRepository _usersRepo)
        {
            this._departmentsRepo = _departmentsRepo;
            this._attorneysRepo = _attorneysRepo;
            this._usersRepo = _usersRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ViewResult New(bool? added)
        {
            NewAttorneyViewModel viewModel = new NewAttorneyViewModel
            {
                Attorney = new Attorney { User = new User() },
                Departments = _departmentsRepo.Departments.ToList()
            };
            ViewBag.AddedAttorney = added ?? false;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult New(Attorney attorney)
        {
            if (!_usersRepo.VerifyUsername(attorney.User.Username))
            {
                ModelState.AddModelError("uqUsername", "El usuario ingresado ya existe está registrado");
            }
            if (!ModelState.IsValid)
            {
                NewAttorneyViewModel viewModel = new NewAttorneyViewModel
                {
                    Departments = _departmentsRepo.Departments.ToList()
                };
                viewModel.Attorney = attorney;
                return View(viewModel);
            }
            else
            {
                _attorneysRepo.Save(attorney);
                return RedirectToAction("New", new { added = true });
            }
        }
    }
}
