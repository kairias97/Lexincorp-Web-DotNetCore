using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IExpenseRepository
    {
        IQueryable<Expense> Expenses { get; }
        void Save(Expense expense);
    }
}
