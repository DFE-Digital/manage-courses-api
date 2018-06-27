using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class CourseDetail
    {
        public string CourseTitle { get; set; }
        public string Route { get; set; }
        public List<string> Subjects { get; set; }
        public string AgeRange { get; set; }
        public string Qualification { get; set; }
        public List<CourseVariant> Variants { get; set; }
    }
}
