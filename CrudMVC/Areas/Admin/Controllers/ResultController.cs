using CrudMVC.Data;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResultController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Classes = _context.Classes.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadStudents(int classId)
        {
            // Fetch students in the class
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();

            // Fetch only subjects linked to the class (checked subjects)
            var subjects = await _context.ClassSubjects
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Subject)
                .Select(cs => cs.Subject)
                .ToListAsync();

            var vm = new ResultEntryVM
            {
                ClassId = classId,
                Subjects = subjects,
                Students = students.Select(s => new StudentResultVM
                {
                    StudentId = s.Id,
                    StudentName = s.Name,
                    RollNumber = s.RollNumber,
                    SubjectMarks = subjects.Select(sub => new SubjectMarkVM
                    {
                        SubjectId = sub.Id,
                        SubjectName = sub.Name,
                        Marks = 0
                    }).ToList()
                }).ToList()
            };

            return View("ResultEntry", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Save(ResultEntryVM model)
        {
            foreach (var student in model.Students)
            {
                foreach (var subject in student.SubjectMarks)
                {
                    // Only save if marks entered, optional
                    var existingResult = await _context.Results
                        .FirstOrDefaultAsync(r => r.StudentId == student.StudentId && r.SubjectId == subject.SubjectId);

                    if (existingResult != null)
                    {
                        existingResult.Marks = subject.Marks; 
                    }
                    else
                    {
                        _context.Results.Add(new Result
                        {
                            StudentId = student.StudentId,
                            SubjectId = subject.SubjectId,
                            Marks = subject.Marks
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
