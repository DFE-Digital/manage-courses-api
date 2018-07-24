using System;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Config
{
    public abstract class TemplateEmailConfig<TEmailModel> : ITemplateEmailConfig<TEmailModel> where TEmailModel : class, IEmailModel
    {
        public Type Type { get; }

        public string ConfigId { get; }
        public string TemplateId { get; }

        public TemplateEmailConfig(string _config, string _templateId)
        {
            Type = typeof(TEmailModel);
            ConfigId = _config;
            TemplateId = _templateId;
        }
    }
}

