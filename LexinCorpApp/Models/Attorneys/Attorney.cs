using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace LexincorpApp.Models
{
    public class Attorney
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del abogado es requerido")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La fecha de admisión es requerida")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AdmissionDate { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string IdentificationNumber { get; set; }
        [Required]
        [Phone]
        public string AssignedPhoneNumber { get; set; }
        [Required]
        [Phone]
        public string PersonalPhoneNumber { get; set; }
        [Required]
        public string EmergencyContact { get; set; }
        [Required]
        [Phone]
        public string EmergencyContactPhoneNumber { get; set; }
        [Required]
        public string NotaryCode { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email es invalido")]
        public string Email { get; set; }
        public decimal VacationCount { get; set; }
        public bool CanApproveVacations { get; set; }
        [NotMapped]
        public decimal AvailableVacationCount { get; set; }

        /*protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Attorney attorney = (Attorney)validationContext.ObjectInstance;
            if(attorney.User.Username.Contains(" "))
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }
        private string GetErrorMessage()
        {
            return $"El nombre de usuario ingresado contiene espacios en blanco, favor revisar.";
        }*/
    }
}
