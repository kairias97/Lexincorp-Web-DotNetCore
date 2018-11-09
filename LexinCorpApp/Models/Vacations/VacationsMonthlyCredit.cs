using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class VacationsMonthlyCredit
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
