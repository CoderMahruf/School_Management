using CrudMVC.Data;
using CrudMVC.Models;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CrudMVC.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddStudentViewModel
            {
                Classes = await _dbContext.Classes
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddStudentViewModel model)
        {
            // Repopulate dropdown
            model.Classes = await _dbContext.Classes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            if (model.ClassId == null)
            {
                ModelState.AddModelError("", "Class is required.");
                return View(model);
            }

            // Get subjects assigned to selected class
            var classSubjects = await _dbContext.ClassSubjects
                .Where(cs => cs.ClassId == model.ClassId.Value)
                .ToListAsync();

            var student = new Student
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                RollNumber = model.RollNumber,
                ClassId = model.ClassId.Value,
                UserId = _userManager.GetUserId(User),

                // Auto-assign subjects
                StudentSubjects = classSubjects
                    .Select(cs => new StudentSubject
                    {
                        SubjectId = cs.SubjectId
                    }).ToList()
            };

            _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var students = await _dbContext.Students
                .Include(s => s.Class)
                .ToListAsync();

            return View(students);
        }

      

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _dbContext.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound();

            var model = new EditStudentVM
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Phone = student.Phone,
                RollNumber = student.RollNumber,
                ClassId = student.ClassId,

                Classes = await _dbContext.Classes
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditStudentVM model)
        {
            model.Classes = await _dbContext.Classes
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            var student = await _dbContext.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (student == null)
                return NotFound();

            // Update basic fields
            student.Name = model.Name;
            student.Email = model.Email;
            student.Phone = model.Phone;
            student.RollNumber = model.RollNumber;
            student.ClassId = model.ClassId;

            // Remove old subject mappings
            _dbContext.StudentSubjects.RemoveRange(student.StudentSubjects);

            // Get new class subjects
            var classSubjects = await _dbContext.ClassSubjects
                .Where(cs => cs.ClassId == model.ClassId)
                .ToListAsync();

            // Re-assign subjects automatically
            student.StudentSubjects = classSubjects
                .Select(cs => new StudentSubject
                {
                    StudentId = student.Id,
                    SubjectId = cs.SubjectId
                }).ToList();

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _dbContext.Students
                .Include(s => s.StudentSubjects)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student != null)
            {
                _dbContext.StudentSubjects.RemoveRange(student.StudentSubjects);
                _dbContext.Students.Remove(student);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(List));
        }
    }
}