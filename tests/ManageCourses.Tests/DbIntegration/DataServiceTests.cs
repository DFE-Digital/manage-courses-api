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
            var result = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            Assert.IsTrue(result.Count == numOrgs);
            Assert.IsTrue(result.All(c => c.TotalCourses == numCourses));
        }

        [Test]
        public void GetOrganisationsShouldReturnNoData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var result = DataService.GetOrganisationsForUser("anyone@testing.com").ToList();//try to get the list using an invalid email
            Assert.IsTrue(result.Count == 0);
            Assert.IsTrue(result.All(c => c.TotalCourses == 0));
        }

        [Test]
        public void GetOrganisationShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();

            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)//we have a valid list of data
            {
                var result = DataService.GetOrganisationForUser(TestUserEmail1, org.UcasCode);//get the organisation
                Assert.IsTrue(result.OrganisationName == org.OrganisationName);
            }
        }

        [Test]
        public void GetOrganisationShouldReturnNull()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var result = DataService.GetOrganisationForUser("anyone@testing.com", org.UcasCode);//try to get the organisation using an invalid email
                Assert.IsNull(result);
            }
        }

        [Test]
        public void GetCoursesShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var result = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the course for each org
                Assert.AreEqual(numCourses, result.Courses.Count);
            }
        }
        [Test]
        public void GetCoursesShouldReturnStatus()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            orgList.Count.Should().Be(numOrgs);

            foreach (var org in orgList)
            {
                var result = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the course for each org

                foreach (var course in result.Courses)
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
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            orgList.Count.Should().Be(numOrgs);

            foreach (var org in orgList)
            {
                var result = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the course for each org

                foreach (var course in result.Courses)
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
        [TestCase("", "ucasCode")]
        [TestCase("anyon@anywhere.com", "ABC")]
        public void GetCoursesWithInvalidUserAndInvalidUcasCodeShouldNotReturnNoData(string email, string ucasCode)
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);//ensure we have data

            var result = DataService.GetCourses(email, ucasCode);//get the course for each org
            Assert.True(result.Courses.Count == 0);
        }

        [Test]
        public void GetCoursesWithInvalidUserAndValidUcasCodeShouldNotReturnNoData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var result = DataService.GetCourses("anyone@anywhere.com", org.UcasCode);//get the course for each org
                Assert.True(result.Courses.Count == 0);
            }
        }

        [Test]
        [TestCase("xxx")]
        [TestCase("   ")]
        [TestCase("")]
        [TestCase(null)]
        public void GetCoursesWithValidUserAndInvalidUcasCodeShouldNotReturnNoData(string ucasCode)
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);

            var result = DataService.GetCourses(TestUserEmail1, ucasCode);//get the course for each org
            Assert.True(result.Courses.Count == 0);
        }

        [Test]
        public void GetCourseShouldReturnData()
        {
            const int numOrgs = 5;
            const int numCourses = 6;
            LoadData(TestUserEmail1, numOrgs, numCourses);
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var coursesList = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the courses for each org
                foreach (var course in coursesList.Courses)
                {
                    var result = DataService.GetCourse(TestUserEmail1, org.UcasCode, course.CourseCode);
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
            var orgList = DataService.GetOrganisationsForUser(TestUserEmail1).ToList();
            Assert.IsTrue(orgList.Count == numOrgs);
            Assert.IsTrue(orgList.All(c => c.TotalCourses == numCourses));

            foreach (var org in orgList)
            {
                var coursesList = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the courses for each org
                foreach (var course in coursesList.Courses)
                {
                    var result = DataService.GetCourse("someone@somewhere.com", org.UcasCode, course.CourseCode);
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
            Context.McUsers.Add(new McUser { FirstName = "fname", LastName = "lname", Email = email });
            LoadSubjects(numSubjects);
            for (var counter = 1; counter <= numOrgs; counter++)
            {
                var orgId = "org" + counter;
                var instCode = "AB" + counter;
                Context.McOrganisations.Add(new McOrganisation { Id = counter, OrgId = orgId, Name = "Organisation " + counter });
                Context.Institutions.Add(new Institution
                {
                    Address1 = "add2",
                    Address2 = "add2",
                    Address3 = "add3",
                    Address4 = "add4",
                    Postcode = "AB1 CD2",
                    InstCode = instCode,
                    InstFull = "Intitution " + counter
                });
                LoadCourses(instCode, numCourses, Context.Subjects);
                Context.McOrganisationUsers.Add(new McOrganisationUser { Email = email, OrgId = orgId });
                Context.McOrganisationIntitutions.Add(new McOrganisationInstitution
                {
                    InstCode = instCode,
                    OrgId = orgId
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
        private void LoadCourses(string instCode, int numRecords, IEnumerable<Subject> subjects)
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
                    InstCode = instCode
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
                    InstCode = instCode,
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

                LoadCourseSubjects(course, instCode, subjects);
            }
        }

        private void LoadSubjects(int numRecords)
        {
            for (var counter = 1; counter <= numRecords; counter++)
            {
                var subjectCode = "SC" + counter;
                Context.Subjects.Add(new Subject
                {
                    SubjectCode = subjectCode,
                    SubjectName = "subject " + counter
                });
            }
        }

        private void LoadCourseSubjects(Course course, string instCode, IEnumerable<Subject> subjects)
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
