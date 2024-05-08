namespace UsersStudentsAPIApp.Services.Exceptions
{
    public class TeacherAlreadyExistsException : Exception
    {
        public TeacherAlreadyExistsException(string s)
            : base(s)
        {
        }
    }
}
