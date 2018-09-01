using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IPackageRepository
    {
        IQueryable<Package> Packages { get; }
        void Save(Package package);
    }
}
