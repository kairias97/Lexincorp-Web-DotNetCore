using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class NewVacationsRequestViewModel
    {
        public decimal DaysAvailable { get; set; }
        public VacationsRequest VacationsRequest { get; set; }
        public string AttorneyName { get; set; }
    }
}
