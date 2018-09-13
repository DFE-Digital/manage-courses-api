using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.ApiClient
{
    public interface ICourseMapper
    {
        SearchAndCompare.Domain.Models.Course MapToSearchAndCompareCourse(UcasInstitution ucasInstData, ApiClient.Course ucasCourseData, InstitutionEnrichmentModel orgEnrichmentModel, CourseEnrichmentModel courseEnrichmentModel);
    }
}
