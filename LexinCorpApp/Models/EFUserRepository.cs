using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFUserRepository : IUserRepository
    {
        private ApplicationDbContext context;
        public EFUserRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<User> Users { get => context.Users; }

        public void Save(User user)
        {
            if (user.UserId == 0)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();
        }

        public bool VerifyUsername(string username)
        {
            return !context.Users.Any(u => u.Username == username);
        }
    }
}
