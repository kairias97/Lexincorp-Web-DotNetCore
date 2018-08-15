using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LexinCorpApp.Models
{
    public class ClientType
    {
        
        public int ClientTypeId { get; set; }
        public string Name { get; set; }
        public ICollection<Client> Clients { get; set; }
    }
}
