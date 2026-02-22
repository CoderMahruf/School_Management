namespace CrudMVC.Areas.Admin.Models
{
    public class UserVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Roles { get; set; }
    }
}
