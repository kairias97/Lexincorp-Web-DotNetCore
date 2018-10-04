using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LexincorpApp.Infrastructure;
using System.IO;
using Syncfusion.Report;
using Microsoft.EntityFrameworkCore;
using Syncfusion.ReportWriter;

namespace LexincorpApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseRepository _expensesRepo;
        private readonly IAttorneyRepository _attorneyRepo;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IActivityRepository _activityRepo;
        public int PageSize = 5;
        
        public ExpenseController(IExpenseRepository expensesRepository, IAttorneyRepository attorneyRepository,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IActivityRepository activityRepository)
        {
            this._expensesRepo = expensesRepository;
            this._attorneyRepo = attorneyRepository;
            _hostingEnvironment = hostingEnvironment;
            this._activityRepo = activityRepository;
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
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
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
        [Authorize]
        public IActionResult Report()
        {
            ExpenseReportViewModel viewModel = new ExpenseReportViewModel();
            viewModel.Attorneys = _attorneyRepo.Attorneys;
            viewModel.NewExpenseReport = new NewExpenseReport();
            //viewModel.NewExpenseReport.InitialDate = DateTime.Now;
            //viewModel.NewExpenseReport.FinalDate = DateTime.Now.Date;
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Report(NewExpenseReport newExpenseReport)
        {
            var list = new List<ActivityExpense>();
            list = _activityRepo.Expenses.Include(a => a.Expense).Include(a => a.Activity).ThenInclude(b => b.Package)
                .Include(a => a.Activity).ThenInclude(b => b.BillableRetainer)
                .Include(a => a.Activity).ThenInclude(b => b.Item)
                .Where(a => a.Activity.RealizationDate >= newExpenseReport.InitialDate && a.RealizationDate <= newExpenseReport.FinalDate
                    &&( newExpenseReport.UserId == null || a.Activity.CreatorId == newExpenseReport.UserId)
                    && (newExpenseReport.ActivityType == null || a.Activity.ActivityType == newExpenseReport.ActivityType)
                ).ToList();
            //if(newExpenseReport.UserId != null)
            //{
            //    list = list.Where(a => a.Activity.CreatorId == newExpenseReport.UserId).ToList();
            //}
            //if(newExpenseReport.ActivityType != null)
            //{
            //    list = list.Where(a => a.Activity.ActivityType == newExpenseReport.ActivityType).ToList();
            //}
            var gastos = list.Select(g => new {
                expenseName = g.Expense.Name,
                expenseDate = g.RealizationDate,
                expenseQuantity = g.Quantity,
                expensePrice = g.UnitAmount,
                expenseSubtotal = g.TotalAmount,
                expenseAssociatedTo = g.Activity.ActivityType == ActivityTypeEnum.Hourly ? "Horario" : g.Activity.ActivityType == ActivityTypeEnum.Item ?
                $"Item - {g.Activity.Item.Name}" : g.Activity.ActivityType == ActivityTypeEnum.Package ? $"Paquete - {g.Activity.Package.Name}" : g.Activity.ActivityType == ActivityTypeEnum.Retainer ? $"Retainer - {g.Activity.BillableRetainer.Name}" : ""
            }).ToList();
            //Simulacion de varios gastos
            gastos.AddRange(gastos);
            gastos.AddRange(gastos);
            gastos.AddRange(gastos);
            gastos.AddRange(gastos);
            gastos.AddRange(gastos);
            gastos.AddRange(gastos);
            string basePath = _hostingEnvironment.ContentRootPath;
            string fullPath = basePath + @"/Reports/Expenses.rdlc";
            FileStream inputStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            ReportDataSourceCollection dataSources = new ReportDataSourceCollection();
            dataSources.Add(new ReportDataSource { Name = "ExpensesDataSet", Value = gastos});

            Syncfusion.ReportWriter.ReportWriter writer = new Syncfusion.ReportWriter.ReportWriter(inputStream, dataSources);
            //writer.DataSources = dataSources;
            writer.ReportProcessingMode = ProcessingMode.Local;
            MemoryStream memoryStream = new MemoryStream();
            writer.Save(memoryStream, WriterFormat.PDF);
            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/pdf");
            fileStreamResult.FileDownloadName = $"ReporteDeGastosDel{newExpenseReport.InitialDate.ToString("ddMMyyyy")}Al{newExpenseReport.FinalDate.ToString("ddMMyyyy")}.pdf";
            return fileStreamResult;
        }
    }
}