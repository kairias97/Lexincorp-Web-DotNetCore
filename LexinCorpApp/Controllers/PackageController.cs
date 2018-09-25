using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models;
using LexincorpApp.Models.ExternalServices;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PackageController : Controller
    {
        private readonly IAttorneyRepository _attorneysRepo;
        private readonly IPackageRepository _packagesRepo;
        private readonly IMailSender _mailSender;
        public int PageSize = 10;

        public PackageController(IAttorneyRepository attorneysRepo,
            IPackageRepository packagesRepo,
            IMailSender mailSender)
        {
            _attorneysRepo = attorneysRepo;
            _packagesRepo = packagesRepo;
            _mailSender = mailSender;
        }

        public IActionResult New()
        {
            ViewBag.AddedPackage = TempData["added"];
            NewPackageViewModel vm = new NewPackageViewModel
            {
                Package = new Package(),
                OnlyAdminNotification = true,
                IsEnglish = false,
                IsClientSelected = false

            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(Package package, string ClientName, bool IsEnglish, bool IsClientSelected, bool OnlyAdminNotification)
        {
            if (!ModelState.IsValid)
            {
                NewPackageViewModel vm = new NewPackageViewModel
                {
                    IsClientSelected = IsClientSelected,
                    Package = package,
                    OnlyAdminNotification = OnlyAdminNotification,
                    ClientName = ClientName,
                    IsEnglish = IsEnglish
                };
                return View(vm);
            }
            var currentUserIdClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First();
            //The creator is the current user
            package.CreatorUserId = Convert.ToInt32(currentUserIdClaim.Value);
            _packagesRepo.Save(package);
            //Handling notification
            Func<Attorney, bool> filterNotifications = attorney
                => !OnlyAdminNotification || attorney.User.IsAdmin == OnlyAdminNotification;
            var emails = _attorneysRepo.Attorneys.Where(filterNotifications).Select(a => a.Email);
            string msg = $"El usuario {HttpContext.User.Identity.Name} ha creado el paquete '{package.Name}' para el cliente {ClientName} " +
                $"por un monto de honorarios acordados de ${package.Amount}.";
            _mailSender.SendMail(emails, "Creación de paquete", msg);
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Package, bool> filterFunction = package => String.IsNullOrEmpty(filter) || package.Name.CaseInsensitiveContains(filter)
                || package.Client.Name.CaseInsensitiveContains(filter);
            PackageListViewModel vm = new PackageListViewModel
            {
                CurrentFilter = filter,
                Packages = _packagesRepo.Packages
                    .Include(p => p.Client)
                    .Where(filterFunction)
                    .OrderBy(r => r.Name)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _packagesRepo.Packages.Count(filterFunction)
                }
            };
            return View(vm);
        }

        public IActionResult Edit(int id)
        {
            var package = _packagesRepo.Packages.Include(p=> p.Client).Where(p => p.Id == id).FirstOrDefault();
            if (package == null)
            {
                return NotFound();
            }

            ViewBag.UpdatedPackage = TempData["updated"];
            return View(package);
        }
        [HttpPost]
        public IActionResult Edit(Package package)
        {
            if (!ModelState.IsValid)
            {
                return View(package);
            }
            _packagesRepo.Save(package);
            TempData["updated"] = true;
            return RedirectToAction(nameof(Edit), new { id = package.Id});
        }
        [Authorize]
        public JsonResult Search(int clientId)
        {
            var list = _packagesRepo.Packages.Include(p => p.Client).Where(p => p.ClientId == clientId && p.IsFinished == false)
                .OrderBy(p => p.Name)
                .Select(p => new { Name = p.Name, Id = p.Id });
            return Json(list);
        }
    }
}