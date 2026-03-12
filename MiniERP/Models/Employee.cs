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

        public ICollection<Attendance>? Attendances { get; set; }

        public bool? IsPresent { get; set; }
        public string? AvatarUrl { get; set; }

        // Các cột này trong DB có thể NULL → phải để nullable
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateJoined { get; set; }

        public string? UserId { get; set; }
    }
}
