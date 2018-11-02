
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.DbIntegration;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using GovUk.Education.ManageCourses.Xls.Domain;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.UcasCourseImporter.Tests
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class UcasDataMigratorTests : DbIntegrationTestBase
    {
        private const string InstCode1 = "INSTCODE_1";
        private const string TestUserEmail1 = "email_1@test-manage-courses.gov.uk";
        private const string TestUserEmail2 = "email_2@test-manage-courses.gov.uk";
        private const string TestUserEmail3 = "email_3@test-manage-courses.gov.uk";
        private const string OrgId1 = "OrgId_1";

        [Test]
        public void RepeatImportsArePossible()
        {
            SaveReferenceDataPayload(Context);
            var payload = GetUcasCoursesPayload();

            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload).UpdateUcasData();
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload).UpdateUcasData();
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload).UpdateUcasData();

            foreach (var expected in payload.Courses)
            {
                var count = Context.Courses
                    .Count(o => o.CourseCode == expected.CrseCode);

                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void ErroneousCourseLeavesOtherCoursesAlone()
        {
            SaveReferenceDataPayload(Context);

            var import = GetUcasCoursesPayload();

            //make dodgy
            import.Courses = import.Courses.Concat(new List<UcasCourse>
            {
                new UcasCourse {InstCode = "DOESNOTEXIST", CrseCode = "FOO"}
            }).ToList();

            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, import).UpdateUcasData();
            
            Assert.AreEqual(1, Context.Courses.Count(), "valid courses should be imported anyway");

            //make an update and change import order
            import.Courses.First().CrseTitle = "The best title";
            import.Courses = import.Courses.Reverse();

            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, import).UpdateUcasData();
            
            Assert.AreEqual(1, Context.Courses.Count(), "valid courses should be re-imported anyway");
            Assert.AreEqual("The best title", Context.Courses.Single().Name);
        }
        private static void SaveReferenceDataPayload(ManageCoursesDbContext context)
        {
            var users = new List<User>
            {
                new User
                {
                    FirstName = "FirstName_1",
                    LastName = "LastName_1",
                    Email = TestUserEmail1
                },
                new User
                {
                    FirstName = "FirstName_2",
                    LastName = "LastName_2",
                    Email = TestUserEmail2
                },
                new User
                {
                    FirstName = "FirstName_3",
                    LastName = "LastName_3",
                    Email = TestUserEmail3
                }
            };
            const string orgId2 = "OrgId_2";
            var organisations = new List<Organisation> {
                new Organisation {
                    OrgId = OrgId1
                },
                new Organisation {
                    OrgId = orgId2
                }

            };

            const string instCode2 = "InstCode_2";
            var institutions = new List<Provider>
            {
                new Provider {
                    ProviderCode = InstCode1
                },
                new Provider {
                    ProviderCode = instCode2
                }
            };

            var organisationInstitutions = new List<OrganisationProvider>
            {
                new OrganisationProvider {
                    Provider = institutions[1],
                    Organisation = organisations[1],
                }
            };
            var organisationUsers = new List<OrganisationUser>
            {
                new OrganisationUser {
                    User = users[1],
                },
                new OrganisationUser {
                    User = users[2],
                    Organisation = organisations[0]
                }
            };

            context.Users.AddRange(users);
            context.Organisations.AddRange(organisations);
            context.Institutions.AddRange(institutions);
            context.OrganisationUsers.AddRange(organisationUsers);
            context.OrganisationIntitutions.AddRange(organisationInstitutions);
            context.Save();
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
                        Status = "N",
                        CampusCode = "CMP"
                    }
                },
                Campuses = new List<UcasCampus>
                {
                    new UcasCampus
                    {
                        CampusCode = "CMP",
                        InstCode = InstCode1
                    }   
                }
            };
        }
    }
}