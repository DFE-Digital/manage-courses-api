using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using FluentAssertions;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class TransitionService_Optout_Complex_Tests : TransitionServiceTestsBase
    {
        private const string OptedOutProviderCode = "OptedOut";

        internal override List<Provider> SetupProviders => new List<Provider> { new ProviderBuilder().WithCode(OptedOutProviderCode) };

        private void SetupDbState()
        {
            var optedOut = Context.Providers.First(x => x.ProviderCode == OptedOutProviderCode && x.OptedIn == false);

            var allTypes = Enum.GetValues(typeof(CourseType)).Cast<CourseType>().ToArray();
            Context.Courses.Add(CourseBuilder.Build("allTypes", allTypes, optedOut));

            allTypes = allTypes.Where(x => x != CourseType.RunningPublished).ToArray();

            Context.Courses.Add(CourseBuilder.Build("allTypes_X", allTypes, optedOut));

            for (int i = 0; i < allTypes.Count(); i++)
            {
                Context.Courses.Add(CourseBuilder.Build("allTypes_" + i, allTypes.Skip(i).ToArray(), optedOut));
            }
            Context.Save();
        }

        [Test]
        public void Test_Transitions()
        {
            SetupDbState();

            var transitionData = new []
            {
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes", expectedCount = 1, arrangedCount = 1},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_X", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_0", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_1", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_2", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_3", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_4", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_5", expectedCount = 0, arrangedCount = 0},
                new {providerCode = OptedOutProviderCode, courseCode = "allTypes_6", expectedCount = 0, arrangedCount = 0},
            };

            foreach(var testCase in transitionData)
            {
                var arrangedCourses = Context.GetCourse(testCase.providerCode, testCase.courseCode, Email);

                arrangedCourses.First().CourseSites.Count(
                    x =>
                        "y".Equals(x.Publish, StringComparison.InvariantCultureIgnoreCase) &&
                        x.Status.Equals("r", StringComparison.InvariantCultureIgnoreCase)
                    )
                .Should()
                .Be(testCase.arrangedCount, $"{testCase.courseCode} ");

                Service.UpdateNewCourse(testCase.providerCode, testCase.courseCode, Email);

                var courses = Context.GetCourse(testCase.providerCode, testCase.courseCode, Email);

                courses.First().CourseSites.Count(
                    x =>
                        "y".Equals(x.Publish, StringComparison.InvariantCultureIgnoreCase) &&
                        x.Status.Equals("r", StringComparison.InvariantCultureIgnoreCase)
                    )
                .Should()
                .Be(testCase.expectedCount, $"{testCase.courseCode} ");
            }
        }
    }
}
