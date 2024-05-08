using UsersStudentsAPIApp.Data;

namespace UsersStudentsAPIApp.Repositories
{
    public interface ITeacherRepository
    {
        Task<List<Course>> GetTeacherCoursesAsync(int id);
        Task<Teacher?> GetByPhoneNumber(string? phoneNumber);
        Task<List<User>> GetAllUsersTeachersAsync();
        Task<List<User>> GetAllUsersTeachersAsync(int pageNumber, int pageSize);
        Task<User?> GetTeacherByUsernameAync(string username);
    }
}
