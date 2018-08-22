using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFExpenseRepository : IExpenseRepository
    {
        private ApplicationDbContext context;

        public EFExpenseRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Expense> Expenses => context.Expenses;

        public void Save(Expense expense)
        {
            if (expense.Id == 0)
            {
                context.Expenses.Add(expense);
            } else
            {
                context.Entry(expense).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
