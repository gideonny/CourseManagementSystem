using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseManagementSystem.Filters
{
    public class WelcomeMessageFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //checking for the first visit
            var firstVisitCookie = context.HttpContext.Request.Cookies["FirstVisit"];

            string welcomeMessage;

            // If the cookie doesn't exist it's the users first visit
            if (firstVisitCookie == null)
            {
                var firstVisitDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                context.HttpContext.Response.Cookies.Append("FirstVisit", firstVisitDate, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    //cookie expires after 1 hour
                    Expires = DateTime.Now.AddHours(1)
                });
                welcomeMessage = "Welcome to your first visit!";
            }
            else
            {
                var firstVisitDate = DateTime.Parse(firstVisitCookie).ToString("yyyy-MM-dd");
                welcomeMessage = $"Welcome back! You first visited on {firstVisitCookie}.";
            }
            //storing the message so it's available to the view
            context.HttpContext.Items["WelcomeMessage"] = welcomeMessage;
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
