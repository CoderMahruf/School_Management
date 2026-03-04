using CrudMVC.Models.Entities;

namespace CrudMVC.Models
{
    public class ClassCreateVM
    {
        public string Name { get; set; }

        public List<int> SelectedSubjectIds { get; set; } = new List<int>();

        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
