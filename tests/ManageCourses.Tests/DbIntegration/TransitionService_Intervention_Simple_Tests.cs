using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class TransitionService_Intervention_Simple_Tests : TransitionServiceTestsBase
    {
        private const string OptedInWithExistingCourseProviderCode = "OptedInWithExistingCourses";

        internal override List<Provider> SetupProviders => new List<Provider> { new ProviderBuilder().WithCode(OptedInWithExistingCourseProviderCode).WithOptedIn() };


        private void SetupDbState()
        {
            var optedInWithExistingCourse = Context.Providers.First(x => x.ProviderCode == OptedInWithExistingCourseProviderCode && x.OptedIn == true);

            // There is a manuel intervention that addresses concerning existing course that are new for an opted in provider.
            var optedInProviderExistingCourse = CourseBuilder.Build(CourseType.NewUnpublished, optedInWithExistingCourse);

            optedInProviderExistingCourse.Update(x => {
                // The manuel intervention, replace it with running & published status
                x.CourseSites = new List<CourseSite> {CourseSiteBuilder.Build(CourseType.RunningPublished)};
            });

            Context.Courses.Add(optedInProviderExistingCourse);

            Context.Save();
        }

        [Test]
        public void Test_Transitions()
        {
            SetupDbState();

            var interventionData =
                new {providerCode = OptedInWithExistingCourseProviderCode, arrangedStatus = "R", arrangedPublish = "Y", courseCode = NewUnpublishedCourseCode};

            Test_UpdateNewCourse(interventionData, true);
        }
    }
}
