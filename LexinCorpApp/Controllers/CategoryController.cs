using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoryController : Controller
    {
        public int PageSize = 5;

        private readonly  ICategoryRepository _categoriesRepo;
        public CategoryController(ICategoryRepository repo)
        {
            _categoriesRepo = repo;
        }
        public IActionResult New()
        {
            ViewBag.AddedCategory = TempData["added"];
            return View(new Category());
        }
        [HttpPost]
        public IActionResult New(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _categoriesRepo.Save(category);
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Category, bool> filterFunction = category => String.IsNullOrEmpty(filter) || category.Name.CaseInsensitiveContains(filter);
            CategoryListViewModel vm = new CategoryListViewModel
            {
                CurrentFilter = filter,
                Categories = _categoriesRepo.Categories
                    .Where(filterFunction)
                    .OrderBy(c => c.Id)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _categoriesRepo.Categories.Count(filterFunction)
                }
            };
            return View(vm);

        }

        public IActionResult Edit(int id)
        {
            Category category = _categoriesRepo.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.UpdatedCategory = TempData["updated"];
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            _categoriesRepo.Save(category);
            TempData["updated"] = true;
            return RedirectToAction(nameof(Edit), new { id = category.Id});
        }
    }
}