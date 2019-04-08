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
    public class TransitionService_Optin_Simple_Tests : TransitionServiceTestsBase
    {
        private const string OptedInProviderCode = "OptedIn";

        internal override List<Provider> SetupProviders => new List<Provider> { new ProviderBuilder().WithCode(OptedInProviderCode).WithOptedIn() };

        private void SetupDbState()
        {
            var optedIn = Context.Providers.First(x => x.ProviderCode == OptedInProviderCode && x.OptedIn);

            // Relic data
            Context.Courses.Add(CourseBuilder.Build(CourseType.SuspensedUnpublished, optedIn));
            Context.Courses.Add(CourseBuilder.Build(CourseType.SuspensedPublished, optedIn));
            Context.Courses.Add(CourseBuilder.Build(CourseType.DiscontinuedUnpublished, optedIn));
            Context.Courses.Add(CourseBuilder.Build(CourseType.DiscontinuedPublished, optedIn));


            // Transition has started
            // New course has been added to database hence course is new as the course site is new
            Context.Courses.Add(CourseBuilder.Build(CourseType.NewUnpublished, optedIn));
            Context.Courses.Add(CourseBuilder.Build(CourseType.NewPublished, optedIn));

            // Somehow course has a running site
            Context.Courses.Add(CourseBuilder.Build(CourseType.RunningUnpublished, optedIn));
            Context.Courses.Add(CourseBuilder.Build(CourseType.RunningPublished, optedIn));

            Context.Save();
        }

        [Test]
        public void Test_Transitions()
        {
            SetupDbState();

            var noOpsTestCases = new[] {

                new {providerCode = OptedInProviderCode, arrangedStatus = "S", arrangedPublish = "N", courseCode = SuspensedUnpublishedCourseCode},
                new {providerCode = OptedInProviderCode, arrangedStatus = "S", arrangedPublish = "Y", courseCode = SuspensedPublishedCourseCode},
                new {providerCode = OptedInProviderCode, arrangedStatus = "D", arrangedPublish = "N", courseCode = DiscontinuedUnpublishedCourseCode},
                new {providerCode = OptedInProviderCode, arrangedStatus = "D", arrangedPublish = "Y", courseCode = DiscontinuedPublishedCourseCode},
            };

            foreach(var testCase in noOpsTestCases)
            {
                Test_UpdateNewCourse(testCase, false);
            }

            var transitionData = new []
            {
                new {providerCode = OptedInProviderCode, arrangedStatus = "N", arrangedPublish = "N", courseCode = NewUnpublishedCourseCode},
                new {providerCode = OptedInProviderCode, arrangedStatus = "N", arrangedPublish = "Y", courseCode = NewPublishedCourseCode},
                new {providerCode = OptedInProviderCode, arrangedStatus = "R", arrangedPublish = "N", courseCode = RunningUnpublishedCourseCode},
                // This is actually a no ops
                new {providerCode = OptedInProviderCode, arrangedStatus = "R", arrangedPublish = "Y", courseCode = RunningPublishedCourseCode},
            };

            foreach(var testCase in transitionData)
            {
                Test_UpdateNewCourse(testCase, true);
            }
        }
    }
}
