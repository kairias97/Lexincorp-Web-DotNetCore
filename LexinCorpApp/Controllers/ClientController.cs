using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
namespace LexincorpApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientRepository _clientsRepo;
        private readonly IClientTypeRepository _clientTypesRepo;
        private readonly IBillingModeRepository _billingModesRepo;
        private readonly IDocumentDeliveryMethodRepository _documentDeliveryMethodsRepo;
        public int PageSize = 5;

        public ClientController(
            IClientRepository _clientsRepo,
            IClientTypeRepository _clientTypesRepo,
            IBillingModeRepository _billingModesRepo,
            IDocumentDeliveryMethodRepository _documentDeliveryMethodsRepo)
        {
            this._clientsRepo = _clientsRepo;
            this._clientTypesRepo = _clientTypesRepo;
            this._billingModesRepo = _billingModesRepo;
            this._documentDeliveryMethodsRepo = _documentDeliveryMethodsRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Client, bool> filterFunction = c => String.IsNullOrEmpty(filter) || c.Name.Contains(filter) || c.Contact.Contains(filter) || c.ContactEmail == filter;

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
        public ViewResult New(bool? added)
        {
            NewClientViewModel viewModel = new NewClientViewModel
            {
                BillingModes = _billingModesRepo.BillingModes.ToList(),
                ClientTypes = _clientTypesRepo.ClientTypes.ToList(),
                DocumentDeliveryMethods = _documentDeliveryMethodsRepo.DocumentDeliveryMethods.ToList(),
                Client = new Client()
            };
            ViewBag.AddedClient = added ?? false;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult New(Client client)
        {
            if (!_clientsRepo.VerifyTributaryId(client.TributaryId))
            {
                ModelState.AddModelError("uqTributaryId", "La identificación tributaria especificada ya está asociada a otro cliente registrado");
            }
            if (!ModelState.IsValid)
            {
                NewClientViewModel viewModel = new NewClientViewModel
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
                return RedirectToAction("New", new { added = true});
            }

            
        }

        
    }
}
