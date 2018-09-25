using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IEnrichmentService
    {
        UcasInstitutionEnrichmentGetModel GetInstitutionEnrichment(string instCode, string email);
        void SaveInstitutionEnrichment(UcasInstitutionEnrichmentPostModel model, string instCode, string email);
        bool PublishInstitutionEnrichment(string instCode, string email);
        UcasCourseEnrichmentGetModel GetCourseEnrichment(string instCode, string ucasCourseCode, string email);
        void SaveCourseEnrichment(CourseEnrichmentModel model, string instCode, string ucasCourseCode, string email);
        bool PublishCourseEnrichment(string instCode, string ucasCourseCode, string email);
        IList<UcasCourseEnrichmentGetModel> GetCourseEnrichmentMetadata(string instCode, string email);
        InstitutionEnrichmentModel GetPublishableInstitutionEnrichmentModel(string instCode, string email);
    }
}
