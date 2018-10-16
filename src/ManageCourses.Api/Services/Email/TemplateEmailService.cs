using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using GovUk.Education.ManageCourses.Api.Services.Email.Config;

namespace GovUk.Education.ManageCourses.Api.Services.Email
{
    /// <summary>
    /// The provides the functionality to to send an template-based email.
    /// <see cref="_config">The template configuration to use to send the email and the email model to use.</see>
    /// <typeparam name="TEmailModel">The type of the email model.</typeparam>
    /// <seealso cref="GovUk.Education.ManageCourses.Api.Services.Email.ITemplateEmailService{TEmailModel}" />
    /// <example> 
    /// This sample shows expected implementation.
    /// 
    /// Required implementation to derived from 
    /// <seealso cref="GovUk.Education.ManageCourses.Api.Services.Email.TemplateEmailService{TEmailModel}" />
    /// <seealso cref="GovUk.Education.ManageCourses.Api.Services.Email.Model.IEmailModel" />
    /// <seealso cref="GovUk.Education.ManageCourses.Api.Services.Email.ITemplateEmailService{TEmailModel}" />
    /// <code>
    /// public class CustomEmailModel : IEmailModel
    /// {
    /// }
    /// 
    /// public class CustomTemplateEmailService : TemplateEmailService&lt;CustomEmailModel&gt;, ICustomTemplateEmailService
    /// {
    /// }
    /// public class CustomTemplateEmailConfig : TemplateEmailConfig&lt;CustomEmailModel&gt;, ICustomTemplateEmailConfig
    /// {
    /// public override string ConfigId => "email:custom_template_id";
    /// ...
    /// }
    /// 
    /// // startup class
    /// services.AddScoped&lt;ICustomTemplateEmailConfig, CustomTemplateEmailConfig&gt;();
    /// services.AddScoped&lt;ICustomTemplateEmailService, CustomTemplateEmailService&gt;();
    /// </code>
    /// </example>
    /// </summary>
    public abstract class TemplateEmailService<TEmailModel> : ITemplateEmailService<TEmailModel> where TEmailModel : class, IEmailModel
    {
        private readonly INotificationClientWrapper _notificationClient;

        private readonly ITemplateEmailConfig<TEmailModel> _config;

        public TemplateEmailService(INotificationClientWrapper noticationClientWrapper, ITemplateEmailConfig<TEmailModel> config)
        {
            _notificationClient = noticationClientWrapper;
            _config = config;
        }

        public void Send(TEmailModel model)
        {
            _notificationClient.SendEmail(model.EmailAddress, _config.TemplateId, model.Personalisation);
        }
    }
}

