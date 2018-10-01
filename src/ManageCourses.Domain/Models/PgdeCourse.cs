namespace GovUk.Education.ManageCourses.Domain.Models
{
    /// <summary>
    /// List of courses that should always be shown as PGDE
    /// </summary>
    public class PgdeCourse
    {
        public int Id { get; set; }
        public string InstCode { get; set; }
        public string CourseCode { get; set; }
    }
}
