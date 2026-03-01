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
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCounts()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var teacherUsers = await _userManager.GetUsersInRoleAsync("Teacher");
            var studentUsers = await _userManager.GetUsersInRoleAsync("Student");
            var allUsers = _dbContext.Users.ToList();

            var result = new
            {
                AdminCount = adminUsers.Count,
                TeacherCount = teacherUsers.Count,
                StudentCount = studentUsers.Count,
                TotalUsers = allUsers.Count
            };

            return Json(result);
        }
    }
}
