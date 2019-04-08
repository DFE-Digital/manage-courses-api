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
    public class TransitionService_Optout_Simple_Tests : TransitionServiceTestsBase
    {
        private const string OptedOutProviderCode = "OptedOut";

        internal override List<Provider> SetupProviders => new List<Provider> { new ProviderBuilder().WithCode(OptedOutProviderCode) };

        private void SetupDbState()
        {
            var optedOut = Context.Providers.First(x => x.ProviderCode == OptedOutProviderCode && x.OptedIn == false);

            // Relic data

            Context.Courses.Add(CourseBuilder.Build(CourseType.DiscontinuedUnpublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.DiscontinuedPublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.NewUnpublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.NewPublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.RunningUnpublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.RunningPublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.SuspensedUnpublished, optedOut));
            Context.Courses.Add(CourseBuilder.Build(CourseType.SuspensedPublished, optedOut));

            Context.Save();
        }

        [Test]
        public void Test_Transitions()
        {
            SetupDbState();

            var runningAnyWay =
                new {providerCode = OptedOutProviderCode, arrangedStatus = "R", arrangedPublish = "Y", courseCode = RunningPublishedCourseCode};

            Test_UpdateNewCourse(runningAnyWay, true);

            var noOpsTestCases = new[] {
                new {providerCode = OptedOutProviderCode, arrangedStatus = "R", arrangedPublish = "N", courseCode = RunningUnpublishedCourseCode},

                new {providerCode = OptedOutProviderCode, arrangedStatus = "D", arrangedPublish = "N", courseCode = DiscontinuedUnpublishedCourseCode},
                new {providerCode = OptedOutProviderCode, arrangedStatus = "D", arrangedPublish = "Y", courseCode = DiscontinuedPublishedCourseCode},
                new {providerCode = OptedOutProviderCode, arrangedStatus = "S", arrangedPublish = "N", courseCode = SuspensedUnpublishedCourseCode},
                new {providerCode = OptedOutProviderCode, arrangedStatus = "S", arrangedPublish = "Y", courseCode = SuspensedPublishedCourseCode},

                new {providerCode = OptedOutProviderCode, arrangedStatus = "N", arrangedPublish = "N", courseCode = NewUnpublishedCourseCode},
                new {providerCode = OptedOutProviderCode, arrangedStatus = "N", arrangedPublish = "Y", courseCode = NewPublishedCourseCode},
            };

            foreach(var testCase in noOpsTestCases)
            {
                Test_UpdateNewCourse(testCase, false);
            }
        }
    }
}
