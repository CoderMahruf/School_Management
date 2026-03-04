using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class AddStudentViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string RollNumber { get; set; }

        [Required]
        public int? ClassId { get; set; }

        // Dropdowns
        public List<SelectListItem> Classes { get; set; } = new();
    }
}