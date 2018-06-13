using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class Payload
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}
