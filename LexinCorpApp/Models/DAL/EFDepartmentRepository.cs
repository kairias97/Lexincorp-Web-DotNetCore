using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFDepartmentRepository:IDepartmentRepository
    {
        private ApplicationDbContext context;

        public EFDepartmentRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<Department> Departments { get => context.Departments; }
    }
}
