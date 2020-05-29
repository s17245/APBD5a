using APBD5a.DTOs.Requests;
using APBD5a.DTOs.Responses;
using APBD5a.Models;
using Microsoft.IdentityModel.Tokens;
using SimpleCrypto;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APBD5a.Service
{
    public class SqlServerDbService : IStudentDbService
    {
        public IConfiguration Configuration { get; }
        public SqlServerDbService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17245;Integrated Security=True";
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response = new EnrollStudentResponse();

            response.IndexNumber = request.IndexNumber;
            Enrollment enrollment = new Enrollment();

            using (SqlConnection connectionSql = new SqlConnection(ConString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connectionSql;
                connectionSql.Open();

                var transaction = connectionSql.BeginTransaction();

                try
                {
                    sqlCommand.CommandText = "select idstudy from studies where name=@name";
                    sqlCommand.Parameters.AddWithValue("name", request.Studies);

                    sqlCommand.Transaction = transaction;

                    var readIdStudy = sqlCommand.ExecuteReader();

                    if (!readIdStudy.Read())
                    {
                        response.dtoResponse = "nie ma takich studiów";
                        return response;
                    }

                    int idStudy = (int)readIdStudy["IdStudy"];
                    readIdStudy.Close();

                    sqlCommand.CommandText = "select IdEnrollment from Enrollment where IdEnrollment >= (select max(IdEnrollment) from Enrollment)";

                    var maxEnrollment = sqlCommand.ExecuteReader();

                    if (!maxEnrollment.Read()) { }

                    int newEnrollment = (int)maxEnrollment["IdEnrollment"] + 1;
                    maxEnrollment.Close();

                    sqlCommand.CommandText = "select idEnrollment, StartDate from Enrollment WHERE idStudy=@idStudy AND Semester=1 ORDER BY StartDate";
                    sqlCommand.Parameters.AddWithValue("idStudy", idStudy);

                    DateTime dateNow;

                    var readEnrollment = sqlCommand.ExecuteReader();

                    if (readEnrollment.Read())
                    {
                        newEnrollment = (int)readEnrollment["IdEnrollment"];
                        dateNow = (DateTime)readEnrollment["StartDate"];
                        readEnrollment.Close();
                    }
                    else
                    {

                        response.dtoResponse = "nie ma takiej rekrutacji";

                        dateNow = DateTime.Now;

                        sqlCommand.CommandText = "insert into Enrollment values(@id, @Semester, @IdStud, @StartDate)";
                        sqlCommand.Parameters.AddWithValue("id", newEnrollment);
                        sqlCommand.Parameters.AddWithValue("Semester", 1);
                        sqlCommand.Parameters.AddWithValue("IdStud", idStudy);
                        sqlCommand.Parameters.AddWithValue("StartDate", dateNow);

                        readEnrollment.Close();
                        sqlCommand.ExecuteNonQuery();
                    }

                    enrollment.IdEnrollment = newEnrollment;
                    enrollment.Semester = 1;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = dateNow.ToString();

                    response.enrollment = enrollment;

                    sqlCommand.CommandText = "select IndexNumber FROM Student WHERE IndexNumber=@indexNumber";
                    sqlCommand.Parameters.AddWithValue("indexNumber", request.IndexNumber);

                    DateTime birthDateTime = Convert.ToDateTime(request.BirthDate);
                    string formattedDate = birthDateTime.ToString("yyyy-MM-dd HH:mm");

                    try
                    {
                        sqlCommand.CommandText = "insert into Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@IndexN, @FirstN, @LastN, @BirthD, @IdEnrollm)";

                        sqlCommand.Parameters.AddWithValue("IndexN", request.IndexNumber);
                        sqlCommand.Parameters.AddWithValue("FirstN", request.FirstName);
                        sqlCommand.Parameters.AddWithValue("LastN", request.LastName);
                        sqlCommand.Parameters.AddWithValue("BirthD", formattedDate);
                        sqlCommand.Parameters.AddWithValue("IdEnrollm", newEnrollment);

                        response.dtoResponse = "Done";
                        sqlCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        response.dtoResponse = "Student z takim numerem indeksu istnieje już w bazie.";
                        response.dtoResponse = ex.Message;
                    }
                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    response.dtoResponse = exc.Message;
                }
                return response;
            }
        }

        public PromoteStudentResponse PromoteStudents(PromoteStudentRequest request)
        {
            PromoteStudentResponse response = new PromoteStudentResponse();

            using (SqlConnection connectionSql = new SqlConnection(ConString))
            using (var sqlCommand = new SqlCommand("PromoteStudents", connectionSql))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@Studies", request.Studies);
                sqlCommand.Parameters.AddWithValue("@semester", request.Semester);

                connectionSql.Open();
                sqlCommand.ExecuteNonQuery();
                connectionSql.Close();
                response.dtoResponse = "Done";

                SqlCommand command = new SqlCommand();
                command.Connection = connectionSql;
                connectionSql.Open();

                int newSemester = request.Semester + 1;

                command.CommandText = "SELECT IdStudy FROM Studies where Name=@name";
                command.Parameters.AddWithValue("name", request.Studies);

                var idStudies = command.ExecuteReader();

                if (!idStudies.Read())
                {
                    response.dtoResponse = "Podanych studia nie istnieją";
                    return response;
                }

                int idstudies = (int)idStudies["IdStudy"];
                idStudies.Close();

                Enrollment enrollment = new Enrollment();

                command.CommandText = "SELECT idEnrollment,idStudy,Semester,StartDate FROM Enrollment WHERE idStudy=@idStudy AND Semester=@Semester";
                command.Parameters.AddWithValue("idStudy", idstudies);
                command.Parameters.AddWithValue("Semester", newSemester);

                var newEnrollSemester = command.ExecuteReader();

                if (newEnrollSemester.Read())
                {
                    enrollment.IdEnrollment = (int)newEnrollSemester["IdEnrollment"];
                    enrollment.IdStudy = (int)newEnrollSemester["IdStudy"];
                    enrollment.Semester = (int)newEnrollSemester["Semester"];
                    enrollment.StartDate = (String)newEnrollSemester["StartDate"];
                }

                newEnrollSemester.Close();
                response.enrollment = enrollment;

                return response;
            }
        }


        public Student GetStudent(string index)
        {
            using (SqlConnection connectionSql = new SqlConnection(ConString))
            using (SqlCommand sqlCommand = new SqlCommand())
            {
                sqlCommand.Connection = connectionSql;
                sqlCommand.CommandText = "SELECT IndexNumber from Student WHERE IndexNumber=@Index";
                sqlCommand.Parameters.AddWithValue("Index", index);

                connectionSql.Open();

                var student = new Student();

                SqlDataReader sqlRead = sqlCommand.ExecuteReader();

                if (sqlRead.Read())
                {
                    student.IndexNumber = sqlRead["IndexNumber"].ToString();
                    student.FirstName = sqlRead["FirstName"].ToString();
                    student.LastName = sqlRead["LastName"].ToString();
                    student.BirthDate = sqlRead["BirthDate"].ToString().Substring(0, 8);
                    student.Studies = sqlRead["IdEnrollment"].ToString();
                    return student;

                }
                else
                {
                    return null;
                }
            }
        }

        Enrollment IStudentDbService.PromoteStudents(int semester, string studies)
        {
            using (var conn = new SqlConnection(ConString))
            using (var command = new SqlCommand("PromoteStudents", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.Add(new SqlParameter("@Studies", studies));
                command.Parameters.Add(new SqlParameter("@Semester", semester));

                var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                conn.Open();
                command.ExecuteNonQuery();
                var result = returnParameter.Value;
                System.Diagnostics.Debug.WriteLine("home " + result);

                conn.Close();

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                conn.Open();

                com.CommandText = "select idEnrollment,idStudy,Semester,StartDate from Enrollment where idEnrollment=@idEnrollment";
                com.Parameters.AddWithValue("idEnrollment", result);


                Enrollment enrollment = new Enrollment();
                var enroll = com.ExecuteReader();
                if (enroll.Read())
                {
                    enrollment.IdEnrollment = (int)enroll["IdEnrollment"];
                    enrollment.IdStudy = (int)enroll["IdStudy"];
                    enrollment.Semester = (int)enroll["Semester"];
                    enrollment.StartDate = enroll["StartDate"].ToString();
                    enroll.Close();
                }
                else
                {
                    enroll.Close();
                }
                conn.Close();
                return enrollment;
            }
        }

        public LoginResponse LoginStudent(string login, string haslo)
        {
            ICryptoService cryptoService = new PBKDF2();

            var st = new Student();
            var resp = new LoginResponse();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber,Password,salt from Student WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", login);

                con.Open();
                SqlDataReader sqlRead = com.ExecuteReader();
                if (sqlRead.Read())
                {
                    st.IndexNumber = sqlRead["IndexNumber"].ToString();
                    string BaseSalt = sqlRead["salt"].ToString();
                    string password = sqlRead["Password"].ToString();
                    string hasloLocal = cryptoService.Compute(haslo, BaseSalt);
                    bool isPasswordValid = cryptoService.Compare(password, hasloLocal);
                    if (!isPasswordValid)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                con.Close();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, st.IndexNumber),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                resp.accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                resp.refreshToken = Guid.NewGuid();

                con.Open();
                com.CommandText = "UPDATE Student SET refreshToken=@Refresh WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Refresh", resp.refreshToken);

                com.ExecuteNonQuery();
                con.Close();

            }

            return resp;
        }

        public LoginResponse RefreshToken(string refToken)
        {
            var st = new Student();
            var resp = new LoginResponse();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber from Student WHERE refreshToken=@refToken";
                com.Parameters.AddWithValue("refToken", refToken);

                con.Open();
                SqlDataReader sqlRead = com.ExecuteReader();
                if (sqlRead.Read())
                {
                    st.IndexNumber = sqlRead["IndexNumber"].ToString();
                }
                else
                {
                    return null;
                }
                con.Close();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, st.IndexNumber),
                    new Claim(ClaimTypes.Role, "student")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                (
                    issuer: "Gakko",
                    audience: "Students",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds
                );

                resp.accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                resp.refreshToken = Guid.NewGuid();

                con.Open();
                com.CommandText = "UPDATE Student SET refreshToken=@Refresh WHERE IndexNumber=@Index";
                com.Parameters.AddWithValue("Index", st.IndexNumber);
                com.Parameters.AddWithValue("Refresh", resp.refreshToken);

                com.ExecuteNonQuery();
                con.Close();

            }

            return resp;
        }

        Enrollment IStudentDbService.PromoteStudents(PromoteStudentRequest request)
        {
            throw new NotImplementedException();
        }
    }


}