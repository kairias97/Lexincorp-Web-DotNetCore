using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LexincorpApp.Models
{
    public class Attorney
    {
        public int AttorneyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
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
    }
}
