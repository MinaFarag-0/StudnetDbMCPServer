using Microsoft.Data.SqlClient;

namespace DBStudnetMCPServer
{
    public class DbHelper
    {
        // IMPORTANT: Replace with your actual SQL Server connection string
        private readonly string _connectionString = "Server=.;Database=StudentCoursesDb;Integrated Security=True;TrustServerCertificate=True;";

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DbHelper()
        {

        }

        // --- Student Operations ---

        public void CreateStudent(Student student)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Students (FirstName, LastName, DateOfBirth, Email) VALUES (@FirstName, @LastName, @DateOfBirth, @Email); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth.HasValue ? (object)student.DateOfBirth.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(student.Email) ? DBNull.Value : (object)student.Email);

                    connection.Open();
                    student.StudentID = Convert.ToInt32(command.ExecuteScalar());
                    Console.WriteLine($"Student '{student.FirstName} {student.LastName}' created with ID: {student.StudentID}");
                }
            }
        }

        public Student GetStudentById(int studentId)
        {
            Student student = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT StudentID, FirstName, LastName, DateOfBirth, Email FROM Students WHERE StudentID = @StudentID;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new Student
                            {
                                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
                            };
                        }
                    }
                }
            }
            return student;
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT StudentID, FirstName, LastName, DateOfBirth, Email FROM Students ORDER BY LastName, FirstName;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student
                            {
                                StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
                            });
                        }
                    }
                }
            }
            return students;
        }

        public void RemoveStudent(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // First, remove related entries in StudentCourses
                string deleteStudentCoursesSql = "DELETE FROM StudentCourses WHERE StudentID = @StudentID;";
                using (SqlCommand command = new SqlCommand(deleteStudentCoursesSql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Removed student's course enrollments for StudentID: {studentId}");
                }

                // Then, remove the student
                string deleteStudentSql = "DELETE FROM Students WHERE StudentID = @StudentID;";
                using (SqlCommand command = new SqlCommand(deleteStudentSql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    // Connection is already open from the previous command
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Student with ID {studentId} removed successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"No student found with ID {studentId}.");
                    }
                }
            }
        }

        // --- Course Operations ---

        public void CreateCourse(Course course)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO Courses (CourseName, CourseCode, Credits) VALUES (@CourseName, @CourseCode, @Credits); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CourseName", course.CourseName);
                    command.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                    command.Parameters.AddWithValue("@Credits", course.Credits);

                    connection.Open();
                    course.CourseID = Convert.ToInt32(command.ExecuteScalar());
                    Console.WriteLine($"Course '{course.CourseName}' created with ID: {course.CourseID}");
                }
            }
        }

        public List<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = "SELECT CourseID, CourseName, CourseCode, Credits FROM Courses ORDER BY CourseName;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courses.Add(new Course
                            {
                                CourseID = reader.GetInt32(reader.GetOrdinal("CourseID")),
                                CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
                                CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
                                Credits = reader.GetInt32(reader.GetOrdinal("Credits"))
                            });
                        }
                    }
                }
            }
            return courses;
        }

        // --- Enrollment Operation ---

        public void EnrollStudentToCourse(int studentId, int courseId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Check if enrollment already exists to prevent duplicate primary key
                string checkSql = "SELECT COUNT(*) FROM StudentCourses WHERE StudentID = @StudentID AND CourseID = @CourseID;";
                using (SqlCommand checkCommand = new SqlCommand(checkSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@StudentID", studentId);
                    checkCommand.Parameters.AddWithValue("@CourseID", courseId);
                    connection.Open();
                    int existingEnrollments = (int)checkCommand.ExecuteScalar();

                    if (existingEnrollments > 0)
                    {
                        Console.WriteLine($"Student ID {studentId} is already enrolled in Course ID {courseId}.");
                        return;
                    }
                }

                // If not enrolled, proceed with insertion
                string enrollSql = "INSERT INTO StudentCourses (StudentID, CourseID, EnrollmentDate) VALUES (@StudentID, @CourseID, GETDATE());";
                using (SqlCommand command = new SqlCommand(enrollSql, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentId);
                    command.Parameters.AddWithValue("@CourseID", courseId);

                    // Connection is already open from the check command
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Student ID {studentId} successfully enrolled in Course ID {courseId}.");
                }
            }
        }
    }
}