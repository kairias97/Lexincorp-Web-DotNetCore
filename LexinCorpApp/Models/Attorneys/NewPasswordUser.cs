using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models
{
    public class NewPasswordUser
    {
        [Required(ErrorMessage = "Contraseña actual es requerida")]
        public string oldPassword { get; set; }
        [Required(ErrorMessage = "Contraseña nueva es requerida")]
        public string newPassword { get; set; }
        [Required(ErrorMessage = "Confirmación de nueva contraseña es requerida")]
        public string confirmNewPassword { get; set; }
    }
}
