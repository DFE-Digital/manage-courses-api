using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class School
    {
        public string LocationName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostCode { get; set; }
        public string Code { get; set; }
        public string FullTimeVacancies { get; set; }
        public string PartTimeVacancies { get; set; }
        public string ApplicationsAcceptedFrom { get; set; }
    }
}
