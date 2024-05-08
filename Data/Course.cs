namespace UsersStudentsAPIApp.Data
{
    public class Course
    {
        public int Id { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Student> Students { get; } = new HashSet<Student>();

        public int TeacherId { get; set; }
        public virtual Teacher? Teacher { get; set; }
    }
}
