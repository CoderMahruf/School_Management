namespace CrudMVC.Models
{
    public class AddStudentViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid ClassId { get; set; }
        public string RollNumber { get; set; }
    }
}