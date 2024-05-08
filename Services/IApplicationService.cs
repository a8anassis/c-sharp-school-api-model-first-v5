namespace UsersStudentsAPIApp.Services
{ 
    public interface IApplicationService
    {
        UserService UserService { get; }
        StudentService StudentService { get; }
        TeacherService TeacherService { get; }
        // CourseService { get; }
    }
}
