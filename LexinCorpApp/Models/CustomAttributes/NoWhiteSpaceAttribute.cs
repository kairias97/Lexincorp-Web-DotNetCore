using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexincorpApp.Models.Validators
{
    public class NoWhiteSpaceAttribute : ValidationAttribute
    {

        public NoWhiteSpaceAttribute()
        {
           
        }
        

        private string GetErrorMessage(string memberName)
        {
            return $"No se permiten espacios en blanco en propiedad {memberName}";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.ToString().Contains(" "))
            {
                return new ValidationResult(this.ErrorMessage ?? GetErrorMessage(validationContext.MemberName));
            }
            return ValidationResult.Success;
        }
        
    }
}
