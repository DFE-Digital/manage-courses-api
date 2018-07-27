using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        void ProcessUcasPayload(UcasPayload payload);
        OrganisationCourses GetCoursesForUserOrganisation(string email, string ucasCode); 
        IEnumerable<UserOrganisation> GetOrganisationsForUser(string email);
        void ProcessReferencePayload(ReferenceDataPayload payload);
    }
}
    