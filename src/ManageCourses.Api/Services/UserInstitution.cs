using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services
{
    internal class UserInstitution
    {
        public McUser McUser { get; set; }
        public UcasInstitution UcasInstitution { get; set; }
    }
}
