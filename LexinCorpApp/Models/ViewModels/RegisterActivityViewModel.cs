using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.ViewModels
{
    public class RegisterActivityViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<AttorneySelection> Attorneys { get; set; }
        public IEnumerable<Expense> Expenses { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public bool IsEnglish { get; set; }
        public bool IsClientSelected { get; set; }
        public string ClientName { get; set; }
        public decimal? FeePerHour { get; set; }
        public int ClientId { get; set; }
    }
}
