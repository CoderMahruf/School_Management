using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class SubjectEditVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string SubjectCode { get; set; }
    }
}
