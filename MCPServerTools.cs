using DBStudnetMCPServer;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public static class StudentMCPServer
{
    private static DbHelper _dbHelper = new DbHelper();

    [McpServerTool(Name = "create_new_student"), Description("Create New Student")]
    public static void CreateStudent(Student student) => _dbHelper.CreateStudent(student);

    [McpServerTool, Description("Get Student By Id")]
    public static Student GetStudentById(int studentId) => _dbHelper.GetStudentById(studentId);

    [McpServerTool, Description("Get All Students")]
    public static List<Student> GetAllStudents() => _dbHelper.GetAllStudents();

    [McpServerTool, Description("Remove Student By Id")]
    public static void RemoveStudent(int studentId) => _dbHelper.RemoveStudent(studentId);

    [McpServerTool, Description("Create New Course")]
    public static void CreateCourse(Course course) => _dbHelper.CreateCourse(course);

    [McpServerTool, Description("Enroll Student To Course")]
    public static void EnrollStudentToCourse(int studentId, int courseId) => _dbHelper.EnrollStudentToCourse(studentId, courseId);

    [McpServerTool, Description("Get All Courses")]
    public static List<Course> GetAllCourses() => _dbHelper.GetAllCourses();
}
