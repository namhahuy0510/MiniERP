namespace MiniERP.Models
{
    public class EmployeeManagementModel
    {
        public Employee Employee { get; set; } = new Employee();
        public List<Employee> EmployeeList { get; set; } = new List<Employee>();
        public List<Department> DepartmentList { get; set; } = new List<Department>();
    }
}
