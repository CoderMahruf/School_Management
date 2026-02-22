using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class LoginVM
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please enter Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
