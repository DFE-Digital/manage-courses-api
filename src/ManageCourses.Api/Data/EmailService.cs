
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public class EmailService : IEmailService
    {

        private SmtpClient _smtpClient;
        private string _user;

        // This class only supports the RFC-compliant port 587
        private const int SupportedSmtpPort = 587;

        public EmailService(string host, string user, string password)
        {
            _user = user;
            _smtpClient = new SmtpClient
            {
                Host = host,
                Port = SupportedSmtpPort,
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
            _smtpClient.Send(mailMessage);
        }
    }
}