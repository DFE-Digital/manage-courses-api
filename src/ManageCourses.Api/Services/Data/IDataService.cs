using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {

        // IEnumerable<InstitutionSummary> GetInstitutionSummariesForUser(string email);
        // InstitutionSummary GetInstitutionSummaryForUser(string email, string instCode);
        Course GetCourseForUser(string email, string instCode, string courseCode);
        List<Course> GetCoursesForUser(string email, string instCode);
        Institution GetUcasInstitutionForUser(string name, string instCode);
    }
}
