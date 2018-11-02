using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public interface ICourseMapper
    {
        SearchAndCompare.Domain.Models.Course MapToSearchAndCompareCourse(Provider ucasProviderData, Course ucasCourseData, ProviderEnrichmentModel providerEnrichmentModel, CourseEnrichmentModel courseEnrichmentModel);
    }
}
