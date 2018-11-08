using LexincorpApp.Models.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFVacationsRequestRepository: IVacationsRequestRepository
    {
        private ApplicationDbContext context;
        private IMailSender _mailer;
        private IConfiguration config;
        public EFVacationsRequestRepository(ApplicationDbContext ctx,
            IMailSender mailer, IConfiguration config)
        {
            this.context = ctx;
            this._mailer = mailer;
            this.config = config;
        }
        public bool ValidateRequest(int userId, decimal requestedQuantity)
        {

            decimal availableDays = GetAvailableVacationCount(userId);
            if (availableDays >= requestedQuantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Save(VacationsRequest vacationsRequest, int userId)
        {
            var attorney = context.Attorneys
                .Where(x => x.UserId == userId)
                .Select(a => new { a.Id , a.Name,  a.Email}).FirstOrDefault();
            vacationsRequest.AttorneyId = attorney.Id;
            //Get all the emails and userIds to Notify
            var approverAttorneys = context.Attorneys
                .Where(x => x.CanApproveVacations)
                .Select(a => new { a.Id, a.Email, a.UserId }).ToList();
            //Add the approval fields to the request made
            vacationsRequest.VacationsRequestAnswers = approverAttorneys.Select(a => new VacationsRequestAnswer
            {
                ApproverId = a.UserId,
                IsApproved = null
            }).ToList();
            context.VacationsRequests.Add(vacationsRequest);
            context.SaveChanges();
            string message = "El abogado '{0}' ha realizado una solicitud de {1} día(s) de vacaciones a comenzar el {2}, " +
                "debido al motivo '{3}'. Esta solicitud se encuentra en espera de aprobación. **Este es un mensaje autogenerado por el sistema, " +
                "favor no responder**";
            var mailList = approverAttorneys.Select(a => a.Email).ToList();
            var subject = "Nueva solicitud de vacaciones";
            mailList.Add(attorney.Email);
            message = String.Format(message, attorney.Name, vacationsRequest.Quantity, vacationsRequest.StartDate.ToString("dd/MM/yyyy"),
                vacationsRequest.Reason);
            _mailer.SendMail(mailList, subject, message);
            

        }
        public void Approve(VacationsRequest vacationsRequest, int approverId, out string message)
        {
            message = "";
            //Get the answer to be updated
            var approverAnswer = context.VacationsRequestAnswers
                .Where(a => a.RequestId == vacationsRequest.Id && a.ApproverId == approverId && a.IsApproved == null).FirstOrDefault();
            if (approverAnswer == null)
            {
                message = "No se puede responder a esta solicitud";
                return;
            }
            string subject = "";
            //To handle rejection
            if (vacationsRequest.IsApproved == false)
            {
                approverAnswer.IsApproved = false;
                //Notify of the rejection to everyone involved
                var notificationEmails = new List<string>();
                var requestor = context.Attorneys.Where(a => a.Id == vacationsRequest.AttorneyId).Select(a => new { a.Email, a.Id, a.Name }).First();
                
                //Not used the following since only to the requestor is notified of the rejectal
                //var otherApprovers = context.VacationsRequestAnswers
                //    .Include(x => x.Approver)
                //    .ThenInclude(u => u.Attorney)
                //    .Where(x => x.RequestId == vacationsRequest.Id && x.ApproverId != approverId)
                //    .Select(a => new { a.Approver.Attorney.Id, a.Approver.Attorney.Name, a.Approver.Attorney.Email })
                //    .ToList();

                var rejector = context.Attorneys.Where(a => a.UserId == approverId).Select(a => new { a.Name, a.Email, a.Id, a.UserId }).First();
                notificationEmails.Add(requestor.Email);
                //notificationEmails.Add(rejector.Email);
                //Notification for the rest of approvers so they dont have to worry about their approval
                //if (otherApprovers.Any())
                //{
                //    notificationEmails.AddRange(otherApprovers.Select(o => o.Email).ToList());
                //}
               
                string messageBody = $"La solicitud realizada por {requestor.Name} de {vacationsRequest.Quantity} día(s) de vacaciones para la fecha {vacationsRequest.StartDate.ToString("dd/MM/yyyy")}, " +
                    $"por motivo de '{vacationsRequest.Reason}', ha sido rechazada por {rejector.Name} debido a '{vacationsRequest.RejectReason}'. **Este es un mensaje autogenerado por el sistema, favor no responder**";
                message = "Se ha rechazado la solicitud exitosamente!";
                subject = "Rechazo de solicitud de vacaciones";
                context.Update(vacationsRequest);
                _mailer.SendMail(notificationEmails, subject, messageBody);
            } else if (vacationsRequest.IsApproved == true)
            {
                approverAnswer.IsApproved = true;
                //Check if there is someone else to approve besides the approver
                bool anyOtherPendingAnswer = context.VacationsRequestAnswers
                    .Any(x => x.RequestId == vacationsRequest.Id && x.ApproverId != approverId && x.IsApproved == null);
                if (anyOtherPendingAnswer)
                {
                    message = "Se ha respondido con éxito a la solicitud de vacaciones, la solicitud se encuentra en espera del resto de aprobaciones";
                } else
                {
                    vacationsRequest.RejectReason = null;

                    //Notify that it was marked as completed
                    var notificationEmails = new List<string>();
                    var requestor = context.Attorneys.Where(a => a.Id == vacationsRequest.AttorneyId).Select(a => new { a.Email, a.Id, a.Name }).First();

                    var otherApprovers = context.VacationsRequestAnswers
                        .Include(x => x.Approver)
                        .ThenInclude(u => u.Attorney)
                        .Where(x => x.RequestId == vacationsRequest.Id && x.ApproverId != approverId)
                        .Select(a => new { a.Approver.Attorney.Id, a.Approver.Attorney.Name, a.Approver.Attorney.Email })
                        .ToList();

                    var approver = context.Attorneys.Where(a => a.UserId == approverId).Select(a => new { a.Name, a.Email, a.Id, a.UserId }).First();

                    notificationEmails.Add(requestor.Email);
                    //Notify to the persons allowed via config of the application
                    string notifyTo = Convert.ToString(config["LexincorpAdmin:NotifyToVacationApproval"]);
                    notificationEmails.Add(notifyTo);
                    //notificationEmails.Add(approver.Email);

                    string otherApproverNamesArray = "";
                    //If i wanted to notify to the rest of the approvers
                    if (otherApprovers.Any())
                    {

                        otherApproverNamesArray = String.Join(", ", otherApprovers.Select(a => a.Name).ToList());
                        otherApproverNamesArray += " y ";
                        //If i wanted to notify the other approvers
                        //notificationEmails.AddRange(otherApprovers.Select(o => o.Email).ToList());
                    }

                    string messageBody = $"La solicitud realizada por {requestor.Name} de {vacationsRequest.Quantity} día(s) de vacaciones para la fecha {vacationsRequest.StartDate.ToString("dd/MM/yyyy")}, " +
                        $"por motivo de '{vacationsRequest.Reason}', ha sido completamente aprobada por {otherApproverNamesArray}{approver.Name}. **Este es un mensaje autogenerado por el sistema, favor no responder**";
                    message = "Se ha completado con éxito la aprobación de la solicitud de vacaciones";
                    subject = $"Solicitud de vacaciones aprobada para el {vacationsRequest.StartDate.ToString("dd/MM/yyyy")}";
                    context.Update(vacationsRequest);


                    _mailer.SendMail(notificationEmails, subject, messageBody);
                }

                    

                
            } else
            {
                message = "No se hizo nada, ni se afirmó ni rechazó la solicitud";
                return;
            }
            context.SaveChanges();
            //var attorney = context.Attorneys.Where(a => a.Id == vacationsRequest.AttorneyId).FirstOrDefault();
            //if(vacationsRequest.IsApproved == true && attorney.VacationCount >= vacationsRequest.Quantity)
            //{
            //    attorney.VacationCount -= vacationsRequest.Quantity;
            //}
            //context.SaveChanges();
        }
        public IQueryable<VacationsRequest> VacationsRequests()
        {
            return context.VacationsRequests;
        }
        public List<VacationsRequest> GetVacationsRequests(int attorneyId)
        {
            return context.VacationsRequests.Where(v => v.AttorneyId == attorneyId).ToList();
        }

        public decimal GetTotalVacationCount(int userId)
        {
            return context.Attorneys.Where(a => a.UserId == userId).Select(a => a.VacationCount).FirstOrDefault();
        }

        public decimal GetAvailableVacationCount(int userId)
        {
            return GetTotalVacationCount(userId) - GetReservedVacationCount(userId);
        }

        public decimal GetReservedVacationCount(int userId)
        {
            int attorneyId = context.Attorneys.Where(a => a.UserId == userId).Select(a => a.Id).FirstOrDefault();
            return context.VacationsRequests.Where(vr => vr.AttorneyId == attorneyId && (!vr.IsApplied && (vr.IsApproved == true || vr.IsApproved == null))).Sum(vr => vr.Quantity);
        }

        public void ApplyApprovedVacationsRequests(out string message, out bool applied)
        {
            var date = DateTime.UtcNow;
            var approvedUnappliedRequests = context.VacationsRequests
                .Include(vr => vr.Attorney)
                .Where(vr => (vr.StartDate.Year == date.Year && vr.StartDate.Month == date.Month && vr.StartDate.Day == date.Day) && !vr.IsApplied && vr.IsApproved == true)
                .ToList();
            if (approvedUnappliedRequests.Count > 0)
            {
                foreach (var vr in approvedUnappliedRequests)
                {
                    vr.IsApplied = true;
                    vr.Attorney.VacationCount -= vr.Quantity;
                    if (vr.Attorney.VacationCount < 0)
                    {
                        vr.Attorney.VacationCount = 0;
                    }
                }
                context.SaveChanges();
                message = $"Fueron aplicadas con éxito {approvedUnappliedRequests.Count} solicitudes aprobadas para el día de hoy {DateTime.UtcNow}";
                applied = true;
            } else
            {
                message = "No hay solicitudes aprobadas, pendientes de aplicar para el día de hoy";
                applied = false;
            }
            
        }

        public void ApplyMonthlyVacationsCredit(out string message, out bool applied)
        {
            DateTime currentDate = DateTime.UtcNow;
            int lastDayOfMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            bool wasAppliedThisMonth = context.VacationsMonthlyCredits.Any(vmc => vmc.Month == currentDate.Month && vmc.Year == currentDate.Year);

            if (currentDate.Day == lastDayOfMonth && !wasAppliedThisMonth)
            {
                var attorneys = context.Attorneys.Where(a => a.User.Active).ToList();
                foreach (var attorney in attorneys)
                {
                    attorney.VacationCount += (decimal) 2.5;
                }
                VacationsMonthlyCredit credit = new VacationsMonthlyCredit
                {
                    CreatedDate = DateTime.UtcNow,
                    Month = DateTime.UtcNow.Month,
                    Year = DateTime.UtcNow.Year
                };
                context.VacationsMonthlyCredits.Add(credit);
                context.SaveChanges();
                message = "Se aplicó el incremento en 2.5 días de vacaciones a todos los abogados.";
                applied = true;
                Log.Information("Ya fue aplicado para el mes {0} y año {1} el crédito de 2.5 días de vacaciones");
            } else
            {
                message = "No es el último día del mes o bien ya se aplicó para este mes el crédito de vacaciones a los abogados";
                applied = false;
            }
            
        }
    }
}
