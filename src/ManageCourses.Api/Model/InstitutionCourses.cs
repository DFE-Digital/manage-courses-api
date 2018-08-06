using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class InstitutionCourses
    {
        public InstitutionCourses()
        {
            Courses = new List<Course>();
        }
        public string InstitutionName { get; set; }
        public string InstitutionCode { get; set; }
        public List<Course> Courses { get; set; }
    }
}
