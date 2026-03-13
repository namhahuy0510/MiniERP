using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace MiniERP.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly MiniERPContext _context;

        public EmployeeController(MiniERPContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult EmployeeManagement()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportEmployees(string format)
        {
            var employees = _context.Employees.ToList();
            format = (format ?? "").ToLowerInvariant();

            string fileNameBase = $"employees_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (format == "json")
            {
                var json = System.Text.Json.JsonSerializer.Serialize(employees, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", $"{fileNameBase}.json");
            }

            // Common text representation
            var lines = new List<string>
            {
                "Id\tFullName\tPosition\tSalary\tStartDate\tEndDate"
            };
            foreach (var e in employees)
            {
                lines.Add($"{e.Id}\t{e.FullName}\t{e.Position}\t{e.Salary}\t{e.StartDate:yyyy-MM-dd}\t{e.EndDate:yyyy-MM-dd}");
            }
            var content = string.Join(Environment.NewLine, lines);

            if (format == "md")
            {
                // Simple markdown table
                var mdLines = new List<string>
                {
                    "| Id | Họ tên | Vị trí | Lương | Ngày vào | Ngày kết thúc |",
                    "|----|--------|--------|-------|----------|----------------|"
                };
                foreach (var e in employees)
                {
                    mdLines.Add($"| {e.Id} | {e.FullName} | {e.Position} | {e.Salary} | {e.StartDate:yyyy-MM-dd} | {e.EndDate:yyyy-MM-dd} |");
                }
                var mdContent = string.Join(Environment.NewLine, mdLines);
                var bytes = System.Text.Encoding.UTF8.GetBytes(mdContent);
                return File(bytes, "text/markdown", $"{fileNameBase}.md");
            }

            if (format == "csv")
            {
                var csvLines = new List<string>
                {
                    "Id,FullName,Position,Salary,StartDate,EndDate"
                };
                foreach (var e in employees)
                {
                    csvLines.Add($"{e.Id},\"{e.FullName}\",\"{e.Position}\",{e.Salary},{e.StartDate:yyyy-MM-dd},{e.EndDate:yyyy-MM-dd}");
                }
                var csvContent = string.Join(Environment.NewLine, csvLines);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                return File(bytes, "text/csv", $"{fileNameBase}.csv");
            }

            // Default: plain text
            var defaultBytes = System.Text.Encoding.UTF8.GetBytes(content);
            return File(defaultBytes, "text/plain", $"{fileNameBase}.txt");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee emp)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(emp);
                _context.SaveChanges();
                return RedirectToAction("EmployeeManagement");
            }
            return View(emp);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee emp)
        {
            if (ModelState.IsValid)
            {
                _context.Update(emp);
                _context.SaveChanges();
                return RedirectToAction("EmployeeManagement");
            }
            return View(emp);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();
            _context.Employees.Remove(emp);
            _context.SaveChanges();
            return RedirectToAction("EmployeeManagement");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var employees = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                employees = employees.Where(e => (e.FullName ?? "").Contains(keyword)
                                              || (e.Position ?? "").Contains(keyword));
            }

            return PartialView("EmployeeSearchResults", await employees.ToListAsync());
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emp = _context.Employees.FirstOrDefault(e => e.UserId == userId);
            if (emp == null) return NotFound();
            return View(emp);
        }
    }
}
