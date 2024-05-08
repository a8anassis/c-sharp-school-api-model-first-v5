using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.DTO;

namespace UsersStudentsAPIApp.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string username, string password);
        Task<User?> UpdateUserAsync(int userId, User request);
        Task<User?> GetByUsernameAsync(string username);
        Task<List<User>> GetAllUsersFilteredAsync(int pageNumber, int pageSize, List<Func<User, 
            bool>> predicates);
        Task<UserTeacherReadOnlyDTO?> GetUserTeacherInfoAync(string username);
    }
}
