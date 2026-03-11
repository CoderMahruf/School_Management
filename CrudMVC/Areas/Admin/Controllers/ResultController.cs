using CrudMVC.Data;
using CrudMVC.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

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

        private string CalculateGrade(double average)
        {
            if (average >= 90) return "A+";
            if (average >= 76) return "A";
            if (average >= 60) return "A-";
            if (average >= 50) return "B";
            if (average >= 40) return "C";
            if (average >= 33) return "D";
            return "F";
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

        public async Task<IActionResult> ClassResults(int classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.RollNumber)
                .ToListAsync();

            var subjects = await _context.ClassSubjects
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Subject)
                .Select(cs => cs.Subject)
                .ToListAsync();

            var results = await _context.Results
                .Where(r => r.ClassId == classId)
                .ToListAsync();

            var vm = students.Select(student => new StudentResultVM
            {
                StudentId = student.Id,
                StudentName = student.Name,
                RollNumber = student.RollNumber,

                SubjectMarks = subjects.Select(subject =>
                {
                    var mark = results
                        .FirstOrDefault(r => r.StudentId == student.Id && r.SubjectId == subject.Id);

                    return new SubjectMarkVM
                    {
                        SubjectId = subject.Id,
                        SubjectName = subject.Name,
                        Marks = mark?.Marks ?? 0
                    };
                }).ToList()

            }).ToList();

            ViewBag.ClassId = classId;

            return View(vm);
        }

        public async Task<IActionResult> ExportMarksheet(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound();

            var results = await _context.Results
                .Where(r => r.StudentId == studentId)
                .Include(r => r.Subject)
                .ToListAsync();

            int totalMarks = results.Sum(r => r.Marks);
            double average = results.Count > 0 ? totalMarks / (double)results.Count : 0;

            string grade = CalculateGrade(average);

            var vm = new StudentResultVM
            {
                StudentId = student.Id,
                StudentName = student.Name,
                RollNumber = student.RollNumber,
                ClassName = student.Class.Name,
                TotalMarks = totalMarks,
                Grade = grade,

                SubjectMarks = results.Select(r => new SubjectMarkVM
                {
                    SubjectId = r.SubjectId,
                    SubjectName = r.Subject.Name,
                    Marks = r.Marks
                }).ToList()
            };

            var document = new StudentMarksheetDocument(vm);
            var pdf = document.GeneratePdf();

            return File(pdf, "application/pdf", $"{student.Name}_Marksheet.pdf");
        }
    }
}
