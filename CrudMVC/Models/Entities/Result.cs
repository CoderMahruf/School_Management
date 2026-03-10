namespace CrudMVC.Models.Entities
{
    public class Result
    {
        public Guid Id { get; set; }

        public int ClassId { get; set; } 
        public Class Class { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public string RollNumber { get; set; }

        public int Marks { get; set; }
    }
}
