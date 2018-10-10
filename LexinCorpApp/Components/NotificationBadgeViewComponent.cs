using LexincorpApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LexincorpApp.Components
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationBadgeViewComponent(INotificationRepository notificationRepo)
        {
            _notificationRepository = notificationRepo;
        }
        public IViewComponentResult Invoke()
        {
            int userId = Convert.ToInt32(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
            int closureNotificationsCount = _notificationRepository.GetPendingNotificationsCount(userId);
            return View(closureNotificationsCount);
        }
    }
}
