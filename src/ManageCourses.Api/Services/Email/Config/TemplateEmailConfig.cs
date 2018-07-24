using System;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;
using Microsoft.Extensions.Configuration;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Config
{
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

