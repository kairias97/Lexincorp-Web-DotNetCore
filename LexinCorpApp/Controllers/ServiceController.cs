﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador, Regular")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            TempData["filter"] = filter;
            ViewBag.Updated = TempData["updated"];
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            TempData.Keep();
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
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Edit(Service service)
        {
            TempData.Keep();
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
            return RedirectToAction("Admin", new { filter = TempData["filter"] });
        }
        [Authorize(Roles = "Administrador, Regular")]
        public JsonResult GetByCategory(int categoryId)
        {
            var list = _servicesRepo.Services.Where(s => s.CategoryId == categoryId)
                .OrderBy(s => s.Name)
                .Select(s => new { Id = s.Id, Name = s.Name, EnglishDescription = s.EnglishDescription, SpanishDescription = s.SpanishDescription });
            return Json(list);
        }
    }
}