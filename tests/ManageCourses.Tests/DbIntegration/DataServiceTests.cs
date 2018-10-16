using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
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
        private const string InstCode1 = "INSTCODE_1";

        private const string TestUserEmail1 = "email_1@test-manage-courses.gov.uk";
        private const string TestUserEmail2 = "email_2@test-manage-courses.gov.uk";
        private const string TestUserEmail3 = "email_3@test-manage-courses.gov.uk";
        private const string OrgId1 = "OrgId_1";

        protected override void Setup()
        {
            var mockLogger = new Mock<ILogger<DataService>>();
            var mockEnrichmentService = new Mock<IEnrichmentService>();
            mockEnrichmentService.Setup(x => x.GetCourseEnrichmentMetadata(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<UcasCourseEnrichmentGetModel>());

            var mockPdgeWhitelist = new Mock<IPgdeWhitelist>();
            mockPdgeWhitelist.Setup(x => x.ForInstitution(It.IsAny<string>())).Returns(new List<PgdeCourse>());
            DataService = new DataService(Context, mockEnrichmentService.Object, new UserDataHelper(), mockLogger.Object, mockPdgeWhitelist.Object);
        }

        [Test]
        public void ProcessPayload()
        {
            var referenceDataPayload = GetReferenceDataPayload();
            DataService.ProcessReferencePayload(referenceDataPayload);

            var payload = GetUcasCoursesPayload();
            DataService.ProcessUcasPayload(payload);

            foreach (var item in Context.McUsers)
            {
                var payloadUser = referenceDataPayload.Users.FirstOrDefault(
                    x => x.FirstName == item.FirstName &&
                    x.LastName == item.LastName &&
                    x.Email == item.Email);

                Assert.IsNotNull(payloadUser);

                var payloadOrgUser = referenceDataPayload.OrganisationUsers.FirstOrDefault(x => x.Email == payloadUser.Email);

                if (payloadOrgUser != null)
                {
                    if (!string.IsNullOrEmpty(payloadOrgUser.OrgId))
                    {
                        Assert.AreEqual(payloadUser.Email, TestUserEmail3);
                        Assert.AreEqual(item.McOrganisationUsers.First().McOrganisation.OrgId, payloadOrgUser.OrgId);
                    }
                    else
                    {
                        Assert.AreEqual(payloadUser.Email, TestUserEmail2);
                    }
                }
                else
                {
                    Assert.AreEqual(payloadUser.Email, TestUserEmail1);
                }
            }

            GetCoursesForUser_isNull(TestUserEmail1, null);
            GetCoursesForUser_isNull(TestUserEmail2, null);
            GetCoursesForUser_isNull(TestUserEmail3, OrgId1);
        }

        [Test]
        public void RepeatImportsArePossible()
        {
            var payload = GetUcasCoursesPayload();
            var userPayload = GetReferenceDataPayload();

            DataService.ProcessReferencePayload(userPayload);
            DataService.ProcessUcasPayload(payload);

            DataService.ProcessReferencePayload(userPayload);
            DataService.ProcessUcasPayload(payload);

            DataService.ProcessReferencePayload(userPayload);
            DataService.ProcessReferencePayload(userPayload);

            DataService.ProcessUcasPayload(payload);
            DataService.ProcessUcasPayload(payload);

            foreach (var expected in userPayload.Users)
            {
                var storedUser = Context.GetMcUsers(expected.Email);

                Assert.AreEqual(1, storedUser.Count());
                Assert.AreEqual(expected.FirstName, storedUser.Single().FirstName);
                Assert.AreEqual(expected.LastName, storedUser.Single().LastName);
            }

            foreach (var expected in userPayload.Organisations)
            {
                var storedOrg = Context.McOrganisations.Where(o => o.OrgId == expected.OrgId);

                Assert.AreEqual(1, storedOrg.Count());
            }

            foreach (var expected in userPayload.Institutions)
            {
                var storedOrg = Context.UcasInstitutions.Where(o => o.InstCode == expected.InstCode);

                Assert.AreEqual(1, storedOrg.Count());
            }

            foreach (var expected in userPayload.OrganisationUsers)
            {
                var count = Context.McOrganisationUsers
                    .Count(o => o.OrgId == expected.OrgId && o.Email == expected.Email);

                Assert.AreEqual(1, count);
            }

            foreach (var expected in userPayload.OrganisationInstitutions)
            {
                var count = Context.McOrganisationIntitutions
                    .Count(o => o.OrgId == expected.OrgId && o.InstitutionCode == expected.InstitutionCode);

                Assert.AreEqual(1, count);
            }

            foreach (var expected in payload.Courses)
            {
                var count = Context.UcasCourses
                    .Count(o => o.CrseCode == expected.CrseCode);

                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void ErroneousCourseLeavesOtherCoursesAlone()
        {
            DataService.ProcessReferencePayload(GetReferenceDataPayload());

            var import = GetUcasCoursesPayload();

            //make dodgy
            import.Courses = import.Courses.Concat(new List<UcasCourse>
            {
                new UcasCourse {InstCode = "DOESNOTEXIST", CrseCode = "FOO"}
            }).ToList();

            DataService.ProcessUcasPayload(import);

            Assert.AreEqual(1, Context.CourseCodes.Count(), "valid courses should be imported anyway");
            Assert.AreEqual(1, Context.UcasCourses.Count(), "valid courses should be imported anyway");

            //make an update and change import order
            import.Courses.First().CrseTitle = "The best title";
            import.Courses = import.Courses.Reverse();

            DataService.ProcessUcasPayload(import);

            Assert.AreEqual(1, Context.CourseCodes.Count(), "valid courses should be re-imported anyway");
            Assert.AreEqual(1, Context.UcasCourses.Count(), "valid courses should be re-imported anyway");
            Assert.AreEqual("The best title", Context.UcasCourses.Single().CrseTitle);
        }

        [TestCase("nothing@nowhere.com", null)]
        public void GetCoursesForUser_isNull(string email, string orgId)
        {
            var result = DataService.GetCoursesForUserOrganisation(email, orgId);

            Assert.IsNull(result.OrganisationId);
            Assert.IsNull(result.OrganisationName);

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

            foreach (var org in orgList)//we have a valist list of data
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

            foreach (var org in orgList)//we have a valist list of data
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

            foreach (var org in orgList)//we have a valist list of data
            {
                var result = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the course for each org

                foreach (var course in result.Courses)
                {
                    course.Schools.All(s => s.Status == "N").Should().BeTrue();
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

            foreach (var org in orgList)//we have a valist list of data
            {
                var result = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the course for each org

                foreach (var course in result.Courses)
                {
                    course.Schools.All(s => s.Publish == "N").Should().BeTrue();
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

            foreach (var org in orgList)//we have a valist list of data
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

            foreach (var org in orgList)//we have a valist list of data
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

            foreach (var org in orgList)//we have a valist list of data
            {
                var coursesList = DataService.GetCourses(TestUserEmail1, org.UcasCode);//get the courses for each org
                foreach (var course in coursesList.Courses)
                {
                    var result = DataService.GetCourse("someone@somewhere.com", org.UcasCode, course.CourseCode);
                    result.Should().BeNull();
                }
            }
        }

        private static ReferenceDataPayload GetReferenceDataPayload()
        {
            var users = new List<McUser>
            {
                new McUser
                {
                    FirstName = "FirstName_1",
                    LastName = "LastName_1",
                    Email = TestUserEmail1
                },
                new McUser
                {
                    FirstName = "FirstName_2",
                    LastName = "LastName_2",
                    Email = TestUserEmail2
                },
                new McUser
                {
                    FirstName = "FirstName_3",
                    LastName = "LastName_3",
                    Email = TestUserEmail3
                }
            };
            const string orgId2 = "OrgId_2";
            var organisations = new List<McOrganisation> {
                new McOrganisation {
                    OrgId = OrgId1
                },
                new McOrganisation {
                    OrgId = orgId2
                }

            };

            const string instCode2 = "InstCode_2";
            var institutions = new List<UcasInstitution>
            {
                new UcasInstitution {
                    InstCode = InstCode1
                },
                new UcasInstitution {
                    InstCode = instCode2
                }
            };

            var organisationInstitutions = new List<McOrganisationInstitution>
            {
                new McOrganisationInstitution {
                    InstitutionCode = instCode2,
                    OrgId = orgId2,
                }
            };
            var organisationUsers = new List<McOrganisationUser>
            {
                new McOrganisationUser {
                    Email = TestUserEmail2,
                },
                new McOrganisationUser {
                    Email = TestUserEmail3,
                    OrgId = OrgId1
                }
            };
            var result = new ReferenceDataPayload
            {
                Users = users,
                OrganisationInstitutions = organisationInstitutions,
                OrganisationUsers = organisationUsers,
                Organisations = organisations,
                Institutions = institutions
            };

            return result;
        }

        private static UcasPayload GetUcasCoursesPayload()
        {
            return new UcasPayload
            {
                Institutions = new List<UcasInstitution>
                {
                    new UcasInstitution { InstCode = InstCode1 }
                },

                Courses = new List<UcasCourse>{
                    new UcasCourse
                    {
                        InstCode = InstCode1,
                        CrseCode = "COURSECODE_1",
                        Status = "N"
                    }
                }
            };
        }

        /// <summary>
        /// setup data so we can test
        /// </summary>
        /// <param name="email">email of the selected user</param>
        /// <param name="numOrgs">number og oganisation records to generate</param>
        /// <param name="numCourses">number of course records to generate</param>
        private void LoadData(string email, int numOrgs, int numCourses)
        {
            int numSubjects = 3;
            Context.McUsers.Add(new McUser { FirstName = "fname", LastName = "lname", Email = email });
            LoadSubjects(numSubjects);
            for (var counter = 1; counter <= numOrgs; counter++)
            {
                var orgId = "org" + counter;
                var instCode = "AB" + counter;
                Context.McOrganisations.Add(new McOrganisation { Id = counter, OrgId = orgId, Name = "Organisation " + counter });
                Context.UcasInstitutions.Add(new UcasInstitution
                {
                    Addr1 = "add2",
                    Addr2 = "add2",
                    Addr3 = "add3",
                    Addr4 = "add4",
                    Postcode = "AB1 CD2",
                    InstCode = instCode,
                    InstFull = "Intitution " + counter
                });
                LoadCourses(instCode, numCourses, numSubjects);
                Context.McOrganisationUsers.Add(new McOrganisationUser { Email = email, OrgId = orgId });
                Context.McOrganisationIntitutions.Add(new McOrganisationInstitution
                {
                    InstitutionCode = instCode,
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
        private void LoadCourses(string instCode, int numRecords, int numSubjects)
        {
            for (var counter = 1; counter <= numRecords; counter++)
            {
                var courseCode = "CC" + counter;
                var campusCode = "C" + counter;
                Context.UcasCourses.Add(new UcasCourse
                {
                    Age = "P",
                    CrseCode = courseCode,
                    CrseOpenDate = "2018-100-16 00:00;00",
                    ProfpostFlag = "PG",
                    ProgramType = "SC",
                    Studymode = "F",
                    CrseTitle = "Title " + counter,
                    InstCode = instCode,
                    CampusCode = campusCode,
                    Status = "N",
                    Publish = "N",
                });
                Context.UcasCampuses.Add(new UcasCampus
                {
                    Addr1 = "add1",
                    Addr2 = "add2",
                    Addr3 = "add3",
                    Addr4 = "add4",
                    Postcode = "PC1 A23",
                    CampusCode = campusCode,
                    CampusName = "Campus " + counter,
                    InstCode = instCode
                });
                Context.CourseCodes.Add(new CourseCode { CrseCode = courseCode, InstCode = instCode });
                LoadCourseSubjects(courseCode, instCode, numSubjects);
            }
        }

        private void LoadSubjects(int numRecords)
        {
            for (var counter = 1; counter <= numRecords; counter++)
            {
                var subjectCode = "SC" + counter;
                Context.UcasSubjects.Add(new UcasSubject
                {
                    SubjectCode = subjectCode,
                    SubjectDescription = "subject " + counter
                });
            }
        }

        private void LoadCourseSubjects(string courseCode, string instCode, int numRecords)
        {
            for (var counter = 1; counter <= numRecords; counter++)
            {
                var subjectCode = "SC" + counter;
                Context.UcasCourseSubjects.Add(new UcasCourseSubject
                {
                    CrseCode = courseCode,
                    InstCode = instCode,
                    SubjectCode = subjectCode
                });
            }
        }
    }
}
