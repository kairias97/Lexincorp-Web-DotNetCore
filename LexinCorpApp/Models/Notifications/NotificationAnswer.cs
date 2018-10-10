using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class NotificationAnswer
    {
        public int Id { get; set; }
        public int ClosureNotificationId { get; set; }
        public virtual ClosureNotification ClosureNotification { get; set; }
        public int TargetUserId { get; set; }
        public virtual User TargetUser { get; set; }
        public bool? WasAffirmative { get; set; }
        public bool IsAnswered { get; set; } = false;
    }
}
