using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.DbIntegration;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using GovUk.Education.ManageCourses.Tests.ImporterTests.PayloadBuilders;
using GovUk.Education.ManageCourses.UcasCourseImporter;
using GovUk.Education.ManageCourses.Xls.Domain;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests
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
            DoImport(payload);

            // assert
            var provider = Context.Providers.Single(x => x.ProviderCode == InstCode3);
            provider.CreatedAt.Should().Be(creationTime, InstCode3 + " is a new record");
            provider.UpdatedAt.Should().Be(creationTime);
            foreach (var site in Context.Sites)
            {
                site.CreatedAt.Should().Be(creationTime);
                site.UpdatedAt.Should().Be(creationTime);
            }

            // re-import #1
            MockTime = new DateTime(2019, 2, 2, 3, 4, 5, 7);
            DoImport(payload);


            // re-import #2
            var updateTime = new DateTime(2019, 3, 2, 3, 4, 5, 7);
            MockTime = updateTime;
            DoImport(payload);

            // assert
            Context.Providers.Single(x => x.ProviderCode == InstCode3).CreatedAt.Should().Be(creationTime);
            Context.Providers.Single(x => x.ProviderCode == InstCode3).UpdatedAt.Should().Be(updateTime);
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

            DoImport(import);

            Context.Courses.Should().HaveCount(1, "valid courses should be imported anyway");

            //make an update and change import order
            import.Courses.First().CrseTitle = "The best title";
            import.Courses = import.Courses.Reverse();

            DoImport(import);

            Context.Courses.Should().HaveCount(1, "valid courses should be re-imported anyway");
            Context.Courses.Single().Name.Should().Be("The best title");
            Context.Courses.Single().Provider.Postcode.Should().Be(InstPostCode1);
        }

        private void DoImport(UcasPayload ucasPayload)
        {
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, ucasPayload, MockClock.Object).UpdateUcasData();
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

            var providers = new List<Provider>
            {
                new ProviderBuilder().WithCode(InstCode1),
                new ProviderBuilder().WithCode(InstCode2),
            };

            var organisationProviders = new List<OrganisationProvider>
            {
                new OrganisationProvider {
                    Provider = providers[1],
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
            context.Providers.AddRange(providers);
            context.OrganisationUsers.AddRange(organisationUsers);
            context.OrganisationProviders.AddRange(organisationProviders);
            context.Save();
        }

        private static UcasPayload GetUcasCoursesPayload()
        {
            const string campusCode = "CMP";
            return new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>{
                    new PayloadInstitutionBuilder()
                        .WithInstCode(InstCode1)
                        .WithRegionCode(1)
                        .WithPostcode(InstPostCode1),
                    new PayloadInstitutionBuilder()
                        .WithInstCode(InstCode2),
                    new PayloadInstitutionBuilder()
                        .WithInstCode(InstCode3),
                })
                .WithCourses(new List<UcasCourse>
                {
                    new PayloadCourseBuilder()
                        .WithInstCode(InstCode1)
                        .WithCrseCode("COURSECODE_1")
                        .WithStatus("N")
                        .WithCampusCode(campusCode),
                })
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithInstCode(InstCode1)
                        .WithCampusCode(campusCode)
                        .WithRegionCode(100),
                });
        }
    }
}
