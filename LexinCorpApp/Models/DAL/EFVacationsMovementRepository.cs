using LexincorpApp.Models.ExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFVacationsMovementRepository: IVacationsMovementRepository
    {
        private ApplicationDbContext context;
        private IVacationsRequestRepository _vacationsRequestRepo;
        private IMailSender _mailer;
        public EFVacationsMovementRepository(ApplicationDbContext ctx,
            IVacationsRequestRepository vacationsRequestRepository,
            IMailSender mailer)
        {
            this.context = ctx;
            this._vacationsRequestRepo = vacationsRequestRepository;
            _mailer = mailer;
        }

        public void Save(VacationsMovement vacationsMovement, int userId)
        {
            context.VacationsMovements.Add(vacationsMovement);
            var approverName = context.Attorneys.Where(a => a.UserId == userId).Select(a => a.Name).FirstOrDefault();
            var attorney = context.Attorneys.Where(a => a.Id == vacationsMovement.AttorneyId).FirstOrDefault();
            string notificationMessage = "El usuario {0} ha procedido a {1} en el sistema la cantidad de {2} días de vacaciones, por motivos de '{3}'." +
                " **Este es un mensaje autogenerado por el sistema, favor no responder**";
            string movement = "";
            string subject = "";
            if (vacationsMovement.MovementType == VacationMovementEnum.Credit)
            {
                movement = "acreditar";
                attorney.VacationCount += vacationsMovement.Quantity;
                subject = "Acreditación de días de vacaciones en sistema web";
            }
            else if(vacationsMovement.MovementType == VacationMovementEnum.Debit && attorney.VacationCount >= vacationsMovement.Quantity)
            {
                movement = "deducir";
                attorney.VacationCount -= vacationsMovement.Quantity;
                subject = "Deducción de días de vacaciones en sistema web";
            }
            notificationMessage = String.Format(notificationMessage, approverName, movement, vacationsMovement.Quantity, vacationsMovement.Reason);
            context.SaveChanges();
            _mailer.SendMail(attorney.Email, subject, notificationMessage);
        }
        public bool ValidateMovement(VacationsMovement vacationsMovement)
        {
            if(vacationsMovement.MovementType == VacationMovementEnum.Credit)
            {
                return true;
            }
            else
            {
                
                
                var userId = context.Attorneys.Where(a => a.Id == vacationsMovement.AttorneyId).Select(a => a.UserId).FirstOrDefault();
                var availableVacations = _vacationsRequestRepo.GetAvailableVacationCount(userId);
                if (availableVacations >= vacationsMovement.Quantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
