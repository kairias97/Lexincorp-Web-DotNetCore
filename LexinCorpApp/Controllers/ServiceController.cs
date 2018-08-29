using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LexincorpApp.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _servicesRepo;
        private readonly ICategoryRepository _categoriesRepo;
        public int PageSize = 4;
        public ServiceController(IServiceRepository servicesRepo, 
            ICategoryRepository categoriesRepo)
        {
            _servicesRepo = servicesRepo;
            _categoriesRepo = categoriesRepo;
        }

        public IActionResult New()
        {
            ViewBag.AddedService = TempData["added"];
            NewServiceViewModel vm = new NewServiceViewModel
            {
                Service = new Service(),
                Categories = _categoriesRepo.Categories
                        .OrderBy(c => c.Name).ToList()
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(Service service)
        {
            if (!ModelState.IsValid)
            {
                NewServiceViewModel vm = new NewServiceViewModel
                {
                    Service = service,
                    Categories = _categoriesRepo.Categories
                        .OrderBy(c => c.Name)
                };
                return View(vm);
            }
            _servicesRepo.Save(service);
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Service, bool> filterFunction = service => String.IsNullOrEmpty(filter) || service.Name.CaseInsensitiveContains(filter);
            ServiceListViewModel vm = new ServiceListViewModel
            {
                CurrentFilter = filter,
                Services = _servicesRepo.Services
                    .Include(service => service.Category)
                    .Where(filterFunction)
                    .OrderBy(c => c.Id)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _servicesRepo.Services.Count(filterFunction)
                }
            };
            return View(vm);

        }

        public IActionResult Edit(int id)
        {
            
            Service service = _servicesRepo.Services.Where(s => s.Id == id).FirstOrDefault();
            if (service == null)
            {
                return NotFound();
            }
            EditServiceViewModel vm = new EditServiceViewModel
            {
                Service = service,
                Categories = _categoriesRepo.Categories
                    .OrderBy(c => c.Name)
            };
            ViewBag.UpdatedService = TempData["updated"];
            return View(vm);
        }
        [HttpPost]
        public IActionResult Edit(Service service)
        {
            if (!ModelState.IsValid)
            {
                EditServiceViewModel vm = new EditServiceViewModel
                {
                    Service = service,
                    Categories = _categoriesRepo.Categories
                    .OrderBy(c => c.Name)
                };
                return View(vm);
            }
            _servicesRepo.Save(service);
            TempData["updated"] = true;
            return RedirectToAction(nameof(Edit), new { id = service.Id });
        }
    }
}