using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class NewPackageViewModel
    {
        public Package Package { get; set; }
        public string ClientName { get; set; }
        public bool IsEnglish { get; set; }
        public bool IsClientSelected { get; set; }
        public bool OnlyAdminNotification { get; set; }
    }
}
