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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class AttorneyController : Controller
    {
        private readonly IDepartmentRepository _departmentsRepo;
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IUserRepository _usersRepo;
        private readonly IGuidManager _guidManager;
        private readonly ICryptoManager _cryptoManager;
        public int PageSize = 5;
        public AttorneyController(IDepartmentRepository _departmentsRepo, IAttorneyRepository _attorneysRepo, IUserRepository _usersRepo,
            IGuidManager _guidManager, ICryptoManager _cryptoManager)
        {
            this._departmentsRepo = _departmentsRepo;
            this._attorneysRepo = _attorneysRepo;
            this._usersRepo = _usersRepo;
            this._guidManager = _guidManager;
            this._cryptoManager = _cryptoManager;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
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
        [Authorize]
        [HttpPost]
        public IActionResult New(Attorney attorney)
        {
            if (!_usersRepo.VerifyUsername(attorney.User.Username))
            {
                ModelState.AddModelError("uqUsername", "El usuario ingresado ya existe");
            }
            //if(attorney.User.Username.Contains(" "))
            //{
            //    ModelState.AddModelError("whiteSpacesUsername", "El nombre de usuario contiene espacios en blanco, favor no incluir espacios en blanco");
            //}
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
                string guidGenerated = _guidManager.GenerateGuid();
                string passwordDefault = guidGenerated.Substring(guidGenerated.Length - 12, 12);
                //falta enviar password sin hash al user
                string passwordHashed = _cryptoManager.HashString(passwordDefault);
                attorney.User.Password = passwordHashed;
                _attorneysRepo.Save(attorney);
                return RedirectToAction("New", new { added = true });
            }
        }
        [Authorize]
        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Attorney, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Name.CaseInsensitiveContains(filter) || c.IdentificationNumber.CaseInsensitiveContains(filter);

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
        [Authorize]
        public IActionResult Edit(int id, bool? updated)
        {
            ViewBag.UpdatedAttorney = updated??false;
            NewAttorneyViewModel viewModel = new NewAttorneyViewModel
            {
                Departments = _departmentsRepo.Departments.ToList()
            };
            viewModel.Attorney = _attorneysRepo.Attorneys.Where(a => a.AttorneyId == id).Include(a => a.User).Include(a => a.Department).First();
            if (viewModel.Attorney == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(Attorney attorney)
        {
            if (!_usersRepo.VerifyUsername(attorney.User.Username) && !_usersRepo.VerifyAttorneyIDAndUsername(attorney.AttorneyId,attorney.UserId))
            {
                ModelState.AddModelError("uqUsername", "El usuario ingresado ya existe");
            }
            if (!_attorneysRepo.VerifyEmail(attorney.Email) && !_attorneysRepo.VerifyAttorneyIDAndEmailOwnership(attorney.AttorneyId, attorney.Email))
            {
                ModelState.AddModelError("uqEmail", "El correo ingresado ya existe");
            }
            if (!_attorneysRepo.VerifyNotaryCode(attorney.NotaryCode) && !_attorneysRepo.verifyAttorneyIDAndNotaryCodeOwnership(attorney.AttorneyId, attorney.NotaryCode))
            {
                ModelState.AddModelError("uqNotaryCode", "El código de notario ingresado ya existe");
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
                _usersRepo.Save(attorney.User);
                return RedirectToAction("Edit", new { updated = true, id = attorney.AttorneyId });
            }
        }
    }
}
