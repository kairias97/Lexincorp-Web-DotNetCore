﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public enum VacationMovementEnum
    {
        [Display(Name = "Acreditar")]
        Credit = 1,
        [Display(Name = "Descontar")]
        Debit = 2
    }
}
