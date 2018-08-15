using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexinCorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LexinCorpApp.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(NewClientViewModel viewModel)
        {
            return View("New", viewModel);
        }
    }
}
