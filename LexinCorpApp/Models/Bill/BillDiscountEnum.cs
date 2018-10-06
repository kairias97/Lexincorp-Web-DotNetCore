using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public enum BillDiscountEnum: int
    {
        [Display(Name = "Porcentual")]
        Percentage = 1,
        [Display(Name = "Neto")]
        Net = 2
    }
}
