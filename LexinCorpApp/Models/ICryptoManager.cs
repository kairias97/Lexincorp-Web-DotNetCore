using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public interface ICryptoManager
    {
        string HashString(string plainTextPwd);
        bool VerifyHash(string plainText, string hashedText);
    }
}
