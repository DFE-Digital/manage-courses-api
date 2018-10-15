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
        public Course()
        {
            Schools = new List<School>();
        }

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
        public CourseQualification Qualification { get; set; }

        /// <summary>
        /// This describes the course in how it differs to other courses with similar titles and subjects.
        /// Information may include
        ///  - The qualification
        ///  - whether it's salaried or an apprenticeship
        ///  - whether it's full-time and/or part time
        /// </summary>
        public string TypeDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public EnumStatus? EnrichmentWorkflowStatus { get; set; }
        public IEnumerable<School> Schools { get; set; }

        /// <summary>
        /// Aggregate of vacancy status for each campus.
        /// </summary>
        /// <value></value>
        public bool HasVacancies { get; internal set; }
    }
}
