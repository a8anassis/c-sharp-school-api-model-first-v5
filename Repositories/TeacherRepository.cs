using Microsoft.EntityFrameworkCore;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.Models;

namespace UsersStudentsAPIApp.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(UsersTeachersAPITestDbContext context)
            : base(context)
        {
        }

        public async Task<Teacher?> GetByPhoneNumber(string? phoneNumber)
        {
            return await _context.Teachers.Where(s => s.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync()!;
        }

        public async Task<List<Course>> GetTeacherCoursesAsync(int id)
        {
            List<Course> courses;
            courses = await _context.Teachers
                       .Where(t => t.Id == id)
                       .SelectMany(t => t.Courses!)
                       .ToListAsync();
            return courses;
        }

        public async Task<List<User>> GetAllUsersTeachersAsync()
        {
            var usersWithTeacherRole = await _context.Users
                   .Where(u => u.UserRole == UserRole.Teacher) 
                   .Include(u => u.Teacher) // Teacher is the navigation property
                   .ToListAsync();

            return usersWithTeacherRole;
        }

        public async Task<List<User>> GetAllUsersTeachersAsync(int pageNumber, int pageSize)
        {
            int skip = pageSize * pageNumber;
            var usersWithTeacherRole = await _context.Users
                   .Where(u => u.UserRole == UserRole.Teacher)
                   .Include(u => u.Teacher) // Teacher is the navigation property
                   .Skip(skip)
                   .Take(pageSize)
                   .ToListAsync();
            
            return usersWithTeacherRole;
        }

        public async Task<User?> GetTeacherByUsernameAync(string username)
        {
            var userTeacher = await _context.Users
            .Where(u => u.Username == username && u.UserRole == UserRole.Teacher)  // Assuming "User" is the navigation property from Student to User, and "Username" is the property to filter by
            .SingleOrDefaultAsync();

            return userTeacher;
        }
    }
}
