using UsersStudentsAPIApp.Models;

namespace UsersStudentsAPIApp.DTO
{
    public class UserFiltersDTO
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public UserRole? UserRole { get; set; }
    }
}
