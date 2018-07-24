using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public interface ITemplateEmailService<EmailModel> where EmailModel : class, IEmailModel
    {
        void Send(EmailModel model);
    }
}
