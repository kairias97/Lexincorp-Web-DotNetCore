using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        public virtual Attorney Attorney { get; set; }
    }
}
