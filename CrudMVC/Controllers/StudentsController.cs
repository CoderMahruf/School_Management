using CrudMVC.Data;
using CrudMVC.Models;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudMVC.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel viewModel)
        {
            var existingStudent = await _dbContext.Students
                .FirstOrDefaultAsync(s => s.Email == viewModel.Email);

            if (existingStudent != null)
            {
                ModelState.AddModelError("Email", "A student with this email already exists.");
                return View(viewModel);
            }
            var user = await _userManager.GetUserAsync(User);
            var student = new Student
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = viewModel.Name,
                Email = viewModel.Email,
                Phone = viewModel.Phone
            };

            await _dbContext.Students.AddAsync(student);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var students = await _dbContext.Students.ToListAsync();
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student viewModel)
        {
            var student = await _dbContext.Students.FindAsync(viewModel.Id);
            if (student is not null)
            {
                student.Name = viewModel.Name;
                student.Email = viewModel.Email;
                student.Phone = viewModel.Phone;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var student = await _dbContext.Students.FindAsync(id);

            if (student != null)
            {
                _dbContext.Students.Remove(student);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }
    }
}
