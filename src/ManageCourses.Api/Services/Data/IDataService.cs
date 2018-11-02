using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        IEnumerable<ProviderSummary> GetProviderSummariesForUser(string email);
        ProviderSummary GetProviderSummaryForUser(string email, string providerCode);
        Course GetCourseForUser(string email, string providerCode, string courseCode);
        List<Course> GetCoursesForUser(string email, string providerCode);
        Provider GetUcasProviderForUser(string name, string providerCode);
    }
}
    