using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

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
        Course GetCourse(string instCode, string ucasCode);
        List<Course> GetAllCourses();
        InstitutionCourses GetCourses(string email, string instCode);
        UcasInstitution GetUcasInstitutionForUser(string name, string instCode);
        UcasInstitution GetUcasInstitution(string instCode);
    }
}
    