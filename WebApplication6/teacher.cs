using System;


namespace WebApplication6
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public DateTime HireDate { get; set; }
        public string Department { get; set; }
        public string? Photo { get; internal set; }
    }
}
