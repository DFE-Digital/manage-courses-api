
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.DbIntegration;
using GovUk.Education.ManageCourses.UcasCourseImporter;
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

        private UcasDataMigrator UcasDataMigrator;

        protected override void Setup()
        {
            UcasDataMigrator = new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object);
        }

        [Test]
        public void RepeatImportsArePossible()
        {
            SaveReferenceDataPayload(Context);
            var payload = GetUcasCoursesPayload();

            UcasDataMigrator.UpdateUcasData(payload);
            UcasDataMigrator.UpdateUcasData(payload);
            UcasDataMigrator.UpdateUcasData(payload);

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
            SaveReferenceDataPayload(Context);

            var import = GetUcasCoursesPayload();

            //make dodgy
            import.Courses = import.Courses.Concat(new List<UcasCourse>
            {
                new UcasCourse {InstCode = "DOESNOTEXIST", CrseCode = "FOO"}
            }).ToList();

            UcasDataMigrator.UpdateUcasData(import);

            Assert.AreEqual(1, Context.CourseCodes.Count(), "valid courses should be imported anyway");
            Assert.AreEqual(1, Context.UcasCourses.Count(), "valid courses should be imported anyway");

            //make an update and change import order
            import.Courses.First().CrseTitle = "The best title";
            import.Courses = import.Courses.Reverse();

            UcasDataMigrator.UpdateUcasData(import);

            Assert.AreEqual(1, Context.CourseCodes.Count(), "valid courses should be re-imported anyway");
            Assert.AreEqual(1, Context.UcasCourses.Count(), "valid courses should be re-imported anyway");
            Assert.AreEqual("The best title", Context.UcasCourses.Single().CrseTitle);
        }
        private static void SaveReferenceDataPayload(ManageCoursesDbContext context)
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

            context.McUsers.AddRange(users);
            context.McOrganisations.AddRange(organisations);
            context.UcasInstitutions.AddRange(institutions);
            context.McOrganisationUsers.AddRange(organisationUsers);
            context.McOrganisationIntitutions.AddRange(organisationInstitutions);
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
                        Status = "N"
                    }
                }
            };
        }
    }
}