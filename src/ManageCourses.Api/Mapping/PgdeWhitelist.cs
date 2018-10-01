using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class PgdeWhitelist : IPgdeWhitelist
    {
        private readonly IManageCoursesDbContext _manageCoursesDbContext;

        public PgdeWhitelist(IManageCoursesDbContext manageCoursesDbContext)
        {
            _manageCoursesDbContext = manageCoursesDbContext;
        }

        public bool IsPgde(string instCode, string courseCode)
        {
            instCode = instCode.ToUpperInvariant();
            courseCode = courseCode.ToUpperInvariant();
            return _manageCoursesDbContext.PgdeCourses.Any(p => p.InstCode == instCode && p.CourseCode == courseCode);
        }
    }
}
