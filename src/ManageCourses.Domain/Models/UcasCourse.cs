namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasCourse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CourseCode { get; set; }
        public string InstCode { get; set; }
        public string StudyMode { get; set; }
        public string AgeGroup { get; set; }
        public string CampusCode { get; set; }
        public string ProfPostFlag { get; set; }
        public string ProgramType { get; set; }
        public string AcreditedProvider { get; set; }
        public string OpenDate { get; set; }
    }
}
