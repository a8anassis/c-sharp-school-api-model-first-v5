namespace UsersStudentsAPIApp.Models
{
    public class Error
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public string? Field { get; set; }

        public Error()
        {

        }

        public Error(string? code, string? message, string? field)
        {
            Code = code;
            Message = message;
            Field = field;
        }
    }
}
