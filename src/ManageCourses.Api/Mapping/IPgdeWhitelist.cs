using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    /// <summary>
    /// A thing that can figure out whether a course is PGDE (PostGraduate Diploma in Education)
    /// </summary>
    public interface IPgdeWhitelist
    {
        /// <summary>
        /// Decide if this course is PGDE
        /// </summary>
        bool IsPgde(string instCode, string courseCode);
        IEnumerable<PgdeCourse> ForInstitution(string instCode);
    }
}
