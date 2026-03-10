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

        [HttpGet]
        public async Task<IActionResult> LoadStudents(int classId)
        {
            // Get students
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();

            // Get subjects assigned to class
            var subjects = await _context.ClassSubjects
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Subject)
                .Select(cs => cs.Subject)
                .ToListAsync();

            // Get student IDs
            var studentIds = students.Select(s => s.Id).ToList();

            // Get subject IDs
            var subjectIds = subjects.Select(s => s.Id).ToList();

            // Get existing results
            var results = await _context.Results
                .Where(r => studentIds.Contains(r.StudentId) && subjectIds.Contains(r.SubjectId))
                .ToListAsync();

            var vm = new ResultEntryVM
            {
                ClassId = classId,
                Subjects = subjects,
                Students = students.Select(student => new StudentResultVM
                {
                    StudentId = student.Id,
                    StudentName = student.Name,
                    RollNumber = student.RollNumber,

                    SubjectMarks = subjects.Select(subject =>
                    {
                        var existingMark = results
                            .FirstOrDefault(r => r.StudentId == student.Id && r.SubjectId == subject.Id);

                        return new SubjectMarkVM
                        {
                            SubjectId = subject.Id,
                            SubjectName = subject.Name,
                            Marks = existingMark?.Marks ?? 0
                        };
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
                        existingResult.RollNumber = student.RollNumber;
                        existingResult.ClassId = model.ClassId;
                    }
                    else
                    {
                        _context.Results.Add(new Result
                        {
                            StudentId = student.StudentId,
                            SubjectId = subject.SubjectId,
                            RollNumber = student.RollNumber,
                            ClassId = model.ClassId,
                            Marks = subject.Marks
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("LoadStudents", new { classId = model.ClassId });
        }
    }
}
