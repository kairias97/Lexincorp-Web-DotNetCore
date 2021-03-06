﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using LexincorpApp.Models.Validators;

namespace LexincorpApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [NoWhiteSpace(ErrorMessage ="El nombre de usuario no puede contener espacios en blanco")]
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        public virtual Attorney Attorney { get; set; }
        public bool Active { get; set; } = true;
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<NotificationAnswer> NotificationAnswers { get; set; }
    }
}
