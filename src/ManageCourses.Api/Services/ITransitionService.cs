using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface ITransitionService
    {
        void UpdateNewCourse(string providerCode, string courseCode, string email);
    }
}
