using UsersStudentsAPIApp.Models;

namespace UsersStudentsAPIApp.Data
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public UserRole? UserRole { get; set; }

        public virtual Student? Student { get; set; }
        public virtual Teacher? Teacher { get; set; }

        public override string? ToString()
        {
            return $"{Username}, {Firstname}, {Lastname}";
        }
    }
}
