﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IBillingModeRepository
    {
        IQueryable<BillingMode> BillingModes { get; }
    }
}
