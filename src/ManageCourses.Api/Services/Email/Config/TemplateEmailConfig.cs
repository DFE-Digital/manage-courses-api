using System;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Config
{
    /// <summary>
    /// The derived class is expected to be injected.
    /// Inherit from this abstract class and set the <see cref="ConfigId">configuration id.</see>
    /// </summary>
    /// <typeparam name="TEmailModel">The type of the email model.</typeparam>
    /// <seealso cref="GovUk.Education.ManageCourses.Api.Services.Email.Config.ITemplateEmailConfig{TEmailModel}" />
    public abstract class TemplateEmailConfig<TEmailModel> : ITemplateEmailConfig<TEmailModel> where TEmailModel : class, IEmailModel
    {
        public Type Type { get; }

        public abstract string ConfigId { get; }

        public string TemplateId { get; }

        public TemplateEmailConfig(IConfiguration configuration)
        {
            Type = typeof(TEmailModel);
            
            TemplateId = configuration[ConfigId];
        }
    }
}

