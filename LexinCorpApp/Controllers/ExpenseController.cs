using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LexincorpApp.Infrastructure;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseRepository _expensesRepo;
        public int PageSize = 5;
        
        public ExpenseController(IExpenseRepository expensesRepository)
        {
            this._expensesRepo = expensesRepository;
        }
        [Authorize]
        public IActionResult New()
        {
            ViewBag.AddedExpense = TempData["added"];
            return View(new Expense());
        }
        [Authorize]
        [HttpPost]
        public IActionResult New(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return View(expense);
            }
            _expensesRepo.Save(expense);
            return RedirectToAction(nameof(New), new { added = true });
        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Expense, bool> filterFunction = e => String.IsNullOrEmpty(filter) || e.SpanishDescription.CaseInsensitiveContains(filter) || e.EnglishDescription.CaseInsensitiveContains(filter);
            ExpenseListViewModel vm = new ExpenseListViewModel
            {
                CurrentFilter = filter,
                Expenses = _expensesRepo.Expenses.Where(filterFunction)
                    .OrderBy(e => e.Id)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = this.PageSize,
                    TotalItems = _expensesRepo.Expenses.Count(filterFunction)
                }
            };
            return View(vm);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.UpdatedExpense = TempData["updated"];
            Expense expense = _expensesRepo.Expenses.Where(e => e.Id == id).FirstOrDefault();
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }
        [HttpPost]
        public IActionResult Edit(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return View(expense);
            }
            _expensesRepo.Save(expense);
            TempData["updated"] = true;
            return RedirectToAction(nameof(Edit));
        }
    }
}