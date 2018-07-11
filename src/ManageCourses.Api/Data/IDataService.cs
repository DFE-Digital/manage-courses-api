using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        void ProcessPayload(Payload payload);
        OrganisationCourses GetCoursesForUser(string email);
    }
}
