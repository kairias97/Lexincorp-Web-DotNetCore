using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ActivityReportViewModel
    {
        public IEnumerable<Attorney> Attorneys { get; set; }
        public NewActivityReport NewActivityReport { get; set; }
    }
}
