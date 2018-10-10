using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BillRequest
    {
        public bool? Hours { get; set; }
        public int? PackageId { get; set; }
        public int? BillableRetainerId { get; set; }
        public int? ItemId { get; set; }
    }
}
