﻿using APBD5a.DTOs.Requests;
using APBD5a.DTOs.Responses;
using APBD5a.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD5a.Service
{
    public interface IStudentDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);

        PromoteStudentResponse PromoteStudents(PromoteStudentRequest request);

        Student GetStudent(string index);
        
    }
}