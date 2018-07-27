using Microsoft.Extensions.Configuration;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Config
{
    public class InviteTemplateEmailConfig : TemplateEmailConfig<InviteEmailModel>, IInviteTemplateEmailConfig
    {
        public override string ConfigId => "email:invite_template_id";
        public InviteTemplateEmailConfig(IConfiguration configuration): base(configuration)
        {
        }
    }
}

