using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IServiceRepository
    {
        IQueryable<Service> Services { get; }
        void Save(Service service);
    }
}
