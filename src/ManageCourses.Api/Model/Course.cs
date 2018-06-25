using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class Course
    {
        public string Title { get; set; }
        public string UcasCode { get; set; }
        public string OrganisationName { get; set; }
        public List<Variant> Variants { get; set; }
    }
}
