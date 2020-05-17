using APBD5a.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD5a.DTOs.Responses
{
    public class PromoteStudentResponse
    {
        public Enrollment enrollment { get; set; }
        public string dtoResponse { get; set; }
    }
}