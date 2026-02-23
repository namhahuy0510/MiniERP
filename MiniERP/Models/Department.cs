namespace MiniERP.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;


        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
