
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FluentAssertions;
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
        private const string InstPostCode1 = "AB12CD";
        private const string InstCode1 = "INSTCODE_1";
        private const string InstCode2 = "INSTCODE_2";
        private const string InstCode3 = "INSTCODE_3";

        private const string TestUserEmail1 = "email_1@test-manage-courses.gov.uk";
        private const string TestUserEmail2 = "email_2@test-manage-courses.gov.uk";
        private const string TestUserEmail3 = "email_3@test-manage-courses.gov.uk";
        private const string OrgId1 = "OrgId_1";

        /// <summary>
        /// Turn off retry as per console app configuration
        /// </summary>
        protected override bool EnableRetryOnFailure => false;

        [Test]
        public void RepeatImportsArePossible()
        {
            SaveReferenceDataPayload(Context);
            var payload = GetUcasCoursesPayload();

            var creationTime = new DateTime(2019, 1, 2, 3, 4, 5, 7);
            MockTime = creationTime;

            // first import
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload, MockClock.Object).UpdateUcasData();

            // assert
            var provider = Context.Providers.Single(x => x.ProviderCode == InstCode3);
            provider.CreatedAt.Should().Be(creationTime, InstCode3 + " is a new record");
            provider.UpdatedAt.Should().Be(creationTime);
            provider.ChangedAt.Should().Be(creationTime);
            foreach (var site in Context.Sites)
            {
                site.CreatedAt.Should().Be(creationTime);
                site.UpdatedAt.Should().Be(creationTime);
            }

            // re-import #1
            MockTime = new DateTime(2019, 2, 2, 3, 4, 5, 7);
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload, MockClock.Object).UpdateUcasData();


            // re-import #2
            var updateTime = new DateTime(2019, 3, 2, 3, 4, 5, 7);
            MockTime = updateTime;
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload, MockClock.Object).UpdateUcasData();

            // assert
            var updatedProvider = Context.Providers.Single(x => x.ProviderCode == InstCode3);
            updatedProvider.CreatedAt.Should().Be(creationTime);
            updatedProvider.UpdatedAt.Should().Be(updateTime);
            updatedProvider.ChangedAt.Should().Be(updateTime);
            foreach (var site in Context.Sites)
            {
                // not testing creation time because sites are dropped and created every time
                site.UpdatedAt.Should().Be(updateTime);
            }

            foreach (var expected in payload.Courses)
            {
                Context.Courses.Count(o => o.CourseCode == expected.CrseCode).Should().Be(1, $"course code '{expected.CrseCode}' was in the payload");
            }
            foreach (var expected in payload.Campuses)
            {
                Context.Sites.Count(o => o.RegionCode == expected.RegionCode).Should().Be(1, $"site region code '{expected.RegionCode}' was in the payload");
            }

            Context.Providers.Single(x => x.ProviderCode == InstCode1).RegionCode.Should().Be(1);
            Context.Providers.Single(x => x.ProviderCode == InstCode2).RegionCode.Should().BeNull();
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

            Context.Courses.Should().HaveCount(1, "valid courses should be imported anyway");

            //make an update and change import order
            import.Courses.First().CrseTitle = "The best title";
            import.Courses = import.Courses.Reverse();

            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, import).UpdateUcasData();

            Context.Courses.Should().HaveCount(1, "valid courses should be re-imported anyway");
            Context.Courses.Single().Name.Should().Be("The best title");
            Context.Courses.Single().Provider.Postcode.Should().Be(InstPostCode1);
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

            var institutions = new List<Provider>
            {
                new Provider {
                    ProviderCode = InstCode1,
                },
                new Provider {
                    ProviderCode = InstCode2
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
            context.Providers.AddRange(institutions);
            context.OrganisationUsers.AddRange(organisationUsers);
            context.OrganisationProviders.AddRange(organisationInstitutions);
            context.Save();
        }

        private static UcasPayload GetUcasCoursesPayload()
        {
            return new UcasPayload
            {
                Institutions = new List<UcasInstitution>
                {
                    new UcasInstitution { InstCode = InstCode1, RegionCode = 1, Postcode = InstPostCode1},
                    new UcasInstitution { InstCode = InstCode2 },
                    new UcasInstitution { InstCode = InstCode3 },
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
                        InstCode = InstCode1,
                        RegionCode = 100
                    }
                }
            };
        }
    }
}
