using Microsoft.Extensions.Configuration;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Config
{
    public class WelcomeTemplateEmailConfig : TemplateEmailConfig<WelcomeEmailModel>, IWelcomeTemplateEmailConfig
    {
        public override string ConfigId => "email:welcome_template_id";
        public WelcomeTemplateEmailConfig(IConfiguration configuration): base(configuration)
        {
        }
    }
}

