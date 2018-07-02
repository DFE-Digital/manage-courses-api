
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IEmailService
    {
        void SendEmailToSupport(string subject, string message);
    }
}
