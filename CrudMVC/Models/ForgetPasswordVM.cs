using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class ForgetPasswordVM
    {
        [Required]
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password not matched.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = default!;

    }
}
