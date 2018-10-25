namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class CourseSubject
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public Subject Subject { get; set; }
    }
}
