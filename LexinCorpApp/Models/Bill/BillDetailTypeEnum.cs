using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models { 

    public enum  BillDetailTypeEnum:int
    {
        RetainerHeader = 1,
        PackageHeader = 2,
        RetainerDetail = 3,
        PackageDetail = 4,
        Item = 5,
        Hours = 6
    }
}
