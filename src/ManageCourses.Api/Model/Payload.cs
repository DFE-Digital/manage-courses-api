using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class Payload
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}
