using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Data;
using MiniERP.Models;

namespace MiniERP.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MiniERPContext _context;

        public EmployeeController(MiniERPContext context)
        {
            _context = context;
        }
        public IActionResult EmployeeManagement()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
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
        public IActionResult Edit(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();
            return View(emp); 
        }

        [HttpPost]
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
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();
            return View(emp); 
        }

        [HttpPost, ActionName("Delete")]
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
    }
}
