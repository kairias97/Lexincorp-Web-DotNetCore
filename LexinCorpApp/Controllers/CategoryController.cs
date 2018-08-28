using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoryController : Controller
    {
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
    }
}