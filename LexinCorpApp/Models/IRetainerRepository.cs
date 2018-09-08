using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IRetainerRepository
    {
        IQueryable<Retainer> Retainers { get;}
        void Save(Retainer retainer);
    }
}
