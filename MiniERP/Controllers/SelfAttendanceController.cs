using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;
using System.Security.Claims;

namespace MiniERP.Controllers
{
    [Authorize]
    public class SelfAttendanceController : Controller
    {
        private readonly MiniERPContext _context;

        public SelfAttendanceController(MiniERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp == null)
            {
                return View("NoEmployeeProfile");
            }

            var today = DateTime.Today;

            var todayAttendance = await _context.Attendance
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.Date == today);

            return View((emp, todayAttendance));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp == null) return RedirectToAction(nameof(Index));

            var today = DateTime.Today;
            var record = await _context.Attendance
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.Date == today);

            if (record == null)
            {
                record = new Attendance
                {
                    EmployeeId = emp.Id,
                    Date = today,
                    IsPresent = true,
                    CheckInTime = DateTime.Now
                };
                _context.Attendance.Add(record);
            }
            else if (!record.IsPresent)
            {
                record.IsPresent = true;
                record.CheckInTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (emp == null) return RedirectToAction(nameof(Index));

            var today = DateTime.Today;
            var record = await _context.Attendance
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.Date == today && a.IsPresent);

            if (record != null && record.CheckOutTime == null)
            {
                record.CheckOutTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

