using CourseManagementSystem.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CourseManagementSystem.ViewModels
{
    public class EnrollmentViewModel
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        [ValidateNever]
        public string StudentName { get; set; }
        [ValidateNever]
        public string CourseName { get; set; }
        [ValidateNever]
        public string RoomNumber { get; set; }
        [ValidateNever]
        public string StartDate { get; set; }
        [ValidateNever]
        public string Instructor { get; set; }
        [ValidateNever]
        public EnrollmentStatus EnrollmentStatus { get; set; }

        // For the radio buttons
        public bool IsConfirmed { get; set; }
    }
}
