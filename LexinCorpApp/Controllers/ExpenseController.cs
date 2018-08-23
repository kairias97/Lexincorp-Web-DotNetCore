using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseRepository _expensesRepo;
        public int PageSize = 5;
        
        public ExpenseController(IExpenseRepository expensesRepository)
        {
            this._expensesRepo = expensesRepository;
        }
        public IActionResult New(bool? added)
        {
            ViewBag.AddedExpense = added;
            return View(new Expense());
        }
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
            Func<Expense, bool> filterFunction = e => String.IsNullOrEmpty(filter) || e.SpanishDescription.Contains(filter) || e.EnglishDescription.Contains(filter);
            ExpenseListViewModel vm = new ExpenseListViewModel
            {
                CurrentFilter = filter,
                Expenses = _expensesRepo.Expenses.Where(filterFunction),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = this.PageSize,
                    TotalItems = _expensesRepo.Expenses.Count(filterFunction)
                }
            };
            return View(vm);
        }
        public IActionResult Edit(int id, bool? updated)
        {
            ViewBag.UpdatedExpense = updated;
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
            return RedirectToAction(nameof(Edit), new { updated = true });
        }
    }
}