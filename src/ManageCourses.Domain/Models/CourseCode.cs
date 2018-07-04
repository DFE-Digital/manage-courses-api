using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    /// <summary>
    /// This is an implicit join table in the ucas source data.
    /// The source data links from Ucas_Course_Subject to Course on
    /// inst_code and crse_code but ucas_course is unique on campus as well.
    /// This table holds the unique combinations of inst_code and crse_code
    /// so that we can complete the chain of foreign keys
    /// </summary>
    public class CourseCode
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CrseCode { get; set; }

        public UcasInstitution UcasInstitution { get; set; }
        public ICollection<UcasCourse> UcasCourses { get; set; }
        public ICollection<UcasCourseSubject> UcasCourseSubjects { get; set; }
    }
}
