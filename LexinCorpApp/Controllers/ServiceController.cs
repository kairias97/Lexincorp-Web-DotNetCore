using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _servicesRepo;
        private readonly ICategoryRepository _categoriesRepo;

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
    }
}