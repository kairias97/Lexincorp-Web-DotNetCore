using System;
using System.Collections.Generic;

namespace LexincorpApp.CronJob.Models
{
    public partial class Retainers
    {
        public Retainers()
        {
            BillableRetainers = new HashSet<BillableRetainers>();
            RetainerSubscriptions = new HashSet<RetainerSubscriptions>();
        }

        public int Id { get; set; }
        public string SpanishName { get; set; }
        public bool? Active { get; set; }
        public string EnglishName { get; set; }

        public ICollection<BillableRetainers> BillableRetainers { get; set; }
        public ICollection<RetainerSubscriptions> RetainerSubscriptions { get; set; }
    }
}
