using CrudMVC.Areas.Admin.Models;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrudMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Content("User not found");

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
                return Content("You are trying to delete yourself");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return Content("Deleted Successfully");
        }
    }
}
