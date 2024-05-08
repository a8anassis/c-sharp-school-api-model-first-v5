using Microsoft.EntityFrameworkCore;
using UsersStudentsAPIApp.Data;

namespace UsersStudentsAPIApp.Repositories
{
    public class CourseRepository : BaseRepository<Course>, ICourseRepository
    {
        public CourseRepository(UsersTeachersAPITestDbContext context)
            : base(context)
        {
        }

        public async Task<List<Student>> GetCourseStudentsAsync(int id)
        {
            List<Student> courses;
            courses = await _context.Courses
                       .Where(c => c.Id == id)
                       .SelectMany(c => c.Students!)
                       .ToListAsync();
            return courses;
        }

        public async Task<Teacher?> GetCourseTeacherAsync(int id)
        {
            var course = await _context.Courses
                            .Where(c => c.Id == id)
                            .FirstOrDefaultAsync();

            if (course == null)
            {
                return null;
            }

            if (course.Teacher is null)
            {
                return null;
            }

            return course.Teacher;         
        }
    }
}
