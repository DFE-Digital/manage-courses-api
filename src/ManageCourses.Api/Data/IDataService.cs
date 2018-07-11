using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        void ProcessPayload(Payload payload);
        OrganisationCourses GetCoursesForUserOrganisation(string email, string organisationId); 
        IEnumerable<UserOrganisation> GetOrganisationsForUser(string email);
    }
}
    