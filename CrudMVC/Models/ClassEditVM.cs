using CrudMVC.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class ClassEditVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<int> SelectedSubjectIds { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();

    }
}
