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
        public void GetOrganisationsShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var result = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            Assert.IsTrue(result.Count == numOrgs);
            Assert.IsTrue(result.All(c => c.TotalCourses == numCourses));
        }

        [Test]
        public void GetOrganisationsShouldReturnNoData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var result = DataService.GetInstitutionSummariesForUser("anyone@testing.com").ToList();//try to get the list using an invalid email
            Assert.IsTrue(result.Count == 0);
            Assert.IsTrue(result.All(c => c.TotalCourses == 0));
        }

        [Test]
        public void GetOrganisationShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();

            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)//we have a valid list of data
            {
                var result = DataService.GetInstitutionSummaryForUser(TestUserEmail1, org.InstCode);//get the organisation
                Assert.IsTrue(result.InstName == org.InstName);
            }
        }

        [Test]
        public void GetOrganisationShouldReturnNull()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var result = DataService.GetInstitutionSummaryForUser("anyone@testing.com", org.InstCode);//try to get the organisation using an invalid email
                Assert.IsNull(result);
            }
        }

        [Test]
        public void GetCoursesShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var result = DataService.GetCoursesForUser(TestUserEmail1, org.InstCode);//get the course for each org
                Assert.AreEqual(numCourses, result.Count);
            }
        }
        [Test]
        public void GetCoursesShouldReturnStatus()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            orgList.Count.Should().Be(numOrgs);

            foreach (var org in orgList)
            {
                var result = DataService.GetCoursesForUser(TestUserEmail1, org.InstCode);//get the course for each org

                foreach (var course in result)
                {
                    course.CourseSites.All(s => s.Status == "N").Should().BeTrue();
                }
            }
        }

        [Test]
        public void GetCoursesShouldReturnPublish()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            orgList.Count.Should().Be(numOrgs);

            foreach (var org in orgList)
            {
                var result = DataService.GetCoursesForUser(TestUserEmail1, org.InstCode);//get the course for each org

                foreach (var course in result)
                {
                    course.CourseSites.All(s => s.Publish == "N").Should().BeTrue();
                }
            }
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("     ", "    ")]
        [TestCase("anyone@anywher.com", "")]
        [TestCase("", "instCode")]
        [TestCase("anyon@anywhere.com", "ABC")]
        public void GetCoursesWithInvalidUserAndInvalidInstCodeShouldNotReturnNoData(string email, string instCode)
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);//ensure we have data

            var result = DataService.GetCoursesForUser(email, instCode);//get the course for each org
            Assert.True(result.Count == 0);
        }

        [Test]
        public void GetCoursesWithInvalidUserAndValidInstCodeShouldNotReturnNoData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var result = DataService.GetCoursesForUser("anyone@anywhere.com", org.InstCode);//get the course for each org
                Assert.True(result.Count == 0);
            }
        }

        [Test]
        [TestCase("xxx")]
        [TestCase("   ")]
        [TestCase("")]
        [TestCase(null)]
        public void GetCoursesWithValidUserAndInvalidInstCodeShouldNotReturnNoData(string instCode)
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);

            var result = DataService.GetCoursesForUser(TestUserEmail1, instCode);//get the course for each org
            Assert.True(result.Count == 0);
        }

        [Test]
        public void GetCourseShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var coursesList = DataService.GetCoursesForUser(TestUserEmail1, org.InstCode);//get the courses for each org
                foreach (var course in coursesList)
                {
                    var result = DataService.GetCourseForUser(TestUserEmail1, org.InstCode, course.CourseCode);
                    Assert.IsTrue(course != null && result.Name == course.Name);
                }
            }
        }

        [Test]
        public void GetCourseWithInvalidUserShouldReturnNoData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetInstitutionSummariesForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var coursesList = DataService.GetCoursesForUser(TestUserEmail1, org.InstCode);//get the courses for each org
                foreach (var course in coursesList)
                {
                    var result = DataService.GetCourseForUser("someone@somewhere.com", org.InstCode, course.CourseCode);
                    result.Should().BeNull();
                }
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
            LoadSubjects(numSubjects);
            for (var counter = 1; counter <= numOrgs; counter++)
            {
                var orgId = "org" + counter;
                var instCode = "AB" + counter;
                Organisation org = new Organisation { Id = counter, OrgId = orgId, Name = "Organisation " + counter };
                Context.Organisations.Add(org);
                Institution institution = new Institution
                {
                    Address1 = "add2",
                    Address2 = "add2",
                    Address3 = "add3",
                    Address4 = "add4",
                    Postcode = "AB1 CD2",
                    InstCode = instCode,
                    InstName = "Intitution " + counter
                };
                Context.Institutions.Add(institution);
                LoadCourses(institution, numCourses, Context.Subjects);
                Context.OrganisationUsers.Add(new OrganisationUser { User = user, Organisation = org });
                Context.OrganisationIntitutions.Add(new OrganisationInstitution
                {
                    Institution = institution,
                    Organisation = org
                });
            }

            Context.Save();
        }

        /// <summary>
        /// Generates course records for a specific institution
        /// </summary>
        /// <param name="instCode">institution code</param>
        /// <param name="numRecords">number of course records to generate</param>
        /// <param name="numSubjects"></param>
        private void LoadCourses(Institution institution, int numRecords, IEnumerable<Subject> subjects)
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
                    Institution = institution
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
                    Institution = institution,
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
