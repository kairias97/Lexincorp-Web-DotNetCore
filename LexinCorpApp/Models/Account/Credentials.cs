using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class Credentials
    {
        [Required (ErrorMessage = "Debe ingresar un serial")]
        public string Username { get; set; }
        [Required (ErrorMessage = "Debe ingresar su contraseña")]
        public string Password { get; set; }
    }
}
