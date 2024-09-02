namespace EmployeeManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public int Salary { get; set; }
        public int  DepartmentId { get; set; }

    }
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int ManagerId { get; set; }
    }
}

