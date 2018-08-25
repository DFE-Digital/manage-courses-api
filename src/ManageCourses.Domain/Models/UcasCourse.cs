namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasCourse
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CrseCode { get; set; }
        public string CrseTitle { get; set; }                
        public string Studymode { get; set; }
        public string Age { get; set; }
        public string CampusCode { get; set; }
        public string ProfpostFlag { get; set; }
        public string ProgramType { get; set; }
        public string AccreditingProvider { get; set; }
        public string CrseOpenDate { get; set; }
        public string Publish { get; set; }
        public string Status { get; set; }
        public string VacStatus { get; set; }
        public string HasBeenPublished { get; set; }

        public UcasInstitution UcasInstitution { get; set; }
        public UcasInstitution AccreditingProviderInstitution { get; set; }
        public UcasCampus UcasCampus { get; set; }
        public CourseCode CourseCode { get; set; }
    }
}
