﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LexincorpApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseRepository _expensesRepo;

        public ExpenseController(IExpenseRepository expensesRepository)
        {
            this._expensesRepo = expensesRepository;
        }
        [Authorize]
        public IActionResult New(bool? added)
        {
            ViewBag.AddedExpense = added;
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
    }
}