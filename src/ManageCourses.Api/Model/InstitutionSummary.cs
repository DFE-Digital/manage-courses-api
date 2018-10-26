using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class InstitutionSummary
    {
        public string InstName { get; set; }
        public string InstCode { get; set; }
        public int TotalCourses { get; set; }
        public EnumStatus? EnrichmentWorkflowStatus { get; set; }
    }
}
