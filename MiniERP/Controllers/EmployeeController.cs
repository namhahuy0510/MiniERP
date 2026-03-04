using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace MiniERP.Controllers
{
    [Authorize] // bắt buộc phải đăng nhập mới vào được controller này
    public class EmployeeController : Controller
    {
        private readonly MiniERPContext _context;

        public EmployeeController(MiniERPContext context)
        {
            _context = context;
        }

        // User nào đăng nhập cũng xem được danh sách
        [HttpGet]
        public IActionResult EmployeeManagement()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        // Chỉ Admin mới được tạo nhân viên
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

        // Chỉ Admin mới được sửa
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

        // Chỉ Admin mới được xóa
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

        // Tìm kiếm: chỉ cần đăng nhập, không phân role
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
    }
}
