using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class EditServiceViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public Service Service { get; set; }
    }
}
