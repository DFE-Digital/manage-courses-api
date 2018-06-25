using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class Variant
    {
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string CourseCode { get; set; }
        public List<string> ProfpostFlags { get; set; }
        public List<string> ProgramTypes { get; set; }
        public List<string> StudyModes { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public List<Campus> Campuses { get; set; }
    }
}
