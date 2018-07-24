using System;
using GovUk.Education.ManageCourses.Api.Services.Email.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Email.Config
{
    public interface ITemplateEmailConfig<TEmailModel> where TEmailModel : class, IEmailModel
    {
        Type Type { get; }
        string ConfigId { get; }
        string TemplateId { get; }
    }
}

