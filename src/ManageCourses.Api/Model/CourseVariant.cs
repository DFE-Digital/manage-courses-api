using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Api.Model
{
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
