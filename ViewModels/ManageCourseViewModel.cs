using CourseManagementSystem.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CourseManagementSystem.ViewModels
{
    public class ManageCourseViewModel
    {
        public int CourseId { get; set; }
        [ValidateNever]
        public List<Student> Students { get; set; }
        public Student NewStudent { get; set; } = new Student();
        [ValidateNever]
        public Course Course { get; set; }
        [ValidateNever]
        public string CourseName { get; set; }
        [ValidateNever]
        public string RoomNumber { get; set; }
        [ValidateNever]
        public string StartDate { get; set; }
        [ValidateNever]
        public string Instructor { get; set; }

        public int? StudentsConfirmationMessageNotSentCount { get; set; }
        public int? StudentsConfirmationMessageSentCount { get; set; }
        public int? StudentsEnrollmentConfirmedCount { get; set; }
        public int? StudentsEnrollmentDeclinedCount { get; set; }
    }
}
