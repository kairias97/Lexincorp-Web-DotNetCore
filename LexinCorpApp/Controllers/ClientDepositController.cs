using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LexincorpApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles ="Administrador,Regular")]
    public class ClientDepositController : Controller
    {
        public int PageSize = 10;
        private readonly IClientDepositRepository _clientDepositRepo;
        private readonly IPackageRepository _packageRepo;
        public ClientDepositController(IClientDepositRepository clientDepositRepo,
            IPackageRepository packageRepo)
        {
            _clientDepositRepo = clientDepositRepo;
            _packageRepo = packageRepo;
        }

        [Authorize(Policy = "CanAdminDeposits")]
        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            TempData["filter"] = filter;
            ViewBag.Created = TempData["added"];
            ViewBag.Updated = TempData["updated"];
            Func<ClientDeposit, bool> filterFunction = cd => String.IsNullOrEmpty(filter)
                    || cd.Client.Name.CaseInsensitiveContains(filter)
                    || cd.Package.Name.CaseInsensitiveContains(filter);
            var vm = new ClientDepositsViewModel
            {
                CurrentFilter = filter,
                ClientDeposits = _clientDepositRepo.ClientDeposits
                .Include(cd => cd.Client)
                .Include(cd => cd.Package)
                    .Where(filterFunction)
                    .OrderByDescending(r => r.ReceivedDate)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _clientDepositRepo.ClientDeposits.Count()
                }
            };
            return View(vm);
        }
        [Authorize(Policy = "CanAdminDeposits")]
        public IActionResult New()
        {
            TempData.Keep();
            NewClientDepositViewModel vm = new NewClientDepositViewModel
            {
                ClientDeposit = new ClientDeposit(),
                IsClientSelected = false,
                ClientName = "",
                Packages = new List<Package>()

            };
            return View(vm);
        }
        [Authorize(Policy = "CanAdminDeposits")]
        [HttpPost]
        public IActionResult New(ClientDeposit clientDeposit, string ClientName, bool IsClientSelected)
        {
            TempData.Keep();
            if (clientDeposit.PackageId == 0 || !_clientDepositRepo.IsDepositValid(clientDeposit.PackageId, clientDeposit.Amount))
            {
                ModelState.AddModelError("not_allowed_amount", "El paquete es inválido o el monto especificado supera el total abonable y abonado al paquete hasta el momento");
            }
            if (!ModelState.IsValid)
            {
                NewClientDepositViewModel vm = new NewClientDepositViewModel
                {
                    IsClientSelected = IsClientSelected,
                    ClientDeposit = clientDeposit,
                    ClientName = ClientName,
                     Packages = _packageRepo.Packages.Where(p => p.ClientId == clientDeposit.ClientId).ToList()
                };
                return View(vm);
            }
            var currentUserIdClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First();
            //The creator is the current user
            clientDeposit.CreatorId = Convert.ToInt32(currentUserIdClaim.Value);
            _clientDepositRepo.Save(clientDeposit);
            
            TempData["added"] = true;
            return RedirectToAction(nameof(Admin), new { filter = TempData["filter"] });
        }
        [Authorize(Policy = "CanAdminDeposits")]
        public IActionResult Edit(int id)
        {
            TempData.Keep();

            var clientDeposit = _clientDepositRepo.ClientDeposits
                .Include(cd => cd.Client)
                .Include(cd => cd.Package)
                .Where(cd => cd.Id == id)
                .FirstOrDefault();
            if (clientDeposit == null)
            {
                return NotFound();
            }

            return View(clientDeposit);
        }

        [Authorize(Policy = "CanAdminDeposits")]
        [HttpPost]
        public IActionResult Edit(ClientDeposit clientDeposit)
        {
            TempData.Keep();
            //To remove client and package required fields
            ModelState.Remove("Client.Name");
            ModelState.Remove("Client.Contact");
            ModelState.Remove("Client.TributaryId");
            ModelState.Remove("Package.Name");
            ModelState.Remove("Package.Description");
            ModelState.Remove("Package.RealizationDate");
            if (clientDeposit.PackageId == 0 || !_clientDepositRepo.IsDepositValid(clientDeposit.Id, clientDeposit.PackageId, clientDeposit.Amount))
            {
                ModelState.AddModelError("not_allowed_amount", "El paquete es inválido o el monto especificado supera el total abonable y abonado al paquete hasta el momento");
            }
            if (!ModelState.IsValid)
            {
                
                return View(clientDeposit);
            }
            _clientDepositRepo.Save(clientDeposit);

            TempData["updated"] = true;
            return RedirectToAction(nameof(Admin), new { filter = TempData["filter"] });
        }
    }
}