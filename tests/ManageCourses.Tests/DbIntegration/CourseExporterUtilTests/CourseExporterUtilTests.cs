using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.CourseExporterUtil;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration.CourseExporterUtilTests
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class CourseExporterUtilTests : DbIntegrationTestBase
    {
        private const string OptedInWithExistingCourseProviderCode = "OptedInWithExistingCourses";

        private static IEnumerable<Provider> SetupProviders
        {
            get
            {
                return new List<Provider>
                {
                    new ProviderBuilder().WithCode(OptedInWithExistingCourseProviderCode).WithOptedIn()
                };
            }
        }

        protected override void Setup()
        {
            var providers = SetupProviders;
            Context.Providers.AddRange(providers);
            Context.SaveChanges();
            SetupDbState();
        }

        [Test]
        public void ReadAllCourseData_Returns_Courses()
        {
            var publisher = new Publisher(Config);
            var searchCourses = publisher.ReadAllCourseData(Context);
            searchCourses.Should().HaveCount(1);
        }

        // lifted from ManageCourses.Tests/DbIntegration/TransitionService_Intervention_Simple_Tests.cs
        private void SetupDbState()
        {
            var optedInWithExistingCourse = Context.Providers
                .First(x => x.ProviderCode == OptedInWithExistingCourseProviderCode && x.OptedIn);

            var optedInProviderExistingCourse = CourseBuilder
                .Build(CourseType.RunningPublished, optedInWithExistingCourse)
                .WithName("Poetry");

            Context.Courses.Add(optedInProviderExistingCourse);

            Context.Save();
        }

    }
}
