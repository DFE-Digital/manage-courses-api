using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class OrganisationCourses
    {
        public string OrganisationName { get; set; }
        public string OrganisationId { get; set; }
        public List<ProviderCourse> ProviderCourses {get; set; }
    }
}
