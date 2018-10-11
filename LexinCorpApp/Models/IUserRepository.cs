using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }
        void Save(User user);
        bool VerifyUsername(string username);
        bool VerifyAttorneyIDAndUsername(int attorneyID, int userID);
        User GetUserByUsername(string username);
        void UpdateUserPassword(User user);
    }
}
