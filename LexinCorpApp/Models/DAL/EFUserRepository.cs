﻿using System;
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
            } else
            {
                //context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.Attach(user).Context.Entry(user).Property(x => x.IsAdmin).IsModified = true;
            }
            context.SaveChanges();
        }

        public bool VerifyUsername(string username)
        {
            return !context.Users.Any(u => u.Username == username);
        }

        public bool VerifyAttorneyIDAndUsername(int attorneyID, int userID)
        {
            return context.Users.Any(u => u.Attorney.AttorneyId == attorneyID && u.UserId == userID);
        }
    }
}