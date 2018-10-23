using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Data
{
    public interface IDataService
    {
        void ProcessUcasPayload(UcasPayload payload);
        IEnumerable<UserOrganisation> GetOrganisationsForUser(string email);
        UserOrganisation GetOrganisationForUser(string email, string instCode);
        Course GetCourse(string email, string instCode, string ucasCode);
        InstitutionCourses GetCourses(string email, string instCode);
        UcasInstitution GetUcasInstitutionForUser(string name, string instCode);
    }
}
    