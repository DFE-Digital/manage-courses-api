using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Provider
    {
        public int Id { get; set; }

        public string NctlId { get; set; }

        public IList<Course> Courses { get; set; }
    }
}
