using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class NewVacationsMovementViewModel
    {
        public IEnumerable<Attorney> Attorneys { get; set; }
        public VacationsMovement VacationsMovement { get; set; }
        public VacationMovementEnum VacationMovementEnum { get; set; }
    }
}
