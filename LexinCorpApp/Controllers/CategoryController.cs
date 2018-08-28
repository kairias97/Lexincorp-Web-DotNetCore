using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private readonly  ICategoryRepository _categoriesRepo;
        public CategoryController(ICategoryRepository repo)
        {
            _categoriesRepo = repo;
        }
        public IActionResult New(bool? added)
        {
            ViewBag.AddedCategory = ViewData["added"];
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
            ViewData["added"] = true;
            return RedirectToAction(nameof(New));
        }
    }
}