using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.ConfirmationMessageNotSent;
        //Foreign key
        public int? CourseId { get; set; }
       
        public Course? Course { get; set; }
    }

    public enum EnrollmentStatus
    {
        [Display(Name = "Confirmation message not sent")]
        ConfirmationMessageNotSent = 1,

        [Display(Name = "Confirmation message sent")]
        ConfirmationMessageSent = 2,

        [Display(Name = "Enrollment confirmed")]
        EnrollmentConfirmed = 3,

        [Display(Name = "Enrollment declined")]
        EnrollmentDeclined = 4
    }
}
