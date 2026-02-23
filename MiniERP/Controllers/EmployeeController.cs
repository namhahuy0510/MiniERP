using Microsoft.AspNetCore.Mvc;
using MiniERP.Data;

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
    }
}
