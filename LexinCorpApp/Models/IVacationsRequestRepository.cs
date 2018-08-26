using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IVacationsRequestRepository
    {
        bool ValidateRequest(VacationsRequest vacationsRequest);
        void Save(VacationsRequest vacationsRequest);
        void Update(VacationsRequest vacationsRequest);
        List<VacationsRequest> GetVacationsRequests(int attorneyId);
        IQueryable<VacationsRequest> VacationsRequests();
    }
}
