
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class EmailService : IEmailService
    {

        private SmtpClient _smtpClient;
        private string _user;

        public EmailService(string host, string user, string password)
        {
            _user = user;
            _smtpClient = new SmtpClient
            {
                Host = host, // set your SMTP server name here
                Port = 587, // Port 
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(user,password)
            };

        }

        public void SendEmailToSupport(string subject, string message)
        {

            var mailMessage = new MailMessage(_user, _user)
            {
                Subject = subject,
                Body = message
            };
            {
                _smtpClient.Send(mailMessage);
            }
        }
    }
}