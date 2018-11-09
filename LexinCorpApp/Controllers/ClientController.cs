using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LexincorpApp.Infrastructure;

namespace LexincorpApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientRepository _clientsRepo;
        private readonly IClientTypeRepository _clientTypesRepo;
        private readonly IBillingModeRepository _billingModesRepo;
        private readonly IDocumentDeliveryMethodRepository _documentDeliveryMethodsRepo;
        private readonly IRetainerSubscriptionRepository _retainerSubscriptionRepo;
        private readonly IPackageRepository _packageRepo;
        public int PageSize = 5;

        public ClientController(
            IClientRepository _clientsRepo,
            IClientTypeRepository _clientTypesRepo,
            IBillingModeRepository _billingModesRepo,
            IDocumentDeliveryMethodRepository _documentDeliveryMethodsRepo,
            IRetainerSubscriptionRepository retainerSubscriptionRepository,
            IPackageRepository packageRepository)
        {
            this._clientsRepo = _clientsRepo;
            this._clientTypesRepo = _clientTypesRepo;
            this._billingModesRepo = _billingModesRepo;
            this._documentDeliveryMethodsRepo = _documentDeliveryMethodsRepo;
            this._retainerSubscriptionRepo = retainerSubscriptionRepository;
            this._packageRepo = packageRepository;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            TempData["filter"] = filter;
            ViewBag.Updated = TempData["updated"];
            Func<Client, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Name.CaseInsensitiveContains(filter) || c.Contact.CaseInsensitiveContains(filter) || c.ContactEmail == filter;

            ClientsListViewModel viewModel = new ClientsListViewModel();
            viewModel.CurrentFilter = filter;
            viewModel.Clients = _clientsRepo.Clients
                .Include(c => c.ClientType)
                .Where(filterFunction)
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize);
            viewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = pageNumber, 
                ItemsPerPage = PageSize,
                TotalItems = _clientsRepo.Clients.Count(filterFunction)
            };
            return View(viewModel);
        }
        [Authorize]
        public ViewResult New()
        {
            ClientFormViewModel viewModel = new ClientFormViewModel
            {
                BillingModes = _billingModesRepo.BillingModes.ToList(),
                ClientTypes = _clientTypesRepo.ClientTypes.ToList(),
                DocumentDeliveryMethods = _documentDeliveryMethodsRepo.DocumentDeliveryMethods.ToList(),
                Client = new Client()
            };
            ViewBag.AddedClient = TempData["added"];
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult New(Client client)
        {
            if (!_clientsRepo.VerifyTributaryId(client.TributaryId))
            {
                ModelState.AddModelError("uqTributaryId", "La identificación tributaria especificada ya está asociada a otro cliente registrado");
            }
            if (!ModelState.IsValid)
            {
                ClientFormViewModel viewModel = new ClientFormViewModel
                {
                    BillingModes = _billingModesRepo.BillingModes.ToList(),
                    ClientTypes = _clientTypesRepo.ClientTypes.ToList(),
                    DocumentDeliveryMethods = _documentDeliveryMethodsRepo.DocumentDeliveryMethods.ToList()
                };
                viewModel.Client = client;
                return View(viewModel);
            } else
            {
                _clientsRepo.Save(client);
                TempData["added"] = true;
                return RedirectToAction("New");
            }

            
        }
        [Authorize]
        public IActionResult Edit(int id)
        {
            TempData.Keep();
            ViewBag.UpdatedClient = TempData["updated"];
            ClientFormViewModel vm = new ClientFormViewModel
            {
                Client = _clientsRepo.Clients
                    .Where(c => c.Id == id)
                    .First(),
                BillingModes = _billingModesRepo.BillingModes.ToList(),
                ClientTypes = _clientTypesRepo.ClientTypes.ToList(),
                DocumentDeliveryMethods = _documentDeliveryMethodsRepo.DocumentDeliveryMethods.ToList()
            };
            if (vm.Client == null)
            {
                return NotFound();
            }
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(Client client)
        {
            TempData.Keep();
            if (!_clientsRepo.VerifyTributaryId(client.TributaryId) && !_clientsRepo.VerifyTributaryIdOwnership(client.Id, client.TributaryId))
            {
                ModelState.AddModelError("uqTributaryId", "La identificación tributaria especificada ya está asociada a otro cliente registrado");
            }
            if (!ModelState.IsValid)
            {
                ClientFormViewModel viewModel = new ClientFormViewModel
                {
                    BillingModes = _billingModesRepo.BillingModes.ToList(),
                    ClientTypes = _clientTypesRepo.ClientTypes.ToList(),
                    DocumentDeliveryMethods = _documentDeliveryMethodsRepo.DocumentDeliveryMethods.ToList()
                };
                viewModel.Client = client;
                return View(viewModel);
            }
            else
            {
                _clientsRepo.Save(client);
                TempData["updated"] = true;
                return RedirectToAction("Admin", new { filter = TempData["filter"] });
            }
            
        }
        [Authorize]
        public JsonResult Search(string term)
        {
            Func<Client, bool> filterFunction = c => String.IsNullOrEmpty(term) || c.Name.CaseInsensitiveContains(term);

            var list = _clientsRepo.Clients.Include(c => c.Packages).Where(filterFunction)
                .OrderBy(c => c.Name)
                .Take(10)
                .Select(c=> new { Name = c.Name, Id = c.Id, BillingInEnglish = c.BillingInEnglish,
                FeePerHour = c.FixedCostPerHour, Packages = c.Packages.Where(p => p.IsFinished == false),
                IsHourBilled = c.IsHourBilled ? "Permite" : "No Permite"});
            return Json(list);
        }
        [Authorize]
        public IActionResult View(int id)
        {
            ClientDetailsViewModel viewModel = new ClientDetailsViewModel
            {
                Client = _clientsRepo.Clients
                    .Where(c => c.Id == id)
                    .First(),
                BillingModes = _billingModesRepo.BillingModes.ToList(),
                ClientTypes = _clientTypesRepo.ClientTypes.ToList(),
                DocumentDeliveryMethods = _documentDeliveryMethodsRepo.DocumentDeliveryMethods.ToList()
            };
            viewModel.RetainerSubscriptions = _retainerSubscriptionRepo.Subscriptions.Where(s => s.ClientId == id).OrderBy(c => c.Id);
            viewModel.Packages = _packageRepo.Packages.Where(p => p.ClientId == id && p.IsFinished == false).OrderBy(c => c.Id);
            return View(viewModel);
        }

    }
}
