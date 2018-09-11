using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IRetainerSubscriptionRepository _retainersubscriptionRepository;

        public RetainerSubscriptionController(IRetainerRepository retainerRepository,
            IRetainerSubscriptionRepository retainerSubscriptionRepository)
        {
            _retainerRepository = retainerRepository;
            _retainersubscriptionRepository = retainerSubscriptionRepository;
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
            _retainersubscriptionRepository.Save(RetainerSubscription);
            TempData["added"] = true;
            return RedirectToAction(nameof(New));
        }
        public JsonResult GetByClient(int clientId)
        {
            var results = _retainersubscriptionRepository.Subscriptions
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
    }
}