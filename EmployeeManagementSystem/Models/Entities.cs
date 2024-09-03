namespace EmployeeManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
        public string Position { get; set; }
        public string Salary { get; set; }
        public int DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
   
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int ManagerId { get; set; }
    }
}

