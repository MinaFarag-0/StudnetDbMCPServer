# DBStudnetMCPServer

## MCP Server with MSSQL

### Tools
![image](https://github.com/user-attachments/assets/e38ef3c9-b313-444f-8b0a-ffe5963224cf)

`CreateStudent`
- Create New Student
- **Inputs**
  - `Student student`

`GetStudentById`
- Get Student By Id
- **Inputs**
  - `int studentId`

`GetAllStudents`
- Get All Students

`RemoveStudent`
- Remove Student By Id
- **Inputs**
  - `int studentId`

`CreateCourse`
- Create New Course
- **Inputs**
  - `Course course`

`EnrollStudentToCourse`
- Enroll Student To Course
- **Inputs**
  - `int studentId`
  - `int courseId`

`GetAllCourses`
- Get All Courses


## Usage with VS Code
```
{
    "inputs": [],
    "servers": {
        "StudentDbServer": {
                "type": "stdio",
                "command": "dotnet",
                "args": [
                    "run",
                    "--project",
                    "PATH_TO_YOUR_MCPSERVER_Project"
                ]
            }
    }
}
```
