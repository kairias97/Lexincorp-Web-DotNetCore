using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class ClosureNotification
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int PackageId { get; set; }
        public virtual Package Package {get; set;}
        public bool Active { get; set; } = true;
        public bool? WasClosed { get; set; }
        public virtual ICollection<NotificationAnswer> NotificationAnswers { get; set; }
    }
}
