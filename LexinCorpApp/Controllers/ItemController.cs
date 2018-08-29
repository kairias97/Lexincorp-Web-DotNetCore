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
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemsRepo;
        public int PageSize = 5;
        public ItemController(IItemRepository itemsRepository)
        {
            _itemsRepo = itemsRepository;
        }

        public IActionResult New(bool? added)
        {
            ViewBag.AddedItem = added;
            return View(new Item());
        }
        [HttpPost]
        public IActionResult New(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }
            _itemsRepo.Save(item);
            return RedirectToAction(nameof(New), new { added = true });

        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            Func<Item, bool> filterFunction = i => String.IsNullOrEmpty(filter) || i.SpanishDescription.CaseInsensitiveContains(filter) || i.EnglishDescription.CaseInsensitiveContains(filter);
            ItemListViewModel vm = new ItemListViewModel
            {

                CurrentFilter = filter,
                Items = _itemsRepo.Items
                    .Where(filterFunction)
                    .OrderBy(i => i.Id)
                    .Skip((pageNumber - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _itemsRepo.Items.Count(filterFunction)
                }
            };
            return View(vm);
        }

        public IActionResult Edit(int id, bool? updated)
        {
            ViewBag.UpdatedItem = updated;
            Item item = _itemsRepo.Items.Where(i => i.Id == id).FirstOrDefault();
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost]
        public IActionResult Edit(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }
            _itemsRepo.Save(item);
            return RedirectToAction(nameof(Edit), new { updated = true });
        }

    }
}