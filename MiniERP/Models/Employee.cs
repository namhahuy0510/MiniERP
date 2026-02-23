using System.ComponentModel.DataAnnotations.Schema;

namespace MiniERP.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Position { get; set; }

        [Column(TypeName = "decimal(12,4)")]
        public decimal Salary { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
