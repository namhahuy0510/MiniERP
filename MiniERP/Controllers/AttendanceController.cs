using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniERP.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class AttendanceController : Controller
    {
        private readonly MiniERPContext _context;

        public AttendanceController(MiniERPContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? date)
        {
            var queryDate = date ?? DateTime.Today;

            var attendances = await _context.Attendance
                .Include(a => a.Employee)
                .Where(a => a.Date == queryDate)
                .ToListAsync();

            ViewBag.Date = queryDate;
            ViewBag.Employees = await _context.Employees
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return View(attendances);
        }

        [HttpPost]
        public async Task<IActionResult> Tick(int employeeId, bool isPresent)
        {
            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                Date = DateTime.Today,
                IsPresent = isPresent,
                CheckInTime = isPresent ? DateTime.Now : null,
                CheckOutTime = null
            };

            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int attendanceId)
        {
            var record = await _context.Attendance.FindAsync(attendanceId);
            if (record != null && record.IsPresent)
            {
                record.CheckOutTime = DateTime.Now;
                _context.Update(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
