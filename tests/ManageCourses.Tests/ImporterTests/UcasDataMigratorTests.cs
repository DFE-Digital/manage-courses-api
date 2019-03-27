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
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload, MockClock.Object).UpdateUcasData();

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
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload, MockClock.Object).UpdateUcasData();


            // re-import #2
            var updateTime = new DateTime(2019, 3, 2, 3, 4, 5, 7);
            MockTime = updateTime;
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, payload, MockClock.Object).UpdateUcasData();

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

        [Test]
        public void Provider()
        {
            const string instCode = "AA1";
            const string instName = "Armadillo 1";
            UcasInstitution institution = new PayloadInstitutionBuilder()
                .WithInstCode(instCode)
                .WithFullName(instName);
            var ucasPayload = new PayloadBuilder().WithInstitutions(new List<UcasInstitution> { institution });

            // import as new
            DoImport(ucasPayload);
            Context.Providers.Single(p => p.ProviderCode == instCode).ProviderName
                .Should().Be(instName);

            // import modification
            const string modifiedInstName = "Modified " + instName;
            institution.InstFull = modifiedInstName;
            DoImport(ucasPayload);
            Context.Providers.Single(p => p.ProviderCode == instCode).ProviderName
                .Should().Be(modifiedInstName);
        }

        [Test]
        public void ProviderOptedIn()
        {
            // note: we can't test an import of a new provider that's opted in because
            // they have to be in our database already to be opted in

            // arrange
            const string instCode = "AA1";
            const string unmodifiedInstName = "Armadillo 1";
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(instCode)
                .WithName(unmodifiedInstName)
                .WithOptedIn());
            Context.Save();

            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                        .WithFullName("Modified " + unmodifiedInstName)
                });

            // act
            DoImport(ucasPayload);

            // assert
            Context.Providers.Single(p => p.ProviderCode == instCode).ProviderName
                .Should().Be(unmodifiedInstName);
        }

        [Test]
        public void Course()
        {
            const string instCode = "INST101";
            const string campusCode = "CAMP101";
            const string courseCode = "CRS101";
            const string courseName = "Course 101";

            // build payload
            UcasCourse ucasCourse = new PayloadCourseBuilder()
                .WithInstCode(instCode)
                .WithCrseCode(courseCode)
                .WithCampusCode(campusCode)
                .WithName(courseName);
            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                })
                .WithCourses(new List<UcasCourse> {ucasCourse})
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // import as new
            DoImport(ucasPayload);
            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .Name.Should().Be(courseName);

            // import as modified
            const string modifiedCourseName = "Modified " + courseName;
            ucasCourse.CrseTitle = modifiedCourseName;
            DoImport(ucasPayload);
            provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .Name.Should().Be(modifiedCourseName);
        }

        [Test]
        public void CourseOptedIn()
        {
            // note: we can't test an import of a new provider's course where they are opted in because
            // they have to be in our database already to be opted in

            const string instCode = "AA1";
            const string courseCode = "CRS101";
            const string unmodifiedCourseName = "Primary";
            const string campusCode = "CAMP101";

            // arrange
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(instCode)
                .WithOptedIn()
                .WithCourses(new List<Course>
                {
                    new CourseBuilder()
                        .WithCode(courseCode)
                        .WithName(unmodifiedCourseName)
                }));
            Context.Save();

            UcasCourse ucasCourse = new PayloadCourseBuilder()
                .WithInstCode(instCode)
                .WithCrseCode(courseCode)
                .WithCampusCode(campusCode)
                .WithName("Modified " + unmodifiedCourseName);
            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                })
                .WithCourses(new List<UcasCourse> {ucasCourse})
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // act
            DoImport(ucasPayload);

            // assert
            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .Name.Should().Be(unmodifiedCourseName);
        }


        [Test]
        public void Course_AccreditingProvider()
        {
            const string instCode = "INST101";
            const string accreditingProviderCode = "AINST201";
            const string accreditingProviderCode2 = "AINST202";
            const string campusCode = "CAMP101";
            const string courseCode = "CRS101";

            Context.Providers.Add(new ProviderBuilder()
                .WithCode(accreditingProviderCode));
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(accreditingProviderCode2));
            Context.Save();

            // payload
            UcasCourse ucasCourse = new PayloadCourseBuilder()
                .WithInstCode(instCode)
                .WithCampusCode(campusCode)
                .WithCrseCode(courseCode)
                .WithAccreditingProvider(accreditingProviderCode);
            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                })
                .WithCourses(new List<UcasCourse> {ucasCourse})
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // import as new
            DoImport(ucasPayload);
            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode);

            // import as modified
            ucasCourse.AccreditingProvider = accreditingProviderCode2;
            DoImport(ucasPayload);
            var updatedProvider = Context.Providers.Single(p => p.ProviderCode == instCode);
            updatedProvider.Courses.Count.Should().Be(1);
            updatedProvider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode2);
        }

        [Test]
        public void CourseOptedIn_AccreditingProvider()
        {
            // note: we can't test an import of a new provider's course where they are opted in because
            // they have to be in our database already to be opted in

            const string instCode = "AA1";
            const string accreditingProviderCode = "AINST201";
            const string accreditingProviderCode2 = "AINST202";
            const string courseCode = "CRS101";
            const string unmodifiedCourseName = "Primary";
            const string campusCode = "CAMP101";

            // arrange
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(instCode)
                .WithOptedIn()
                .WithCourses(new List<Course>
                {
                    new CourseBuilder()
                        .WithCode(courseCode)
                        .WithName(unmodifiedCourseName)
                        .WithAccreditingProvider(new ProviderBuilder()
                            .WithCode(accreditingProviderCode))
                }));
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(accreditingProviderCode2));
            Context.Save();

            UcasCourse ucasCourse = new PayloadCourseBuilder()
                .WithInstCode(instCode)
                .WithCrseCode(courseCode)
                .WithAccreditingProvider(accreditingProviderCode2)
                .WithCampusCode(campusCode);
            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                })
                .WithCourses(new List<UcasCourse> {ucasCourse})
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // act
            DoImport(ucasPayload);

            // assert
            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode);
        }

        [Test]
        public void Course_AccreditingProviderOptedIn()
        {
            const string instCode = "INST101";
            const string accreditingProviderCode = "AINST201";
            const string accreditingProviderCode2 = "AINST202";
            const string campusCode = "CAMP101";
            const string courseCode = "CRS101";

            Context.Providers.Add(new ProviderBuilder()
                .WithOptedIn()
                .WithCode(accreditingProviderCode));
            Context.Providers.Add(new ProviderBuilder()
                .WithOptedIn()
                .WithCode(accreditingProviderCode2));
            Context.Save();

            // payload
            UcasCourse ucasCourse = new PayloadCourseBuilder()
                .WithInstCode(instCode)
                .WithCampusCode(campusCode)
                .WithCrseCode(courseCode)
                .WithAccreditingProvider(accreditingProviderCode);
            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                })
                .WithCourses(new List<UcasCourse> {ucasCourse})
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // import as new
            DoImport(ucasPayload);
            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode);

            // import as modified
            ucasCourse.AccreditingProvider = accreditingProviderCode2;
            DoImport(ucasPayload);
            var updatedProvider = Context.Providers.Single(p => p.ProviderCode == instCode);
            updatedProvider.Courses.Count.Should().Be(1);
            updatedProvider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode2);
        }

        [Test]
        public void CourseOptedIn_AccreditingProviderOptedIn()
        {
            // note: we can't test an import of a new provider's course where they are opted in because
            // they have to be in our database already to be opted in

            const string instCode = "AA1";
            const string accreditingProviderCode = "AINST201";
            const string accreditingProviderCode2 = "AINST202";
            const string courseCode = "CRS101";
            const string unmodifiedCourseName = "Primary";
            const string campusCode = "CAMP101";

            // arrange
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(instCode)
                .WithOptedIn()
                .WithCourses(new List<Course>
                {
                    new CourseBuilder()
                        .WithCode(courseCode)
                        .WithName(unmodifiedCourseName)
                        .WithAccreditingProvider(new ProviderBuilder()
                            .WithOptedIn()
                            .WithCode(accreditingProviderCode))
                }));
            Context.Providers.Add(new ProviderBuilder()
                .WithOptedIn()
                .WithCode(accreditingProviderCode2));
            Context.Save();

            UcasCourse ucasCourse = new PayloadCourseBuilder()
                .WithInstCode(instCode)
                .WithCrseCode(courseCode)
                .WithAccreditingProvider(accreditingProviderCode2)
                .WithCampusCode(campusCode);
            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithInstCode(instCode)
                })
                .WithCourses(new List<UcasCourse> {ucasCourse})
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // act
            DoImport(ucasPayload);

            // assert
            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.Courses.Count.Should().Be(1);
            provider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode);
        }

        [Test]
        public void AllVariations()
        {
            // build a big payload with all of the above
            const string instCode = "AA1";
            const string instCodeOptedIn = "AA2";
            const string instName = "Armadillo 1";
            const string modifiedInstName = "Modified " + instName;
            const string instNameOptedIn = "OptedIn Armadillo 2";
            const string accreditingProviderCode = "AINST201";
            const string accreditingProviderCodeOptedIn = "AINST202";
            const string accreditingProviderCode3 = "AINST203";
            const string accreditingProviderCode4 = "AINST204";
            const string courseCode = "CRS101";
            const string courseCode2 = "CRS102";
            const string courseCode3 = "CRS103";
            const string unmodifiedCourseName = "Primary";
            const string modifiedCourseName = "Modified " + unmodifiedCourseName;
            const string campusCode = "CAMP101";

            // arrange
            // accrediting providers
            var accreditingProvider = new ProviderBuilder()
                .WithCode(accreditingProviderCode);
            Context.Providers.Add(accreditingProvider);
            var accreditingProviderOptedIn = new ProviderBuilder()
                .WithOptedIn()
                .WithCode(accreditingProviderCodeOptedIn);
            Context.Providers.Add(accreditingProviderOptedIn);
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(accreditingProviderCode3));
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(accreditingProviderCode4));
            // non-opted in provider and its courses
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(instCode)
                .WithName(instName)
                .WithCourses(new List<Course>
                {
                    new CourseBuilder()
                        .WithCode(courseCode)
                        .WithName(unmodifiedCourseName),
                    new CourseBuilder()
                        .WithCode(courseCode2)
                        .WithAccreditingProvider(accreditingProvider),
                    new CourseBuilder()
                        .WithCode(courseCode3)
                        .WithAccreditingProvider(accreditingProviderOptedIn),
                }));
            // opted in provider and its courses
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(instCodeOptedIn)
                .WithName(instNameOptedIn)
                .WithOptedIn()
                .WithCourses(new List<Course>
                {
                    new CourseBuilder()
                        .WithCode(courseCode)
                        .WithName(unmodifiedCourseName),
                    new CourseBuilder()
                        .WithCode(courseCode2)
                        .WithAccreditingProvider(accreditingProvider),
                    new CourseBuilder()
                        .WithCode(courseCode3)
                        .WithAccreditingProvider(accreditingProviderOptedIn),
                }));
            Context.Save();

            var ucasPayload = new PayloadBuilder()
                .WithInstitutions(new List<UcasInstitution>
                {
                    new PayloadInstitutionBuilder()
                        .WithFullName(modifiedInstName)
                        .WithInstCode(instCode),
                    new PayloadInstitutionBuilder()
                        .WithFullName("Modified " + instNameOptedIn)
                        .WithInstCode(instCodeOptedIn),
                })
                .WithCourses(new List<UcasCourse> {
                    new PayloadCourseBuilder()
                        .WithInstCode(instCode)
                        .WithCrseCode(courseCode)
                        .WithName(modifiedCourseName)
                        .WithCampusCode(campusCode),
                    new PayloadCourseBuilder()
                        .WithInstCode(instCode)
                        .WithCrseCode(courseCode2)
                        .WithAccreditingProvider(accreditingProviderCode3)
                        .WithCampusCode(campusCode),
                    new PayloadCourseBuilder()
                        .WithInstCode(instCode)
                        .WithCrseCode(courseCode3)
                        .WithAccreditingProvider(accreditingProviderCode4)
                        .WithCampusCode(campusCode),
                    new PayloadCourseBuilder()
                        .WithInstCode(instCodeOptedIn)
                        .WithCrseCode(courseCode)
                        .WithName(modifiedCourseName)
                        .WithCampusCode(campusCode),
                    new PayloadCourseBuilder()
                        .WithInstCode(instCodeOptedIn)
                        .WithCrseCode(courseCode2)
                        .WithAccreditingProvider(accreditingProviderCode3)
                        .WithCampusCode(campusCode),
                    new PayloadCourseBuilder()
                        .WithInstCode(instCodeOptedIn)
                        .WithCrseCode(courseCode3)
                        .WithAccreditingProvider(accreditingProviderCode4)
                        .WithCampusCode(campusCode),
                })
                .WithCampuses(new List<UcasCampus>
                {
                    new PayloadCampusBuilder()
                        .WithCampusCode(campusCode)
                        .WithInstCode(instCode)
                });

            // act
            DoImport(ucasPayload);

            // assert

            var provider = Context.Providers.Single(p => p.ProviderCode == instCode);
            provider.ProviderName.Should().Be(modifiedInstName);
            provider.Courses.Count(c => c.Provider.ProviderCode == instCode).Should().Be(3);
            provider.Courses.Single(c => c.Provider.ProviderCode == instCode && c.CourseCode == courseCode)
                .Name.Should().Be(modifiedCourseName);
            provider.Courses.Single(c => c.Provider.ProviderCode == instCode && c.CourseCode == courseCode2)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode3);
            provider.Courses.Single(c => c.Provider.ProviderCode == instCode && c.CourseCode == courseCode3)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode4);

            var providerOptedIn = Context.Providers.Single(p => p.ProviderCode == instCodeOptedIn);
            providerOptedIn.ProviderName.Should().Be(instNameOptedIn);
            providerOptedIn.Courses.Count(c => c.Provider.ProviderCode == instCodeOptedIn).Should().Be(3);
            providerOptedIn.Courses.Single(c => c.Provider.ProviderCode == instCodeOptedIn && c.CourseCode == courseCode)
                .Name.Should().Be(unmodifiedCourseName);
            providerOptedIn.Courses.Single(c => c.Provider.ProviderCode == instCodeOptedIn && c.CourseCode == courseCode2)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCode);
            providerOptedIn.Courses.Single(c => c.Provider.ProviderCode == instCodeOptedIn && c.CourseCode == courseCode3)
                .AccreditingProvider.ProviderCode.Should().Be(accreditingProviderCodeOptedIn);
        }

        private void DoImport(UcasPayload ucasPayload)
        {
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, ucasPayload).UpdateUcasData();
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
