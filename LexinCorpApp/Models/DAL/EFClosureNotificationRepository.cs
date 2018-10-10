using LexincorpApp.Models.ExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace LexincorpApp.Models
{
    public class EFClosureNotificationRepository : INotificationRepository
    {
        private ApplicationDbContext context;
        private IMailSender mailer;
        public EFClosureNotificationRepository(ApplicationDbContext ctx,
            IMailSender mailer)
        {
            context = ctx;
            this.mailer = mailer;
        }
        public void AnswerClosureNotification(int notificationAnswerId, int userId, bool isAffirmative, out bool wasClosed)
        {
            if (CheckPendingAnswer(notificationAnswerId, userId))
            {
                var usersToNotify = context.Users
                    .Where(u => u.Active && u.Username != "webAdmin" && u.Id != userId)
                    .Select(u => new { UserId = u.Id, Email = u.Attorney.Email }).ToList();

                string petitionerUser = context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.Username).First();

                var notification = context.ClosureNotifications
                    .Where(cn => cn.NotificationAnswers.Any(na => na.Id == notificationAnswerId))
                    .First();

                var packageToClose = context.Packages.Where(p => p.Id == notification.PackageId)
                    .Select(p => new { p.Name, ClientName = p.Client.Name })
                    .First();
                //Check if there is no users besides the petitioner to have to answer to the notification
                //Ignoring the default webAdmin user
                //In the case that is rejected the notification itself
                if (!isAffirmative)
                {
                    notification.Active = false;
                    notification.WasClosed = false;
                    var notificationAnswer = new NotificationAnswer
                    {
                        Id = notificationAnswerId,
                        IsAnswered = true,
                        WasAffirmative = false
                    };
                    context.NotificationAnswers.Attach(notificationAnswer);
                    context.Entry(notificationAnswer).Property(na => na.IsAnswered).IsModified = true;
                    context.Entry(notificationAnswer).Property(na => na.WasAffirmative).IsModified = true;
                    context.SaveChanges();
                    //Notify that it was cancelled
                    if (usersToNotify.Any())
                    {
                        //Notify that the closure was cancelled
                        var emailsToNotify = usersToNotify.Select(u => u.Email).ToList();
                        string subject = $"Cierre de paquete {packageToClose.Name} denegado";
                        string body = $"El usuario {petitionerUser} ha denegado una solicitud para cerrar el paquete '{packageToClose.Name}' del cliente {packageToClose.ClientName}. " +
                            $"\n**Este es un mensaje autogenerado por el sistema, favor no responder**";
                        mailer.SendMail(emailsToNotify, subject, body);
                    }
                    wasClosed = false;
                }
                else
                {
                    //In the case is accepted
                    var notificationAnswer = new NotificationAnswer
                    {
                        Id = notificationAnswerId,
                        IsAnswered = true,
                        WasAffirmative = true
                    };
                    context.NotificationAnswers.Attach(notificationAnswer);
                    context.Entry(notificationAnswer).Property(na => na.IsAnswered).IsModified = true;
                    context.Entry(notificationAnswer).Property(na => na.WasAffirmative).IsModified = true;
                    context.SaveChanges();
                    //Check if there is no more pending answers
                    if (!context.NotificationAnswers.Any(na => na.ClosureNotificationId == notification.Id && !na.IsAnswered))
                    {
                        //Meaning that the package can be closed
                        notification.Active = false;
                        notification.WasClosed = true;
                        var targetPackage = new Package
                        {
                            Id = notification.PackageId,
                            IsFinished = true
                        };

                        context.Packages.Attach(targetPackage);
                        context.Entry(targetPackage).Property(p => p.IsFinished).IsModified = true;
                        context.SaveChanges();
                        //Notify that the package was successfully closed
                        var emailsToNotify = usersToNotify.Select(u => u.Email).ToList();
                        string subject = $"Cierre de paquete {packageToClose.Name} completado";
                        string body = $"Se ha marcado como finalizado el paquete '{packageToClose.Name}' del cliente {packageToClose.ClientName} " +
                            $"dado que ha sido permitido por todos los usuarios activos. " +
                            $"\n**Este es un mensaje autogenerado por el sistema, favor no responder**";
                        mailer.SendMail(emailsToNotify, subject, body);
                        wasClosed = true;
                    } else
                    {
                        wasClosed = false;
                    }
                }
            } else
            {
                wasClosed = false;
            }
            
        }

        

        public bool CheckPendingAnswer(int notificationAnswerId, int userId)
        {
            return context.NotificationAnswers.Any(na => na.Id == notificationAnswerId && na.TargetUserId == userId
            && !na.IsAnswered && na.ClosureNotification.Active);
        }

        public IEnumerable<NotificationAnswer> GetPendingNotifications(int userId)
        {
            return context.NotificationAnswers
                .Include(na => na.ClosureNotification)
                .Where(na => na.TargetUserId == userId && !na.IsAnswered && na.ClosureNotification.Active)
                .ToList();

        }

        public int GetPendingNotificationsCount(int userId)
        {
            return context.NotificationAnswers
                .Count(na => na.TargetUserId == userId && !na.IsAnswered && na.ClosureNotification.Active);
        }

        public void RequestPackageClosure(int packageId, int petitionerId, out bool wasClosed)
        {
            wasClosed = false;
            if (!VerifyExistingOpenNotification(packageId))
            {
                //Check if there is no users besides the petitioner to have to answer to the notification
                //Ignoring the default webAdmin user
                var usersToNotify = context.Users
                    .Where(u => u.Active && u.Username != "webAdmin" && u.Id != petitionerId)
                    .Select(u => new { UserId = u.Id, Email = u.Attorney.Email }).ToList();

                if (usersToNotify.Count > 0)
                {
                    string petitionerUser = context.Users
                        .Where(u => u.Id == petitionerId)
                        .Select(u => u.Username).First();
                    wasClosed = false;
                    var packageToClose = context.Packages.Where(p => p.Id == packageId).Select(p => new { p.Name, ClientName = p.Client.Name}).First();

                    ClosureNotification newNotification = new ClosureNotification
                    {
                        Active = true,
                        Description = $"Cierre de paquete '{packageToClose.Name}' del cliente {packageToClose.ClientName}",
                        PackageId = packageId,
                        WasClosed = null
                    };
                    newNotification.NotificationAnswers = usersToNotify.Select(u => new NotificationAnswer {
                        TargetUserId = u.UserId,
                        IsAnswered = false,
                        WasAffirmative  = null
                        
                    }).ToList();
                    context.ClosureNotifications.Add(newNotification);
                    context.SaveChanges();
                    //Notify that the closure is in course
                    var emailsToNotify = usersToNotify.Select(u => u.Email).ToList();
                    string subject = $"Solicitud activa de Cierre de paquete {packageToClose.Name}";
                    string body = $"El usuario {petitionerUser} ha iniciado una solicitud para cerrar el paquete '{packageToClose.Name}' del cliente {packageToClose.ClientName}. " +
                        $"Permita o evite este cierre de paquete en la sección de notificaciones del sistema" +
                        $"\n**Este es un mensaje autogenerado por el sistema, favor no responder**";
                    mailer.SendMail(emailsToNotify, subject, body);
                } else 
                {
                    //Close the package without notifying since the requester is the one that closed the package
                    var packageToClose = new Package
                    {
                        Id = packageId,
                        IsFinished = true
                    };
                    context.Packages.Attach(packageToClose);
                    context.Entry(packageToClose).Property(p => p.IsFinished).IsModified = true;
                    context.SaveChanges();
                    wasClosed = true;

                }

            }
        }

        public bool VerifyExistingOpenNotification(int packageId)
        {
            return context.ClosureNotifications.Any(cn => cn.PackageId == packageId && cn.Active);
        }
    }
}
