using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseRepository _expensesRepo;

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
    }
}