using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IAttorneyRepository
    {
        IQueryable<Attorney> Attorneys { get; }
        void Save(Attorney attorney);
    }
}
