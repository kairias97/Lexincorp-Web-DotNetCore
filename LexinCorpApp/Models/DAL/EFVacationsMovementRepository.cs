using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFVacationsMovementRepository: IVacationsMovementRepository
    {
        private ApplicationDbContext context;
        public EFVacationsMovementRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }

        public void Save(VacationsMovement vacationsMovement)
        {
            context.VacationsMovements.Add(vacationsMovement);
            var attorney = context.Attorneys.Where(a => a.AttorneyId == vacationsMovement.AttorneyId).FirstOrDefault();
            if (vacationsMovement.MovementType == VacationMovementEnum.Credit)
            {
                attorney.VacationCount += vacationsMovement.Quantity;
            }
            else if(vacationsMovement.MovementType == VacationMovementEnum.Debit && attorney.VacationCount >= vacationsMovement.Quantity)
            {
                attorney.VacationCount -= vacationsMovement.Quantity;
            }
            context.SaveChanges();
        }
        public bool ValidateMovement(VacationsMovement vacationsMovement)
        {
            if(vacationsMovement.MovementType == VacationMovementEnum.Credit)
            {
                return true;
            }
            else
            {
                var attorney = context.Attorneys.Where(a => a.AttorneyId == vacationsMovement.AttorneyId).FirstOrDefault();
                if(attorney.VacationCount >= vacationsMovement.Quantity)
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
