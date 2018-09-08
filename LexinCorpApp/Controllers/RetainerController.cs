using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    public class RetainerController : Controller
    {
        private readonly IRetainerRepository _retainersRepo;
        public int PageSize = 5;
        public RetainerController(IRetainerRepository retainerRepo)
        {
            _retainersRepo = retainerRepo;
        }
        public IActionResult New()
        {
            ViewBag.AddedRetainer = TempData["added"];
            return View(new Retainer());
        }
        [HttpPost]
        public IActionResult New(Retainer retainer)
        {
            if (!ModelState.IsValid)
            {
                return View(retainer);
            }
            _retainersRepo.Save(retainer);
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
        }
        public IActionResult Edit(int id)
        {
            var retainer = _retainersRepo.Retainers.Where(r => r.Id == id).FirstOrDefault();
            if (retainer == null)
            {
                return NotFound();
            }

            ViewBag.UpdatedRetainer = TempData["updated"];
            return View(retainer);
        }
        [HttpPost]
        public IActionResult Edit(Retainer retainer)
        {
            if (!ModelState.IsValid)
            {
                return View(retainer);
            }
            _retainersRepo.Save(retainer);
            TempData["updated"] = true;
            return RedirectToAction(nameof(Edit), new { id = retainer.Id});
        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Retainer, bool> filterFunction = r => String.IsNullOrEmpty(filter) || r.SpanishName.CaseInsensitiveContains(filter) 
                || r.EnglishName.CaseInsensitiveContains(filter);
            RetainerListViewModel vm = new RetainerListViewModel
            {
                CurrentFilter = filter,
                Retainers = _retainersRepo.Retainers
                    .Where(filterFunction)
                    .OrderBy(r => r.SpanishName)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _retainersRepo.Retainers.Count(filterFunction)
                }
            };
            return View(vm);
        }


    }
}