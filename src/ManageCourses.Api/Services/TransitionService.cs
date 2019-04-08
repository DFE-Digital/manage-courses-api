using System;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Api.Services
{
    public class TransitionService : ITransitionService
    {
        private IManageCoursesDbContext _context;

        public TransitionService(IManageCoursesDbContext context)
        {
            _context = context;
        }

        private bool OnlyNewOrRunningAndUnpublished(CourseSite courseSite)
        {
            var isNew = "N".Equals(courseSite.Status, StringComparison.InvariantCultureIgnoreCase);

            var isRunningAndUnPublished =
                "R".Equals(courseSite.Status, StringComparison.InvariantCultureIgnoreCase) &&
                "N".Equals(courseSite.Publish, StringComparison.InvariantCultureIgnoreCase);

            return isNew || isRunningAndUnPublished;
        }

        public void UpdateNewCourse(string providerCode, string courseCode, string email)
        {
            var optedInNewCourse = _context.GetCourse(providerCode, courseCode, email)
                .FirstOrDefault(c =>
                    c.Provider.OptedIn &&
                    c.CourseSites != null &&
                    c.CourseSites.Any(OnlyNewOrRunningAndUnpublished));

            if (optedInNewCourse != null)
            {
                foreach(var cs in optedInNewCourse.CourseSites.Where(OnlyNewOrRunningAndUnpublished))
                {
                    cs.Publish = "Y";
                    cs.Status = "R";
                }

                _context.Save();
            }
        }
    }
}
