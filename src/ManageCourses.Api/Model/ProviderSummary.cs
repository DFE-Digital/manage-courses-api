using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class ProviderSummary
    {
        public string ProviderName { get; set; }
        public string ProviderCode { get; set; }
        public bool OptedIn { get; set; }
        public int TotalCourses { get; set; }
        public EnumStatus? EnrichmentWorkflowStatus { get; set; }
    }
}
