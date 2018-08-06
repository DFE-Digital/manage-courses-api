using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        void ProcessUcasPayload(UcasPayload payload);
        OrganisationCourses GetCoursesForUserOrganisation(string email, string ucasCode); 
        IEnumerable<UserOrganisation> GetOrganisationsForUser(string email);
        UserOrganisation GetOrganisationForUser(string email, string instCode);
        void ProcessReferencePayload(ReferenceDataPayload payload);
        Course GetCourse(string email, string instCode, string ucasCode);
        InstitutionCourses GetCourses(string email, string instCode);

    }
}
    