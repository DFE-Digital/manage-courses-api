using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    public class DbContextTests : DbIntegrationTestBase
    {
        private const string Email = "someone@example.org";
        private const string ProviderCode = "DD3";
        private const string CourseCode = "EE8E";
        private const string CourseName = "Everything Ever";
        private const string Year2020 = "2020";
        private Organisation _organisation;

        protected override void Setup()
        {
            Provider provider = new ProviderBuilder()
                .WithCode(ProviderCode)
                .WithCycle(Context.RecruitmentCycles.Single(rc => rc.Year == RecruitmentCycle.CurrentYear));
            Context.Courses.Add(new CourseBuilder()
                .WithCode(CourseCode)
                .WithProvider(provider)
                .WithName(CourseName)
            );
            _organisation = new Organisation
            {
                OrganisationUsers = new List<OrganisationUser>
                {
                    new OrganisationUser
                    {
                        User = new User
                        {
                            Email = Email,
                        }
                    }
                }
            };
            provider.OrganisationProviders = new List<OrganisationProvider>
            {
                new OrganisationProvider
                {
                    Organisation = _organisation
                }
            };
           Context.SaveChanges();
        }

        [Test]
        public void GetCourse_BeforeRollover_Returns2019Course()
        {
            GetCourse_Returns_2019Course();
        }

        [Test]
        public void GetCourse_AfterRollover_Returns2019Course()
        {
            AddRolloverData();
            GetCourse_Returns_2019Course();
        }

        [Test]
        public void GetCoursesByProviderCode_BeforeRollover_Returns_2019Courses()
        {
            GetCoursesByProviderCode_Returns_2019Courses();
        }

        [Test]
        public void GetCoursesByProviderCode_AfterRollover_Returns_2019Courses()
        {
            AddRolloverData();
            GetCoursesByProviderCode_Returns_2019Courses();
        }

        [Test]
        public void GetOrganisationProvider_BeforeRollover_Returns_2019Provider()
        {
            GetOrganisationProvider_Returns_2019Provider();
        }

        [Test]
        public void GetOrganisationProvider_AfterRollover_Returns_2019Provider()
        {
            AddRolloverData();
            GetOrganisationProvider_Returns_2019Provider();
        }

        [Test]
        public void GetOrganisationProviders_BeforeRollover_Returns_2019Provider()
        {
            GetOrganisationProviders_Returns_2019Provider();
        }

        [Test]
        public void GetOrganisationProviders_AfterRollover_Returns_2019Provider()
        {
            AddRolloverData();
            GetOrganisationProviders_Returns_2019Provider();
        }

        [Test]
        public void GetProvider_BeforeRollover_Returns_2019Provider()
        {
            GetProvider_Returns_2019Provider();
        }

        [Test]
        public void GetProvider_AfterRollover_Returns_2019Provider()
        {
            AddRolloverData();
            GetProvider_Returns_2019Provider();
        }

        private void GetCourse_Returns_2019Course()
        {
            var courses = Context.GetCourse(ProviderCode, CourseCode, Email);
            courses.Count.Should().Be(1);
            courses.First().Name.Should().Be(CourseName);
        }

        private void GetCoursesByProviderCode_Returns_2019Courses()
        {
            var courses = Context.GetCoursesByProviderCode(ProviderCode, Email);
            courses.Count.Should().Be(1);
            courses.First().Name.Should().Be(CourseName);
        }

        private void GetOrganisationProvider_Returns_2019Provider()
        {
            var organisationProvider = Context.GetOrganisationProvider(Email, ProviderCode);
            organisationProvider.Should().NotBeNull();
            organisationProvider.Provider.RecruitmentCycle.Year.Should().Be(RecruitmentCycle.CurrentYear);
        }

        private void GetOrganisationProviders_Returns_2019Provider()
        {
            var organisationProviders = Context.GetOrganisationProviders(Email);
            organisationProviders.Should().NotBeNull();
            organisationProviders.Count().Should().Be(1);
            organisationProviders.Single().Provider.RecruitmentCycle.Year.Should().Be(RecruitmentCycle.CurrentYear);
        }

        private void GetProvider_Returns_2019Provider()
        {
            var provider = Context.GetProvider(Email, ProviderCode);
            provider.Should().NotBeNull();
            provider.RecruitmentCycle.Year.Should().Be(RecruitmentCycle.CurrentYear);
        }

        private void AddRolloverData()
        {
            // add a provider/course in the next cycle to make sure we're getting the right one
            Provider provider2020 = new ProviderBuilder()
                .WithCode(ProviderCode)
                .WithCycle(Context.RecruitmentCycles.Single(rc => rc.Year == Year2020));
            Context.Courses.Add(new CourseBuilder()
                .WithCode(CourseCode)
                .WithProvider(provider2020)
                .WithName(CourseName + " 2020")
            );
            _organisation.OrganisationProviders.Add(
                new OrganisationProvider
                {
                    Provider = provider2020
                });
            Context.SaveChanges();
            Context.Courses.Count().Should().Be(2);
        }
    }
}
