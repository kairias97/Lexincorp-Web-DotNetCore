using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface IAttorneyRepository
    {
        IQueryable<Attorney> Attorneys { get; }
        void Save(Attorney attorney);
        bool VerifyEmail(string email);
        bool VerifyNotaryCode(string notaryCode);
        bool VerifyAttorneyIDAndEmailOwnership(int attorneyID, string email);
        bool verifyAttorneyIDAndNotaryCodeOwnership(int attorneyID, string notaryCode);
    }
}
