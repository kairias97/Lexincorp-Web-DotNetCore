using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public enum ActivityTypeEnum: int
    {
        [Display(Name = "Hora")]
        Hourly = 1,
        [Display(Name = "Paquete")]
        Package = 2,
        [Display(Name = "Retainer")]
        Retainer = 3,
        [Display(Name = "Item")]
        Item = 4
    }
}
