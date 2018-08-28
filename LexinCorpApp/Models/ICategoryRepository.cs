using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Categories { get; }
        void Save(Category category);
    }
}
