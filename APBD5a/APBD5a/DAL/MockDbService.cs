using APBD5a.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace APBD5a.DAL
{
    public class MockDbService : IDbService
    {
        private const string DbConnectionString = "Data Source=db-mssql;Initial Catalog=s17245;Integrated Security=True";

        public IEnumerable<Student> GetStudents()
        {

            var list = new List<Student>();

            using (SqlConnection connectionSql = new SqlConnection(DbConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connectionSql;
                sqlCommand.CommandText = "select * from student";

                connectionSql.Open();
                SqlDataReader readSql = sqlCommand.ExecuteReader();

                while (readSql.Read())
                {
                    var student = new Student();

                    student.IndexNumber = readSql["IndexNumber"].ToString();
                    student.FirstName = readSql["FirstName"].ToString();
                    student.LastName = readSql["LastName"].ToString();
                    student.BirthDate = readSql["BirthDate"].ToString().Substring(0,8); //Convert.ToDateTime(readSql["BirthDate"]);  
                    student.Studies = readSql["IdEnrollment"].ToString();

                    list.Add(student);
                }


            }
            return list;

        }

        public IEnumerable<Enrollment> GetEnrollment(string id)
        {

            var list = new List<Enrollment>();

            using (SqlConnection connectionSql = new SqlConnection(DbConnectionString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connectionSql;
                sqlCommand.CommandText = "SELECT firstname, LastName, BirthDate, st.Name, e.Semester FROM Student s JOIN Enrollment e on e.IdEnrollment=s.IdEnrollment JOIN studies st on st.IdStudy=e.IdStudy ORDER BY 1,2";
                sqlCommand.Parameters.AddWithValue("id", id);

                connectionSql.Open();
                SqlDataReader readSql = sqlCommand.ExecuteReader();

                while (readSql.Read())
                {
                    var enrollment = new Enrollment();

                    enrollment.IdEnrollment = Int32.Parse(readSql["IdEnrollment"].ToString());
                    enrollment.Semester = Int32.Parse(readSql["Semester"].ToString());
                    enrollment.IdStudy = Int32.Parse(readSql["IdStudy"].ToString());
                    enrollment.StartDate = readSql["StartDate"].ToString().Substring(0, 8);// int.Parse(readSql["StartDate"].ToString());//readSql["StartDate"].ToString().Substring(0, 8);

                    list.Add(enrollment);
                }

            }

            return list;
        }

    }


}