using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class ProviderCourse
    {
        public string AccreditingProviderName { get; set; }
        public string AccreditingProviderId { get; set; }
        public List<CourseDetail> CourseDetails { get; set; }
    }
}
