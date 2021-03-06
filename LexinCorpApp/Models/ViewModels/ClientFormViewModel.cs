﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class ClientFormViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<ClientType> ClientTypes { get; set; }
        public IEnumerable<BillingMode> BillingModes { get; set; }
        public IEnumerable<DocumentDeliveryMethod> DocumentDeliveryMethods { get; set; }
    }
}
