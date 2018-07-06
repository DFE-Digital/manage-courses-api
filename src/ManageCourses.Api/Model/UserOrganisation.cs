using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class UserOrganisation
    {
        public string OrganisationName { get; set; }
        public string OrganisationId { get; set; }
        public string UcasCode { get; set; }
        public int TotalCourses { get; set; }
    }
}
