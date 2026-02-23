using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniERP.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }

        public int? WorkDay { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
