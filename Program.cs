using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Entities;
using CourseManagementSystem.Filters;
using CourseManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

//Retrieves connection string from appsettings.json file
var connstr = builder.Configuration.GetConnectionString("CourseManagementSystemDB");

//Registers CourseManagementDbContext in the dependency injection container
builder.Services.AddDbContext<CourseManagementSystemDbContext>(o => o.UseSqlServer(connstr));

// Add services to the container. including the filter so that it is available to all pages
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<WelcomeMessageFilter>();
});

//Dependency Injection for emailservice
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
