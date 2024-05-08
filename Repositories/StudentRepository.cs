using Microsoft.EntityFrameworkCore;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.Models;

namespace UsersStudentsAPIApp.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(UsersTeachersAPITestDbContext context)
            : base(context)
        {
        }

        public async Task<Student?> GetByPhoneNumber(string? phoneNumber)
        {
            return await _context.Students.Where(s => s.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync()!;  
        }

        public async Task<List<Course>> GetStudentCoursesAsync(int id)
        {
            List<Course> courses;
            courses = await _context.Students
                       .Where(s => s.Id == id)
                       .SelectMany(s => s.Courses!)
                       .ToListAsync();
            return  courses;
        }

        public async Task<List<User>> GetAllUsersStudentsAsync()
        {
            var usersWithStudentRole = await _context.Users
                   .Where(u => u.UserRole == UserRole.Student) 
                   .Include(u => u.Student) 
                   .ToListAsync();

            return usersWithStudentRole;
        }
    }
}
