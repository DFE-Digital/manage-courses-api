using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class DataServiceTests : DbIntegrationTestBase
    {
        public IDataService DataService;

        private const string TestUserEmail1 = "email_1@test-manage-courses.gov.uk";

        protected override void Setup()
        {
            var mockLogger = new Mock<ILogger<DataService>>();
            var mockEnrichmentService = new Mock<IEnrichmentService>();
            mockEnrichmentService.Setup(x => x.GetCourseEnrichmentMetadata(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<UcasCourseEnrichmentGetModel>());

            DataService = new DataService(Context, mockEnrichmentService.Object, mockLogger.Object);
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("     ", "    ")]
        [TestCase("anyone@anywher.com", "")]
        [TestCase("", "providerCode")]
        [TestCase("anyon@anywhere.com", "ABC")]
        public void GetCoursesWithInvalidUserAndInvalidProviderCodeShouldNotReturnNoData(string email, string providerCode)
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);//ensure we have data

            var result = DataService.GetCoursesForUser(email, providerCode);//get the course for each org
            Assert.True(result.Count == 0);
        }

        [Test]
        [TestCase("xxx")]
        [TestCase("   ")]
        [TestCase("")]
        [TestCase(null)]
        public void GetCoursesWithValidUserAndInvalidProviderCodeShouldReturnNoData(string providerCode)
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);

            var result = DataService.GetCoursesForUser(TestUserEmail1, providerCode);//get the course for each org
            Assert.True(result.Count == 0);
        }

        [Test]
        public void GetCoursesWithValidUserAndProviderCodeShouldReturnOrderedData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            string providerCode = "AB1";
            LoadData(TestUserEmail1, numOrgs, numCourses, true);

            var result = DataService.GetCoursesForUser(TestUserEmail1, providerCode);//get the course for each org
            var previousRecordTitleInt = 0;
            foreach (var course in result)
            {
                var recordTitleInt = int.Parse(course.Name.Replace("Title", ""));
                recordTitleInt.Should().BeGreaterThan(previousRecordTitleInt);
                previousRecordTitleInt = recordTitleInt;
            }
        }

        /// <summary>
        /// setup data so we can test
        /// </summary>
        /// <param name="email">email of the selected user</param>
        /// <param name="numOrgs">number og oganisation records to generate</param>
        /// <param name="numCourses">number of course records to generate</param>
        /// <param name="reverseCourseOrder"></param>
        private void LoadData(string email, int numOrgs, int numCourses, bool reverseCourseOrder = false)
        {
            Context.Subjects.RemoveRange(Context.Subjects);
            Context.Save();

            int numSubjects = 3;
            User user = new User { FirstName = "fname", LastName = "lname", Email = email };
            Context.Users.Add(user);
            LoadSubjects(numSubjects);
            for (var counter = 1; counter <= numOrgs; counter++)
            {
                var orgId = "org" + counter;
                var providerCode = "AB" + counter;
                Organisation org = new Organisation { Id = counter, OrgId = orgId, Name = "Organisation " + counter };
                Context.Organisations.Add(org);
                Provider provider = new Provider
                {
                    Address1 = "add2",
                    Address2 = "add2",
                    Address3 = "add3",
                    Address4 = "add4",
                    Postcode = "AB1 CD2",
                    ProviderCode = providerCode,
                    ProviderName = "Provider " + counter
                };
                Context.Providers.Add(provider);
                LoadCourses(provider, numCourses, Context.Subjects, reverseCourseOrder);
                Context.OrganisationUsers.Add(new OrganisationUser { User = user, Organisation = org });
                Context.OrganisationProviders.Add(new OrganisationProvider
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
        /// <param name="provider">provider code</param>
        /// <param name="numRecords">number of course records to generate</param>
        /// <param name="subjects"></param>
        /// /// <param name="reverseCourseOrder">flag for listing courses in reverse order for testing order by</param>
        private void LoadCourses(Provider provider, int numRecords, IEnumerable<Subject> subjects, bool reverseCourseOrder)
        {
            var reverseCounter = numRecords;
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
                    Name = "Title" + (reverseCourseOrder ? reverseCounter : counter),
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
                reverseCounter--;
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
