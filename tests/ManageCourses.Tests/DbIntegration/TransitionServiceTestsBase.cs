using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public abstract class TransitionServiceTestsBase : DbIntegrationTestBase
    {
        internal static string NewUnpublishedCourseCode = CourseBuilder.Build(CourseType.NewUnpublished).GetCode();
        internal static string NewPublishedCourseCode = CourseBuilder.Build(CourseType.NewPublished).GetCode();
        internal static string SuspensedUnpublishedCourseCode = CourseBuilder.Build(CourseType.SuspensedUnpublished).GetCode();
        internal static string SuspensedPublishedCourseCode = CourseBuilder.Build(CourseType.SuspensedPublished).GetCode();
        internal static string RunningUnpublishedCourseCode = CourseBuilder.Build(CourseType.RunningUnpublished).GetCode();
        internal static string RunningPublishedCourseCode = CourseBuilder.Build(CourseType.RunningPublished).GetCode();
        internal static string DiscontinuedUnpublishedCourseCode = CourseBuilder.Build(CourseType.DiscontinuedUnpublished).GetCode();
        internal static string DiscontinuedPublishedCourseCode = CourseBuilder.Build(CourseType.DiscontinuedPublished).GetCode();
        internal ITransitionService Service;

        internal const string Email = "12345@example.org";

        internal abstract List<Provider> SetupProviders {get;}
        protected override void Setup()
        {
            Service = new TransitionService(Context);


            var providers = SetupProviders
                .Select(x => new OrganisationProvider
                    {
                        Provider = x,
                    }).ToList();

            var currentRecruitmentCycle = new RecruitmentCycle{ Year = RecruitmentCycle.CurrentYear};

            foreach (var item in providers)
            {
                item.Provider.RecruitmentCycle = currentRecruitmentCycle;
            }

            var user = new User
            {
                Email = Email,
            };

            Context.Add(user);

            var org = new Organisation
            {
                Name = "Bucks Mega Org",
                OrgId = "BMO1",
                OrganisationUsers = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        User = user,
                    },
                },
                OrganisationProviders = providers
            };
            Context.Add(org);

            Context.SaveChanges();
        }

        private void Test_RunningAndPublished(List<Course> courses, string status, string publish, string courseCode, bool isRunningAndPublished)
        {
            courses.Should().HaveCount(1);
            courses.First().CourseSites.Any(x =>
                "y".Equals(x.Publish, StringComparison.InvariantCultureIgnoreCase) &&
                x.Status.Equals("r", StringComparison.InvariantCultureIgnoreCase)
            ).Should().Be(isRunningAndPublished, $"{courseCode} {status} {publish}");
        }
        private void Test_ArrangedState(List<Course> arrangedCourses, string status, string publish, string courseCode)
        {
            arrangedCourses.Should().HaveCount(1);
            arrangedCourses.First().CourseSites.All(x => x.Status.Equals(status, StringComparison.InvariantCultureIgnoreCase) &&  x.Publish.Equals(publish, StringComparison.InvariantCultureIgnoreCase)).Should().BeTrue($"{courseCode} {status} {publish}");
        }

        internal void Test_UpdateNewCourse(dynamic testCase, bool isRunningAndPublished)
        {
            var arrangedCourses = Context.GetCourse(testCase.providerCode, testCase.courseCode, Email);
            Test_ArrangedState(arrangedCourses, testCase.arrangedStatus, testCase.arrangedPublish, testCase.courseCode);

            Service.UpdateNewCourse(testCase.providerCode, testCase.courseCode, Email);

            var courses = Context.GetCourse(testCase.providerCode, testCase.courseCode, Email);

            Test_RunningAndPublished(courses, testCase.arrangedStatus, testCase.arrangedPublish, testCase.courseCode, isRunningAndPublished);
        }
    }
}
