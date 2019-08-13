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
    public class TransitionService_Optin_Complex_Tests : TransitionServiceTestsBase
    {
        private const string OptedInProviderCode = "OptedIn";

        internal override List<Provider> SetupProviders => new List<Provider> { new ProviderBuilder().WithCode(OptedInProviderCode).WithOptedIn() };


        private void SetupDbState()
        {
            var optedIn = Context.Providers.First(x => x.ProviderCode == OptedInProviderCode && x.OptedIn);

            var allTypes = Enum.GetValues(typeof(CourseType)).Cast<CourseType>().ToArray();
            Context.Courses.Add(CourseBuilder.Build("allTypes", allTypes, optedIn));

            allTypes = allTypes.Where(x => x != CourseType.RunningPublished).ToArray();

            Context.Courses.Add(CourseBuilder.Build("allTypes_1", allTypes, optedIn));

            allTypes = allTypes.Where(x => x != CourseType.RunningUnpublished).ToArray();

            Context.Courses.Add(CourseBuilder.Build("allTypes_2", allTypes, optedIn));

            allTypes = allTypes.Where(x => x != CourseType.NewUnpublished).ToArray();

            Context.Courses.Add(CourseBuilder.Build("allTypes_3", allTypes, optedIn));

            allTypes = allTypes.Where(x => x != CourseType.NewPublished).ToArray();

            Context.Courses.Add(CourseBuilder.Build("allTypes_4", allTypes, optedIn));
            Context.Save();
        }

        [Test]
        public void Test_Transitions()
        {
            SetupDbState();

            var transitionData = new []
            {
                new {providerCode = OptedInProviderCode, courseCode = "allTypes", expectedCount = 4, arrangedCount = 1},
                new {providerCode = OptedInProviderCode, courseCode = "allTypes_1", expectedCount = 3, arrangedCount = 0},
                new {providerCode = OptedInProviderCode, courseCode = "allTypes_2", expectedCount = 2, arrangedCount = 0},
                new {providerCode = OptedInProviderCode, courseCode = "allTypes_3", expectedCount = 1, arrangedCount = 0},
                new {providerCode = OptedInProviderCode, courseCode = "allTypes_4", expectedCount = 0, arrangedCount = 0},
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
