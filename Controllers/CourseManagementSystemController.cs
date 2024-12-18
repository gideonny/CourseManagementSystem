using Microsoft.AspNetCore.Mvc;
using CourseManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.ViewModels;
using CourseManagementSystem.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http.Extensions;


namespace CourseManagementSystem.Controllers
{
    public class CourseManagementSystemController : Controller
    {
        //private readonly CourseManagementSystemDbContext _courseManagementSystemDbContext;
        //private readonly EmailService _emailService;
        //private readonly IUrlHelper _urlHelper;

        public CourseManagementSystemDbContext _courseManagementSystemDbContext;
        public EmailService _emailService;
        public CourseManagementSystemController(CourseManagementSystemDbContext courseManagementSystemDbContext, EmailService emailService, IUrlHelperFactory urlHelperFactory)
        {
            _courseManagementSystemDbContext = courseManagementSystemDbContext;
            _emailService = emailService;
            // _urlHelper = urlHelperFactory.GetUrlHelper(ActionContext);
        }

        [HttpGet("/courses")]
        public IActionResult GetAllCourses()
        {
            List<Course> courses = _courseManagementSystemDbContext.Courses.Include(c => c.Students).ToList();
            return View("Items", courses);
        }

        //[HttpGet("/course/{id}")]
        //public IActionResult GetCourseById(int courseId)
        //{
        //    var course = _courseManagementSystemDbContext.Courses.Include(c => c.Students).FirstOrDefault(c => c.CourseId == courseId);

        //    var model = new ManageCourseViewModel
        //    {
        //        Students = course.Students.ToList(),
        //        NewStudent = new Student { CourseId = courseId }
        //    };
        //    return View("ManageCourse", model);
        //}

        [HttpGet("/courses/add-request")]
        public IActionResult GetAddCourseRequest()
        {
            return View("Add", new Course());
        }

        [HttpPost("/courses/")]
        public IActionResult AddNewCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _courseManagementSystemDbContext.Courses.Add(course);
                _courseManagementSystemDbContext.SaveChanges();

                TempData["LastActionMessage"] = "Course was added succesfully";

                return RedirectToAction("GetAllCourses", "CourseManagementSystem");
            }
            return View("Add", course);
        }

        [HttpPost]
        public IActionResult AddNewStudent(int courseId, ManageCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newStudent = model.NewStudent;
                newStudent.CourseId = courseId;

                _courseManagementSystemDbContext.Students.Add(newStudent);
                _courseManagementSystemDbContext.SaveChanges();

                TempData["LastActionMessage"] = "Student added succesfully";

                return RedirectToAction("ManageCourse", "CourseManagementSystem", new { courseId = courseId });
            }

            model.Students = _courseManagementSystemDbContext.Students.Where(s => s.CourseId == courseId).ToList();

            return View("ManageCourse", model);
        }

        [HttpGet("/courses/{CourseId}edit-request")]
        public IActionResult GetEditCourseRequestById(int CourseId)
        {
            var course = _courseManagementSystemDbContext.Courses.Find(CourseId);
            return View("Edit", course);
        }

        [HttpPost("/courses/{CourseId}edit-request")]
        public IActionResult ProcessEditCourseRequest(int courseId, Course course)
        {
            if (courseId != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _courseManagementSystemDbContext.Courses.Update(course);
                _courseManagementSystemDbContext.SaveChanges();

                TempData["LastActionMessage"] = "The course was updated succesfully";

                return RedirectToAction("GetAllCourses", "CourseManagementSystem");
            }

            return View("Edit", course);
        }
        public IActionResult ManageCourse(int courseId)
        {
            var course = _courseManagementSystemDbContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // Get the list of students for the course, ensuring it's never null
            var students = _courseManagementSystemDbContext.Students
                .Where(s => s.CourseId == courseId)
                .ToList() ?? new List<Student>();

            // Calculating counts based on enrollment status
            var enrollmentCounts = students
                .GroupBy(s => s.EnrollmentStatus)
                .ToDictionary(g => g.Key, g => g.Count());

            // Prepare the model with course details and calculated counts
            var model = new ManageCourseViewModel
            {
                CourseId = courseId,
                Students = students,
                NewStudent = new Student { CourseId = courseId },
                CourseName = course.Name,
                RoomNumber = course.RoomNumber,
                StartDate = course.StartDate?.ToString("yyyy-MM-dd"),
                Instructor = course.Instructor,
                StudentsConfirmationMessageNotSentCount = enrollmentCounts.GetValueOrDefault(EnrollmentStatus.ConfirmationMessageNotSent, 0),
                StudentsConfirmationMessageSentCount = enrollmentCounts.GetValueOrDefault(EnrollmentStatus.ConfirmationMessageSent, 0),
                StudentsEnrollmentConfirmedCount = enrollmentCounts.GetValueOrDefault(EnrollmentStatus.EnrollmentConfirmed, 0),
                StudentsEnrollmentDeclinedCount = enrollmentCounts.GetValueOrDefault(EnrollmentStatus.EnrollmentDeclined, 0)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SendConfirmationEmail(int courseId)
        {
            var studentsWithoutConfirmationEmail = _courseManagementSystemDbContext.Students.Where(s => s.CourseId == courseId && s.EnrollmentStatus == EnrollmentStatus.ConfirmationMessageNotSent).Include(s => s.Course).ToList();

            if (!studentsWithoutConfirmationEmail.Any())
            {
                TempData["LastActionMessage"] = "All students have been sent confirmation emails.";
                return RedirectToAction("ManageCourse", new { courseId = courseId });
            }

            string confirmationUrl = Url.Action("ConfirmEnrollment", "CourseManagementSystem", new { courseId }, protocol: Request.Scheme);

            foreach (var student in studentsWithoutConfirmationEmail)
            {
                if (string.IsNullOrEmpty(student.Email))
                {
                    TempData["LastActionMessage"] = $"Student {student.Name} does not have a valid email address.";
                    continue; // Skip this student
                }
                //Accessing the course the student is enrolled in
                var course = student.Course;

                //ensure course exists then send the email
                if (course != null)
                {
                    //generate url
                    confirmationUrl = Url.Action(
                         action: "ConfirmEnrollment",          // Action name
                         controller: "CourseManagementSystem",  // Controller name
                         values: new
                         {
                             studentId = student.StudentId,
                             courseId = courseId
                         },  // Route parameters
                         protocol: Request.Scheme               // Protocol (http/https)
                     );


                    _emailService.SendEmail(student.Email, student.Name, course.Name, course.RoomNumber, course.StartDate?.ToString("yyyy-MM-dd"), course.Instructor, confirmationUrl);

                    //update enrollment status
                    student.EnrollmentStatus = EnrollmentStatus.ConfirmationMessageSent;
                }
            }

            _courseManagementSystemDbContext.SaveChanges();

            TempData["LastActionMessage"] = $"{studentsWithoutConfirmationEmail.Count} enrollment confirmation " + (studentsWithoutConfirmationEmail.Count == 1 ? "email" : "emails") + " sent successfully.";

            return RedirectToAction("ManageCourse", new { courseId = courseId });
        }

        [HttpGet]
        public IActionResult ConfirmEnrollment(int studentId, int courseId)
        {
            var student = _courseManagementSystemDbContext.Students.FirstOrDefault(s => s.StudentId == studentId);
            var course = _courseManagementSystemDbContext.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (student == null)
            {
                // Handle the case where student is not found
                return NotFound("Student not found.");
            }

            // Check for null course

            if (course == null)
            {
                // Handle the case where course is not found
                return NotFound("Course not found.");
            }
            var model = new EnrollmentViewModel
            {
                StudentId = student.StudentId,
                StudentName = student.Name,
                CourseName = course.Name,
                RoomNumber = course.RoomNumber,
                StartDate = course.StartDate?.ToString("yyyy-MM-dd"),
                Instructor = course.Instructor
               
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessConfirmEnrollment(EnrollmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _courseManagementSystemDbContext.Students
                    .FirstOrDefault(s => s.StudentId == model.StudentId);  
                if (student != null)
                {
                    // Set the enrollment status based on IsConfirmed value
                    student.EnrollmentStatus = model.IsConfirmed
                        ? EnrollmentStatus.EnrollmentConfirmed
                        : EnrollmentStatus.EnrollmentDeclined;

                    _courseManagementSystemDbContext.Entry(student).State = EntityState.Modified;

                    _courseManagementSystemDbContext.SaveChanges();
                    return View(model);
                }
                else
                {
                    return NotFound("Student not found.");
                }
            }

            // If model state is invalid, return the same view with validation errors
            return View("ConfirmEnrollment", model);

        }
    }
}

