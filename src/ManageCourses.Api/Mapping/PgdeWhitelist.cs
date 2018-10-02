using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class PgdeWhitelist : IPgdeWhitelist
    {
        private readonly IManageCoursesDbContext _manageCoursesDbContext;

        public PgdeWhitelist(IManageCoursesDbContext manageCoursesDbContext)
        {
            _manageCoursesDbContext = manageCoursesDbContext;
        }

        public IEnumerable<PgdeCourse> ForInstitution(string instCode)
        {
            instCode = instCode.ToUpperInvariant();
            return _manageCoursesDbContext.PgdeCourses.Where(x => x.InstCode == instCode).ToList();
        }

        public bool IsPgde(string instCode, string courseCode)
        {
            instCode = instCode.ToUpperInvariant();
            courseCode = courseCode.ToUpperInvariant();
            return _manageCoursesDbContext.PgdeCourses.Any(p => p.InstCode == instCode && p.CourseCode == courseCode);
        }
    }
}
