namespace CrudMVC.Models.Entities
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
