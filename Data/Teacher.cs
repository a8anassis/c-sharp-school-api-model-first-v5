namespace UsersStudentsAPIApp.Data
{
    public class Teacher
    {
        public int Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Institution { get; set; }

        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<Course> Courses { get; } = new HashSet<Course>();
    }
}
