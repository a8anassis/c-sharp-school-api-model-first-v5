namespace UsersStudentsAPIApp.Repositories
{
    public interface IUnitOfWork
    {
        public UserRepository UserRepository { get; }
        public StudentRepository StudentRepository { get; }
        public TeacherRepository TeacherRepository { get; }
        public CourseRepository CourseRepository { get; }

        Task<bool> SaveAsync();
    }
}
