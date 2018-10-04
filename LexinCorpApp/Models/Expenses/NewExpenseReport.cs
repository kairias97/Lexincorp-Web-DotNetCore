using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class NewExpenseReport
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime InitialDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FinalDate { get; set; }
        public int? UserId { get; set; }
        public ActivityTypeEnum? ActivityType { get; set; }
    }
}
