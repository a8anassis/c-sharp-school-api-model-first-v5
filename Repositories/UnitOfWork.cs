using AutoMapper;
using UsersStudentsAPIApp.Data;

namespace UsersStudentsAPIApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsersTeachersAPITestDbContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(UsersTeachersAPITestDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public UserRepository UserRepository => new(_context, _mapper);
        public StudentRepository StudentRepository => new(_context);
        public TeacherRepository TeacherRepository => new(_context);
        public CourseRepository CourseRepository => new(_context);


        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
