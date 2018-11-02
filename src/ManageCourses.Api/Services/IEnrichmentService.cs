using System.Collections.Generic;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public interface IEnrichmentService
    {
        UcasProviderEnrichmentGetModel GetProviderEnrichment(string providerCode, string email);
        UcasProviderEnrichmentGetModel GetProviderEnrichmentForPublish(string providerCode, string email);
        UcasCourseEnrichmentGetModel GetCourseEnrichment(string providerCode, string ucasCourseCode, string email);        
        UcasCourseEnrichmentGetModel GetCourseEnrichmentForPublish(string providerCode, string ucasCourseCode, string email);
        void SaveProviderEnrichment(UcasProviderEnrichmentPostModel model, string providerCode, string email);
        bool PublishProviderEnrichment(string providerCode, string email);        
        void SaveCourseEnrichment(CourseEnrichmentModel model, string providerCode, string ucasCourseCode, string email);
        bool PublishCourseEnrichment(string providerCode, string ucasCourseCode, string email);
        IList<UcasCourseEnrichmentGetModel> GetCourseEnrichmentMetadata(string providerCode, string email);
    }
}
