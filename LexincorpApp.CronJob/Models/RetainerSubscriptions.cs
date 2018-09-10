using System;
using System.Collections.Generic;

namespace LexincorpApp.CronJob.Models
{
    public partial class RetainerSubscriptions
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int RetainerId { get; set; }
        public decimal AgreedFee { get; set; }
        public decimal AgreedHours { get; set; }
        public decimal AdditionalFeePerHour { get; set; }
        public int CreatorId { get; set; }

        public Clients Client { get; set; }
        public Retainers Retainer { get; set; }
    }
}
