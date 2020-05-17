using System;
using APBD5a.Models;

namespace APBD5a.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }

        public string IndexNumber { get; set; }
        
        public Enrollment enrollment { get; set; }
        
        public string dtoResponse { get; set; }
    }
}
