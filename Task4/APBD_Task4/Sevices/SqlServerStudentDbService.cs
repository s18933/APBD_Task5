using APBD_Task4.DTOs.Requests;
using System;
using System.Data.SqlClient;

namespace APBD_Task4.Sevices
{

    public class SqlServerStudentDbService : IStudentsDbService
    {
        private string _connString = "Data Source=db-mssql;Initial Catalog=s18933;Integrated Security=True";
        public EnrollStudentRequest EnrollStudent(EnrollStudentRequest request)
        {
            if (request.FirstName == null || request.BirthDate == null || request.StudiesName == null)
            {
                return null;
            }

            var enrollment = new EnrollStudentRequest();
            using (var con = new SqlConnection(_connString))
            {
                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    using (var com = new SqlCommand())
                    {
                        int idStudy = 0;
                        int idEnrollment = 0;

                        com.Connection = con;
                        com.Transaction = tran;
                        com.CommandText = "select IdStudy from Studies where Name=@StudiesName";
                        com.Parameters.AddWithValue("StudiesName", request.StudiesName);



                        var dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            tran.Rollback();
                            return null;
                        }
                        else
                        {
                            idStudy = (int)dr["IdStudy"];
                            com.Parameters.AddWithValue("IdStudy", idStudy);
                        }

                        dr.Close();

                        com.CommandText = "Select IdEnrollment from Enrollment where Semester=1 AND IdStudy=@idStudy";

                        dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();

                            com.CommandText = "Select MAX(IdEnrollment) from Enrollment";
                            DateTime currentDate = DateTime.Now;
                            com.Parameters.AddWithValue("CurrentDate", currentDate);

                            var dr1 = com.ExecuteReader();
                            while (dr1.Read())
                            {
                                idEnrollment = int.Parse(dr1["IdEnrollment"].ToString()) + 1;
                                com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                            }
                            dr1.Close();

                            com.CommandText = "Insert into Enrollment(IdEnrollment, Semester, IdStudy, StartDate) values (@IdEnrollment, 1, @idStudy, @CurrentDate)";
                            com.ExecuteNonQuery();
                        }
                        else
                        {
                            idEnrollment = (int)dr["IdEnrollment"];
                            com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                            dr.Close();
                        }

                        com.CommandText = "Insert into Student(IndexNumber, FirstName, LastName, Birthdate, IdEnrollment) values (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";

                        string indexNumber = $"s{new Random().Next(1, 2000)}";
                        com.Parameters.AddWithValue("IndexNumber", indexNumber);
                        com.Parameters.AddWithValue("FirstName", request.FirstName);
                        com.Parameters.AddWithValue("LastName", request.LastName);
                        com.Parameters.AddWithValue("BirthDate", request.BirthDate);

                        dr.Close();
                        com.ExecuteNonQuery();

                        com.CommandText = "Select IndexNumber, FirstName, LastName, BirthDate, Name, Semester from Student, Studies, Enrollment " +
                            "where Enrollment.IdEnrollment = Student.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy " +
                            "AND IndexNumber = @IndexNumber;";

                        dr = com.ExecuteReader();
                        while (dr.Read())
                        {
                            enrollment.IndexNumber = dr["IndexNumber"].ToString();
                            enrollment.FirstName = dr["FirstName"].ToString();
                            enrollment.LastName = dr["LastName"].ToString();
                            enrollment.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                            enrollment.StudiesName = dr["Name"].ToString();
                            enrollment.Semester = Int32.Parse(dr["Semester"].ToString());
                        }
                        dr.Close();
                    }
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
                tran.Commit();
            }
            return enrollment;
        }

        public EnrollStudentRequest PromoteStudents(EnrollStudentRequest request)
        {
            var enrollment = new EnrollStudentRequest();
            using (var con = new SqlConnection(_connString))
            using (var com = new SqlCommand())

            {
                com.Connection = con;

                com.CommandText = "EXEC PromoteStudents @Name, @Semester"; 
                com.Parameters.AddWithValue("Name", request.StudiesName);
                com.Parameters.AddWithValue("Semester", request.Semester);

                con.Open();
                com.ExecuteNonQuery();

                com.CommandText = "Select IndexNumber, FirstName, LastName, BirthDate, Name, Semester from Student, Studies, Enrollment " +
                            "WHERE Enrollment.IdEnrollment = Student.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy " +
                            "AND Semester = (@Semester+1)";
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    enrollment.IndexNumber = dr["IndexNumber"].ToString();
                    enrollment.FirstName = dr["FirstName"].ToString();
                    enrollment.LastName = dr["LastName"].ToString();
                    enrollment.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                    enrollment.StudiesName = dr["Name"].ToString();
                    enrollment.Semester = Int32.Parse(dr["Semester"].ToString());
                }
                dr.Close();
            }
            return enrollment;

        }
    }
}
