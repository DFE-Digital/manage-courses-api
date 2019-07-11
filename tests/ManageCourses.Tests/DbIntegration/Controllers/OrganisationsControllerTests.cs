using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Controllers;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    // [Explicit]
    public class OrganisationsControllerTests : DbIntegrationTestBase
    {
        public OrganisationsController organisationsController;

        private const string TestUserEmail1 = "email_1@test-manage-courses.gov.uk";
        private const string UnauthorisedUserEmail = "not_authorised@test-manage-courses.gov.uk";

        protected override void Setup()
        {
            var mockEnrichmentService = new Mock<IEnrichmentService>();
            mockEnrichmentService.Setup(x => x.GetCourseEnrichmentMetadata(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<UcasCourseEnrichmentGetModel>());

            organisationsController = new OrganisationsController(Context, mockEnrichmentService.Object);
        }

        [Test]
        public void GetAll_Ok()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            organisationsController.SetControllerContext(TestUserEmail1);
            var result = organisationsController.GetAll();

            result.Should().NotBeOfType<UnauthorizedResult>();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAll_Unauthorized()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);

            organisationsController.SetControllerContext(UnauthorisedUserEmail);

            var result = organisationsController.GetAll();

            result.Should().BeOfType<UnauthorizedResult>();
            result.Should().NotBeOfType<OkResult>();
        }

        [Test]
        public void Get_Ok()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            organisationsController.SetControllerContext(TestUserEmail1);
            var result = organisationsController.GetAll();

            result.Should().NotBeOfType<UnauthorizedResult>();
            result.Should().BeOfType<OkObjectResult>();

            var orgList =((List<ProviderSummary>) ((OkObjectResult)result).Value).ToList();

            orgList.All(c => c.TotalCourses == numCourses).Should().BeTrue();

            foreach (var org in orgList)
            {
                var orgResult = organisationsController.Get(org.ProviderCode);


                orgResult.Should().NotBeOfType<UnauthorizedResult>();
                orgResult.Should().BeOfType<OkObjectResult>();

                var orgItem =((ProviderSummary) ((OkObjectResult)orgResult).Value);

                orgItem.ProviderName.Should().Equals(org.ProviderName);
            }
        }

        [Test]
        public void Get_NotFound()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            organisationsController.SetControllerContext(TestUserEmail1);
            var result = organisationsController.GetAll();

            result.Should().NotBeOfType<UnauthorizedResult>();
            result.Should().BeOfType<OkObjectResult>();

            var orgList =((List<ProviderSummary>) ((OkObjectResult)result).Value).ToList();

            orgList.All(c => c.TotalCourses == numCourses).Should().BeTrue();

            organisationsController.SetControllerContext(UnauthorisedUserEmail);

            foreach (var org in orgList)
            {
                var orgResult = organisationsController.Get(org.ProviderCode);

                orgResult.Should().NotBeOfType<OkObjectResult>();
                orgResult.Should().BeOfType<NotFoundResult>();
            }
        }

        /// <summary>
        /// setup data so we can test
        /// </summary>
        /// <param name="email">email of the selected user</param>
        /// <param name="numOrgs">number og oganisation records to generate</param>
        /// <param name="numCourses">number of course records to generate</param>
        private void LoadData(string email, int numOrgs, int numCourses)
        {
            Context.Subjects.RemoveRange(Context.Subjects);
            Context.Save();

            int numSubjects = 3;
            User user = new User { FirstName = "fname", LastName = "lname", Email = email };
            Context.Users.Add(user);

            var recruitmentCycle = new RecruitmentCycle {Year = RecruitmentCycle.CurrentYear};

            LoadSubjects(numSubjects);
            for (var counter = 1; counter <= numOrgs; counter++)
            {
                var orgId = "org" + counter;
                var providerCode = "AB" + counter;
                Organisation org = new Organisation { Id = counter, OrgId = orgId, Name = "Organisation " + counter };
                Context.Organisations.Add(org);
                Provider provider = new Provider()
                {
                    Address1 = "add2",
                    Address2 = "add2",
                    Address3 = "add3",
                    Address4 = "add4",
                    Postcode = "AB1 CD2",
                    ProviderCode = providerCode,
                    ProviderName = "Intitution " + counter,
                    RecruitmentCycle = recruitmentCycle
                };
                Context.Providers.Add(provider);
                LoadCourses(provider, numCourses, Context.Subjects);
                Context.OrganisationUsers.Add(new OrganisationUser { User = user, Organisation = org });
                Context.OrganisationProviders.Add(new OrganisationProvider()
                {
                    Provider = provider,
                    Organisation = org
                });
            }

            Context.Save();
        }

        /// <summary>
        /// Generates course records for a specific provider
        /// </summary>
        /// <param name="provider">provider</param>
        /// <param name="numRecords">number of course records to generate</param>
        /// <param name="subjects"></param>
        private void LoadCourses(Provider provider, int numRecords, IEnumerable<Subject> subjects)
        {
            for (var counter = 1; counter <= numRecords; counter++)
            {
                var courseCode = "CC" + counter;
                var campusCode = "C" + counter;

                var site = new Site
                {
                    Code = campusCode,
                    Address1 = "add1",
                    Address2 = "add2",
                    Address3 = "add3",
                    Address4 = "add4",
                    Postcode = "PC1 A23",
                    LocationName = "Campus " + counter,
                    Provider = provider
                };

                var course = new Course
                {
                    AgeRange = "P",
                    CourseCode = courseCode,
                    StartDate = new DateTime(2018, 10, 16),
                    ProfpostFlag = "PG",
                    ProgramType = "SC",
                    StudyMode = "F",
                    Name = "Title " + counter,
                    Provider = provider,
                    CourseSites =  new List<CourseSite>()
                    {
                        new CourseSite
                        {
                            Status = "N",
                            Publish = "N",
                            Site = site
                        }
                    }
                };
                Context.Courses.Add(course);
                Context.Sites.Add(site);

                LoadCourseSubjects(course, subjects);
            }
        }

        private void LoadSubjects(int numRecords)
        {
            for (var counter = 1; counter <= numRecords; counter++)
            {
                Context.Subjects.Add(new Subject
                {
                    SubjectName = "subject " + counter,
                    SubjectCode = "SC"+counter
                });
            }
        }

        private void LoadCourseSubjects(Course course, IEnumerable<Subject> subjects)
        {
            foreach (var subject in subjects)
            {
                Context.CourseSubjects.Add(new CourseSubject
                {
                    Course = course,
                    Subject = subject
                });
            }
        }
    }
}
