using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }

        public ICollection<CourseSubject> CourseSubjects { get; set; }
    }
}
