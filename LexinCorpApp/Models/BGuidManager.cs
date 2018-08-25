using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class BGuidManager: IGuidManager
    {
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
