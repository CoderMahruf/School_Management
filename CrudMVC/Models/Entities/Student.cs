using Microsoft.AspNetCore.Identity;

namespace CrudMVC.Models.Entities
{
    public class Student
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }   
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
