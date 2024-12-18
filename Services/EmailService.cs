using System.Net.Mail;
using System.Net;
using System.Security.Policy;


namespace CourseManagementSystem.Services
{
    
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _fromAddress;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _fromAddress = _configuration["EmailSettings:SenderEmail"];
            _password = _configuration["EmailSettings:SenderPassword"];
        }

        public void SendEmail(string toAddress, string studentName, string courseName, string roomNumber, string startDate, string instructor, string confirmationUrl)
        {
            if (string.IsNullOrEmpty(toAddress))
            {
                throw new ArgumentNullException(nameof(toAddress), "Email address cannot be null or empty.");
            }
            

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromAddress, _password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromAddress),
                Subject = $"Enrollment Confirmation for \"{courseName}\" required",
                Body = $"<h1>Hello {studentName},</h1><p>Your request to enroll in the course \"{courseName}\" in room {roomNumber} starting {startDate} with instructor {instructor}.</p> <p>We are pleased to have you in the course so if you could <a href=\"{confirmationUrl}\">confirm your enrollment</a> as soon as possible that would be appreciated!</p><p>Sincerely,</p><p>The Course Manager App</p>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toAddress);

            // Send email synchronously
            smtpClient.Send(mailMessage);
        }
    }
}



