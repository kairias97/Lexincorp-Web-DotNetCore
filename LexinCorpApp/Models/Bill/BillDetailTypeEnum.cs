using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models { 

    public enum  BillDetailTypeEnum:int
    {
        Hours = 1,
        Package = 2,
        Retainer = 3,
        Item = 4, 
        RetainerDetail = 5,
        PackageDetail = 6
    }
}
