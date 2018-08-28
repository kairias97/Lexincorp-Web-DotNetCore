using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private ApplicationDbContext context;
        public EFCategoryRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Category> Categories => context.Categories;

        public void Save(Category category)
        {
            if (category.Id == 0)
            {
                context.Categories.Add(category);
            } else
            {
                context.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }
    }
}
