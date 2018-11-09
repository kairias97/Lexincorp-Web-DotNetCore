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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AttorneyController : Controller
    {
        private readonly IDepartmentRepository _departmentsRepo;
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IUserRepository _usersRepo;
        private readonly IGuidManager _guidManager;
        private readonly ICryptoManager _cryptoManager;
        private readonly IMailSender _mailSender;
        public int PageSize = 5;
        public AttorneyController(IDepartmentRepository departmentsRepo, IAttorneyRepository attorneysRepo, IUserRepository usersRepo,
            IGuidManager guidManager, ICryptoManager cryptoManager, IMailSender sender)
        {
            this._departmentsRepo = departmentsRepo;
            this._attorneysRepo = attorneysRepo;
            this._usersRepo = usersRepo;
            this._guidManager = guidManager;
            this._cryptoManager = cryptoManager;
            this._mailSender = sender;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ViewResult New()
        {
            NewAttorneyViewModel viewModel = new NewAttorneyViewModel
            {
                Attorney = new Attorney { User = new User() },
                Departments = _departmentsRepo.Departments.ToList()
            };
            ViewBag.AddedAttorney = TempData["added"];
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
            if (!_attorneysRepo.VerifyEmail(attorney.Email))
            {
                ModelState.AddModelError("uqEmail", "El correo ingresado ya existe");
            }
            if (!_attorneysRepo.VerifyNotaryCode(attorney.NotaryCode))
            {
                ModelState.AddModelError("uqNotaryCode", "El código de notario ingresado ya existe");
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
                //string guidGenerated = _guidManager.GenerateGuid();
                //string passwordDefault = guidGenerated.Substring(guidGenerated.Length - 12, 12);
                string passwordOriginal = attorney.User.Password;
                string passwordHashed = _cryptoManager.HashString(attorney.User.Password);
                attorney.User.Password = passwordHashed;
                _attorneysRepo.Save(attorney);
                //Envío de password sin hash al usuario
                string emailBody = $"Se le ha creado un acceso a la aplicación Lexincorp Nicaragua Web, su usuario es {attorney.User.Username} " +
                    $"y su clave de acceso es {passwordOriginal}. \n**Este es un mensaje autogenerado por el sistema, favor no responder**";
                _mailSender.SendMail(attorney.Email, "Usuario web creado para aplicación Lexincorp Nicaragua Web", emailBody);
                TempData["added"] = true;
                return RedirectToAction("New");
            }
        }
        [Authorize]
        public IActionResult Admin(string filter, int pageNumber = 1)
        {

            ViewBag.Updated = TempData["updated"];
            Func<Attorney, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Name.CaseInsensitiveContains(filter) || c.IdentificationNumber.CaseInsensitiveContains(filter);
            ViewBag.ResetedPassword = TempData["ResetedPassword"];
            AttorneyListViewModel viewModel = new AttorneyListViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.Attorneys = _attorneysRepo.Attorneys
                .Include(a => a.Department)
                .Include(a => a.User)
                .Where(filterFunction)
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber,
                ItemsPerPage = PageSize,
                TotalItems = _attorneysRepo.Attorneys.Count(filterFunction)
            };
            TempData["filter"] = filter;
            return View(viewModel);
        }
        [Authorize]
        public IActionResult Edit(int id)
        {
            TempData.Keep();
            ViewBag.UpdatedAttorney = TempData["updated"];
            NewAttorneyViewModel viewModel = new NewAttorneyViewModel
            {
                Departments = _departmentsRepo.Departments.ToList()
            };
            viewModel.Attorney = _attorneysRepo.Attorneys.Where(a => a.Id == id).Include(a => a.User).Include(a => a.Department).First();
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
            TempData.Keep();
            if (!_usersRepo.VerifyUsername(attorney.User.Username) && !_usersRepo.VerifyAttorneyIDAndUsername(attorney.Id, attorney.UserId))
            {
                ModelState.AddModelError("uqUsername", "El usuario ingresado ya existe");
            }
            if (!_attorneysRepo.VerifyEmail(attorney.Email) && !_attorneysRepo.VerifyAttorneyIDAndEmailOwnership(attorney.Id, attorney.Email))
            {
                ModelState.AddModelError("uqEmail", "El correo ingresado ya existe");
            }
            if (!_attorneysRepo.VerifyNotaryCode(attorney.NotaryCode) && !_attorneysRepo.verifyAttorneyIDAndNotaryCodeOwnership(attorney.Id, attorney.NotaryCode))
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
                bool passwordModified = false;
                if(attorney.User.Password != null && (attorney.User.Password != " " || attorney.User.Password != ""))
                {
                    passwordModified = true;
                    string passwordOriginal = attorney.User.Password;
                    string passwordHashed = _cryptoManager.HashString(attorney.User.Password);
                    attorney.User.Password = passwordHashed;
                }
                _attorneysRepo.Save(attorney, passwordModified);
                _usersRepo.Save(attorney.User);
                TempData["updated"] = true;
                return RedirectToAction("Admin", new { filter = TempData["filter"]});
            }
        }

        [HttpPost]
        public IActionResult ResetPassword(int attorneyId)
        {
            _usersRepo.ResetPassword(attorneyId);
            TempData["ResetedPassword"] = true;
            return RedirectToAction(nameof(Admin), new { filter = TempData["filter"] });
        }
    }
}
