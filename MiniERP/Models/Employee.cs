namespace MiniERP.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } = string.Empty ;
        public decimal Salary   { get; set; }


        public int DepartmentId { get; set; }
        public Department Department { get; set; } = new Department();

    }
}
