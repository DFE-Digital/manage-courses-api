using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    public interface ITemplateEmailService<TEmailModel> where TEmailModel : class, IEmailModel
    {
        void Send(TEmailModel model);
    }
}
