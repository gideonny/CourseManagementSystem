using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Instructor {  get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        [RegularExpression(@"^\d[A-Z]\d\d$", ErrorMessage = "Room number must be in the format: A single digit, a single capital letter, and 2 digits, e.g 3G12, 1C07")]
        public string? RoomNumber { get; set; }

        //Navigation property

    public ICollection<Student>? Students { get; set; } = new List<Student>();
    }
}
