using System.ComponentModel.DataAnnotations;
using System;
namespace GovUk.Education.ManageCourses.Domain.Models
{
    /// <summary>
    /// List of courses that should always be shown as PGDE
    /// </summary>
    [Obsolete("This was used when Ucas is the single source of truth")]
    public class PgdeCourse
    {
        public int Id { get; set; }

        [Required]
        public string ProviderCode { get; set; }
        [Required]
        public string CourseCode { get; set; }
    }
}
