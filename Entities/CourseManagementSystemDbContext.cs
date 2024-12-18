using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Entities;

namespace CourseManagementSystem.Entities
{
    public class CourseManagementSystemDbContext : DbContext
    {
        public CourseManagementSystemDbContext(DbContextOptions<CourseManagementSystemDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
