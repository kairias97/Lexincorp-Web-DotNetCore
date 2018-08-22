using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class EFAttorneyRepository: IAttorneyRepository
    {
        private ApplicationDbContext context;
        public EFAttorneyRepository(ApplicationDbContext ctx)
        {
            this.context = ctx;
        }
        public IQueryable<Attorney> Attorneys { get => context.Attorneys; }
        public void Save(Attorney attorney)
        {
            if (attorney.AttorneyId == 0)
            {
                context.Attorneys.Add(attorney);
            } else
            {
                context.Entry(attorney).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            context.SaveChanges();
        }

        public bool VerifyAttorneyIDAndEmailOwnership(int attorneyID, string email)
        {
            return context.Attorneys.Any(a => a.AttorneyId == attorneyID && a.Email == email);
        }

        public bool verifyAttorneyIDAndNotaryCodeOwnership(int attorneyID, string notaryCode)
        {
            return context.Attorneys.Any(a => a.AttorneyId == attorneyID && a.NotaryCode == notaryCode);
        }

        public bool VerifyEmail(string email)
        {
            return !context.Attorneys.Any(a => a.Email == email);
        }

        public bool VerifyNotaryCode(string notaryCode)
        {
            return !context.Attorneys.Any(a => a.NotaryCode == notaryCode);
            
        }
    }
}
