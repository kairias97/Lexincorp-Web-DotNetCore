using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IVacationsRequestRepository
    {
        bool ValidateRequest(int userId, decimal requestedQuantity);
        void Save(VacationsRequest vacationsRequest, int userId);
        void Approve(VacationsRequest vacationsRequest, int approverId, out string message);
        List<VacationsRequest> GetVacationsRequests(int attorneyId);
        IQueryable<VacationsRequest> VacationsRequests();
        decimal GetTotalVacationCount(int userId);
        decimal GetAvailableVacationCount(int userId);
        decimal GetReservedVacationCount(int userId);
        void ApplyApprovedVacationsRequests(out string message, out bool applied);
        void ApplyMonthlyVacationsCredit(out string message, out bool applied);
    }
}
