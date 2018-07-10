namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasCourseSubject
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CrseCode { get; set; }
        public string SubjectCode { get; set; }
        public string YearCode { get; set; }

        public UcasInstitution UcasInstitution { get; set; }
        public CourseCode CourseCode { get; set; }
        public UcasSubject UcasSubject { get; set; }
    }
}
