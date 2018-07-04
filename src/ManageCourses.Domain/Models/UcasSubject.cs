using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class UcasSubject
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectDescription { get; set; }
        public string TitleMatch { get; set; }

        public ICollection<UcasCourseSubject> UcasCourseSubjects { get; set; }
    }
}
