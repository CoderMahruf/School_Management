using CrudMVC.Models.Entities;

public class ResultEntryVM
{
    public int ClassId { get; set; }

    public List<StudentResultVM> Students { get; set; }

    public List<Subject> Subjects { get; set; }
}

public class StudentResultVM
{
    public int StudentId { get; set; }

    public string StudentName { get; set; }

    public string RollNumber { get; set; }

    public List<SubjectMarkVM> SubjectMarks { get; set; }
}

public class SubjectMarkVM
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; }

    public int Marks { get; set; }
}