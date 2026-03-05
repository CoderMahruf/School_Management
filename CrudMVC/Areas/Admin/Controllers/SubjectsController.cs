using CrudMVC.Data;
using CrudMVC.Models;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public SubjectsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var subjects = await _dbContext.Subjects.ToListAsync();
            return View(subjects);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Subject subject)
        {
            _dbContext.Subjects.Add(subject);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _dbContext.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            var vm = new SubjectEditVM
            {
                Id = subject.Id,
                Name = subject.Name,
                SubjectCode = subject.SubjectCode
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectEditVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var subjectInDb = await _dbContext.Subjects.FindAsync(model.Id);
            if (subjectInDb == null)
                return NotFound();

            // Update only the fields you want
            subjectInDb.Name = model.Name;
            subjectInDb.SubjectCode = model.SubjectCode;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _dbContext.Subjects.FindAsync(id);
            if (subject != null)
            {
                _dbContext.Subjects.Remove(subject);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(List));
        }
    }
}
