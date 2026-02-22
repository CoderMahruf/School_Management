using CrudMVC.Data;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var totalUsers = users.Count;
            var totalStudents = 0;
            var totalTeachers = 0;
            var totalAdmins = 0;

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student"))
                    totalStudents++;

                if (roles.Contains("Teacher"))
                    totalTeachers++;

                if (roles.Contains("Admin"))
                    totalAdmins++;
            }

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalTeachers = totalTeachers;
            ViewBag.TotalAdmins = totalAdmins;

            return View();
        }
    }
}
