namespace GovUk.Education.ManageCourses.Api.Model
{
    public class Course
    {
        public string UcasInstitutionCode { get; set; }
        public string UcasCourseCode { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }//concatenated string of field data
    }
}
