using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IVacationsMovementRepository
    {
        void Save(VacationsMovement vacationsMovement);
        bool ValidateMovement(VacationsMovement vacationsMovement);
    }
}
