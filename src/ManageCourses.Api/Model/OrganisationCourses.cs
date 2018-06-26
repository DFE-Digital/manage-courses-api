using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovUk.Education.ManageCourses.Api.Model
{
    public class OrganisationCourses
    {
        public string OrganisationName { get; set; }
        public string OrganisationId { get; set; }
        public List<ProviderCourse> ProviderCourses {get; set; }
    }

    public class ProviderCourse
    {
        public string AccreditingProviderName { get; set; }
        public string AccreditingProviderId { get; set; }
        public List<CourseDetail> CourseDetails { get; set; }
    }

    public class CourseDetail
    {
        public string CourseTitle { get; set; }
        public string Route { get; set; }
        public List<string> Subjects { get; set; }
        public string AgeRange { get; set; }
        public string Qualification { get; set; }
        public List<CourseVariant> Variants { get; set; }
    }

    public class CourseVariant
    {
        public string CourseCode => this.UcasCode;
        public string UcasCode { get; set; }
        public string Name { get; set; }
        public string TrainingProviderCode { get; set; }
        public string TrainingProgramCode { get; set; }
        public string ProfPostFlag { get; set; }
        public string ProgramType { get; set; }
        public string StudyMode { get; set; }
        public List<Campus> Campuses { get; set; }
    }
}
