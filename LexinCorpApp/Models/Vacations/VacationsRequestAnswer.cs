using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class VacationsRequestAnswer
    {
        public int Id { get; set; }
        public bool? IsApproved { get; set; }
        public int RequestId { get; set; }
        public virtual VacationsRequest Request { get; set; }
        public int ApproverId {get; set;}
        public virtual User Approver { get; set; }

    }
}
