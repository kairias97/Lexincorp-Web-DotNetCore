using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class NewAttorneyViewModel
    {
        public Attorney Attorney { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}
