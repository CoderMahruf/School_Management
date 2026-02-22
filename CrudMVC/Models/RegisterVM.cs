using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class RegisterVM
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please enter Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        public string Role { get; set; }
    }
}
