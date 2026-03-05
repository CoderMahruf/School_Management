using CrudMVC.Data;
using CrudMVC.Models;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ClassController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var classes = await _dbContext.Classes.ToListAsync();
            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var vm = new ClassCreateVM
            {
                Subjects = await _dbContext.Subjects.ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClassCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Subjects = await _dbContext.Subjects.ToListAsync();
                return View(model);
            }

            var newClass = new Class
            {
                Name = model.Name
            };

            foreach (var subjectId in model.SelectedSubjectIds)
            {
                newClass.ClassSubjects.Add(new ClassSubject
                {
                    SubjectId = subjectId
                });
            }

            _dbContext.Classes.Add(newClass);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var classEntity = await _dbContext.Classes
                .Include(c => c.ClassSubjects)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classEntity == null)
                return NotFound();

            var vm = new ClassEditVM
            {
                Id = classEntity.Id,
                Name = classEntity.Name,
                SelectedSubjectIds = classEntity.ClassSubjects
                                        .Select(cs => cs.SubjectId)
                                        .ToList(),
                Subjects = await _dbContext.Subjects.ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClassEditVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Subjects = await _dbContext.Subjects.ToListAsync();
                return View(model);
            }

            var classInDb = await _dbContext.Classes
                .Include(c => c.ClassSubjects)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (classInDb == null)
                return NotFound();

            // Update name
            classInDb.Name = model.Name;

            // Remove old subjects
            classInDb.ClassSubjects.Clear();

            // Add new selected subjects
            foreach (var subjectId in model.SelectedSubjectIds)
            {
                classInDb.ClassSubjects.Add(new ClassSubject
                {
                    ClassId = model.Id,
                    SubjectId = subjectId
                });
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var classes = await _dbContext.Classes.FindAsync(id);
            if (classes != null)
            {
                _dbContext.Classes.Remove(classes);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(List));
        }
    }
}
