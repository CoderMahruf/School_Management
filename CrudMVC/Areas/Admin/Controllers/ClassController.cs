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
            var classes = await _dbContext.Classes
                .Include(c => c.ClassSubjects)
                .ThenInclude(cs => cs.Subject)
                .ToListAsync();

            ViewBag.Subjects = await _dbContext.Subjects.ToListAsync();

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateClass(int id, string name, List<int>? selectedSubjectIds)
        {
            var classInDb = await _dbContext.Classes
                .Include(c => c.ClassSubjects)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (classInDb == null)
                return NotFound();

            // update class name
            classInDb.Name = name;

            // remove old subjects
            classInDb.ClassSubjects.Clear();

            if (selectedSubjectIds != null)
            {
                foreach (var subjectId in selectedSubjectIds)
                {
                    classInDb.ClassSubjects.Add(new ClassSubject
                    {
                        ClassId = id,
                        SubjectId = subjectId
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
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
