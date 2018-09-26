using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IActivityRepository
    {
        IQueryable<Activity> Activities { get; }
        void Save(NewActivityRequest newActivityRequest, int creatorId);
    }
}
