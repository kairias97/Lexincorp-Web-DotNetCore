using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class PreBillRequest
    {
        public int? PackageId { get; set; }
        public int? BillableRetainerId { get; set; }
        public bool? IncludeItems { get; set; }
        public int ClientId { get; set; }
        public DateTime BillDate { get; set; }
    }
}
