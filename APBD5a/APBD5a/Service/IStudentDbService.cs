using APBD5a.DTOs.Requests;
using APBD5a.DTOs.Responses;
using APBD5a.Models;

namespace APBD5a.Service
{
    public interface IStudentDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);

        Enrollment PromoteStudents(PromoteStudentRequest request);

        Student GetStudent(string index);

        LoginResponse LoginStudent(string login, string haslo);
        LoginResponse RefreshToken(string refToken);
        Enrollment PromoteStudents(int semester, string studies);
    }
}