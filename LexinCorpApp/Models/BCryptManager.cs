using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt;

namespace LexincorpApp.Models
{
    public class BCryptManager: ICryptoManager
    {
        public string HashString(string plainTextPwd)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainTextPwd);
        }
        public bool VerifyHash(string plainText, string HashText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, HashText);
        }
    }
}
