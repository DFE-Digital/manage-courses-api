using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Model
{
    /// <summary>
    /// This class should eventually replace all course objects return from the Api.
    /// This should contain alll details required by the Ui
    /// </summary>
    public class Course
    {
        public string CourseCode { get; set; }
        public string ProgramType { get; set; }
        public string ProfpostFlag { get; set; }
        public string Name { get; set; }
        public string InstCode { get; set; }
        public string AccreditingProviderId { get; set; }
        public string AccreditingProviderName { get; set; }
        public string Subjects { get; set; }
        public string StudyMode { get; set; }
        public string AgeRange { get; set; }
        public EnumStatus? EnrichmentWorkflowStatus { get; set; }
        public IEnumerable<School> Schools { get; set; }
    }
}
