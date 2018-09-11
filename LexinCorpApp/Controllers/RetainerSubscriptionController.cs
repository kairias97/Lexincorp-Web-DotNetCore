using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LexincorpApp.Infrastructure;
using LexincorpApp.Models;
using LexincorpApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LexincorpApp.Controllers
{
    public class RetainerSubscriptionController : Controller
    {
        public int PageSize = 10;
        private readonly IRetainerRepository _retainerRepository;
        private readonly IRetainerSubscriptionRepository _retainerSubscriptionRepository;

        public RetainerSubscriptionController(IRetainerRepository retainerRepository,
            IRetainerSubscriptionRepository retainerSubscriptionRepository)
        {
            _retainerRepository = retainerRepository;
            _retainerSubscriptionRepository = retainerSubscriptionRepository;
        }
        public IActionResult New()
        {
            ViewBag.AddedSubscription = TempData["added"];
            NewRetainerSubscriptionViewModel vm = new NewRetainerSubscriptionViewModel
            {
                RetainerSubscription = new RetainerSubscription(),
                IsEnglish = false,
                IsClientSelected = false,
                ClientName = "",
                Retainers = _retainerRepository.Retainers
                    .Where(r => r.Active)

            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(RetainerSubscription RetainerSubscription, string ClientName, bool IsEnglish, bool IsClientSelected)
        {
            if (!ModelState.IsValid)
            {
                NewRetainerSubscriptionViewModel vm = new NewRetainerSubscriptionViewModel
                {
                    IsClientSelected = IsClientSelected,
                    RetainerSubscription = RetainerSubscription,
                    ClientName = ClientName,
                    IsEnglish = IsEnglish,
                    Retainers = _retainerRepository.Retainers.Where(r => r.Active)
                };
                return View(vm);
            }

            var currentUserIdClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First();
            //The creator is the current user
            RetainerSubscription.CreatorId = Convert.ToInt32(currentUserIdClaim.Value);
            _retainerSubscriptionRepository.Save(RetainerSubscription);
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
        }
        public JsonResult GetByClient(int clientId)
        {
            var results = _retainerSubscriptionRepository.Subscriptions
                .Include(s => s.Retainer)
                .Where(rs => rs.ClientId == clientId)
                .Select(rs => new {
                    id = rs.Id,
                    retainerType = new {id = rs.Retainer.Id, spanishName= rs.Retainer.SpanishName, englishName = rs.Retainer.EnglishName},
                    agreedFee = rs.AgreedFee,
                    agreedHours = rs.AgreedHours,
                    additionalFeePerHour = rs.AdditionalFeePerHour,
                    clientId = rs.ClientId
                });
            return Json(results);
        }

        public IActionResult Admin(string filter, int pageNumber = 1)
        {
            TempData["filter"] = filter;
            ViewBag.Deleted = TempData["deleted"];
            Func<RetainerSubscription, bool> filterFunction = rs => String.IsNullOrEmpty(filter) || rs.Client.Name.CaseInsensitiveContains(filter);
            RetainerSubscriptionListViewModel vm = new RetainerSubscriptionListViewModel
            {
                CurrentFilter = filter,
                Subscriptions = _retainerSubscriptionRepository.Subscriptions
                    .Include(rs => rs.Client)
                    .Include(rs => rs.Retainer)
                    .Where(filterFunction)
                    .OrderBy(rs => rs.Retainer.SpanishName)
                    .Skip((pageNumber - 1) * pageNumber)
                    .Take(pageNumber),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage = PageSize,
                    TotalItems = _retainerSubscriptionRepository.Subscriptions.Count(filterFunction)
                }
            };
            return View(vm);
        }
        public IActionResult Edit(int id)
        {
            var subscription = _retainerSubscriptionRepository.Subscriptions
                .Include(s => s.Client)
                .Include(s => s.Retainer)
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (subscription == null)
            {
                return NotFound();
            }

            ViewBag.UpdatedSubscription = TempData["updated"];
            return View(subscription);
        }
        [HttpPost]
        public IActionResult Edit(RetainerSubscription retainerSubscription)
        {
            if (!ModelState.IsValid)
            {
                return View(retainerSubscription);
            }
            _retainerSubscriptionRepository.Save(retainerSubscription);
            TempData["updated"] = true;
            return RedirectToAction(nameof(Edit), new { id = retainerSubscription.Id });
        }
        [HttpPost]
        public IActionResult Delete(int retainerSubscriptionId)
        {
            
            _retainerSubscriptionRepository.Delete(retainerSubscriptionId);
            TempData["deleted"] = true;
            return RedirectToAction(nameof(Admin), new { filter = TempData["filter"], pageNumber = 1});
        }
    }
}