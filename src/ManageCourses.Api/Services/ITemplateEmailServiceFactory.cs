namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface ITemplateEmailServiceFactory
    {
        ITemplateEmailService Build(string templateKey);
    }
}