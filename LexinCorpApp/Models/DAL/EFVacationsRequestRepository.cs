using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFVacationsRequestRepository: IVacationsRequestRepository
    {
        private ApplicationDbContext context;
        public EFVacationsRequestRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public bool ValidateRequest(VacationsRequest vacationsRequest)
        {
            var attorney = context.Attorneys.Where(a => a.AttorneyId == vacationsRequest.AttorneyId).FirstOrDefault();
            if (attorney.VacationCount >= vacationsRequest.Quantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Save(VacationsRequest vacationsRequest)
        {
            context.VacationsRequests.Add(vacationsRequest);
            context.SaveChanges();
        }
        public void Update(VacationsRequest vacationsRequest)
        {
            context.Update(vacationsRequest);
            context.SaveChanges();
            var attorney = context.Attorneys.Where(a => a.AttorneyId == vacationsRequest.AttorneyId).FirstOrDefault();
            if(vacationsRequest.IsApproved == true && attorney.VacationCount >= vacationsRequest.Quantity)
            {
                attorney.VacationCount -= vacationsRequest.Quantity;
            }
            context.SaveChanges();
        }
        public IQueryable<VacationsRequest> VacationsRequests()
        {
            return context.VacationsRequests;
        }
        public List<VacationsRequest> GetVacationsRequests(int attorneyId)
        {
            return context.VacationsRequests.Where(v => v.AttorneyId == attorneyId).ToList();
        }
    }
}
