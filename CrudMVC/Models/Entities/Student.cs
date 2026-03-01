using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models.Entities
{
    public class Student
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }   
        public ApplicationUser User { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string RollNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }

        public Guid ClassId { get; set; }
        public Class Class { get; set; }

    }
}
