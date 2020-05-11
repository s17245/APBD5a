using System;
using System.ComponentModel.DataAnnotations;

namespace APBD5a.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        //[RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage ="Musisz podać imię")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        
        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public string Studies { get; set; }
    }
}
