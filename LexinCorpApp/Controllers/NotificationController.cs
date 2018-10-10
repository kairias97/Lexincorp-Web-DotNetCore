using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexincorpApp.Controllers
{

    [Authorize(Roles = "Administrador, Regular")]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepo)
        {
            _notificationRepository = notificationRepo;
        }
        public IActionResult Index()
        {
            ViewBag.IsAnswerProcessed = TempData["IsAnswerProcessed"];
            ViewBag.AnswerSubmittedMessage = TempData["AnswerSubmittedMessage"];
            int userId = Convert.ToInt32(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
            var notifications = _notificationRepository.GetPendingNotifications(userId);
            return View(notifications);
        }
        [HttpPost]
        public IActionResult AllowClosure(int notificationAnswerId)
        {

            int userId = Convert.ToInt32(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
            if (!_notificationRepository.CheckPendingAnswer(notificationAnswerId, userId))
            {
                TempData["IsAnswerProcessed"] = false;
                TempData["AnswerSubmittedMessage"] = "No puede responder a la notificación debido a que ya no es válida";
            } else
            {
                bool wasClosed;
                _notificationRepository.AnswerClosureNotification(notificationAnswerId, userId, true, out wasClosed);
                TempData["IsAnswerProcessed"] = true;
                if (wasClosed)
                {
                    TempData["AnswerSubmittedMessage"] = "Se ha procesado con éxito la permisión del cierre del paquete y se ha procedido a cerrar el paquete";
                }
                else
                {
                    TempData["AnswerSubmittedMessage"] = "Se ha procesado con éxito la permisión del cierre del paquete, en espera de respuesta por parte del resto de usuarios activos.";
                }
            }
            
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public IActionResult DenyClosure(int notificationAnswerId)
        {
            int userId = Convert.ToInt32(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).First().Value);
            if (!_notificationRepository.CheckPendingAnswer(notificationAnswerId, userId))
            {
                TempData["IsAnswerProcessed"] = false;
                TempData["AnswerSubmittedMessage"] = "No puede responder a la notificación debido a que ya no es válida";
            } else
            {
                bool wasClosed;
                _notificationRepository.AnswerClosureNotification(notificationAnswerId, userId, false, out wasClosed);
                TempData["IsAnswerProcessed"] = true;
                TempData["AnswerSubmittedMessage"] = "Se ha procesado con éxito la cancelación del cierre del paquete";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}