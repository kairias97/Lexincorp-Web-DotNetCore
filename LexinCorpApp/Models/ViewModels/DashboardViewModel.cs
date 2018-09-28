using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class DashboardViewModel
    {
        public decimal HoursWorked { get; set; }
        public decimal Vacations { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}
