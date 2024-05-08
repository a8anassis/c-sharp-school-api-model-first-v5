using System.ComponentModel.DataAnnotations.Schema;

namespace UsersStudentsAPIApp.Data
{
    public class Student
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string? Institution { get; set; }

        public int? UserId { get; set; }
        public virtual User? User { get; set; } = null!;

        public virtual ICollection<Course> Courses { get; } = new HashSet<Course>();

        public override string? ToString()
        {
            return $"{User!.Firstname}, {User.Lastname}, {Institution}";
        }
    }
}
