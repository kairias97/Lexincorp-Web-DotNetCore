using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LexincorpApp.Controllers
{
    public class BillController : Controller
    {
        private readonly IItemRepository _itemRepo;
        public BillController(IItemRepository itemRepository)
        {
            this._itemRepo = itemRepository;
        }
        [Authorize]
        public IActionResult PreBilling()
        {
            NewPreBillViewModel viewModel = new NewPreBillViewModel();
            viewModel.Items = _itemRepo.Items.ToList();
            return View(viewModel);
        }
        public JsonResult GenerateBill(BillHeader billHeader, BillRequest billRequest)
        {
            return Json(new { message = "Factura ingresada exitosamente", success = true });
        }
    }
}
