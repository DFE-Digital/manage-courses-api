using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {

        // IEnumerable<InstitutionSummary> GetInstitutionSummariesForUser(string email);
        // InstitutionSummary GetInstitutionSummaryForUser(string email, string providerCode);
        Course GetCourseForUser(string email, string providerCode, string courseCode);
        List<Course> GetCoursesForUser(string email, string providerCode);
        Provider GetProviderForUser(string name, string providerCode);
    }
}
