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
            if(attorney.User.Username.Contains(" "))
            {
                ModelState.AddModelError("whiteSpacesUsername", "El nombre de usuario contiene espacios en blanco, favor no incluir espacios en blanco");
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
        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Attorney, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Name.Contains(filter) || c.IdentificationNumber.Contains(filter);

            AttorneyListViewModel viewModel = new AttorneyListViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.Attorneys = _attorneysRepo.Attorneys
                .Include(a => a.Department)
                .Include(a => a.User)
                .Where(filterFunction)
                .OrderBy(a => a.AttorneyId)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _attorneysRepo.Attorneys.Count(filterFunction)
            };
            return View(viewModel);
        }
    }
}
