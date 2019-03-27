using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
    public class UcasDataMigratorOptInTests : DbIntegrationTestBase
    {
        /// <summary>
        /// Turn off retry as per console app configuration
        /// </summary>
        protected override bool EnableRetryOnFailure => false;

        [Test]
        public void ProviderOptedOut()
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
        public void CourseOptedOut_NoAccreditingProvider()
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
        public void CourseOptedIn_NoAccreditingProvider()
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
        public void CourseOptedOut_AccreditingProviderOptedOut()
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
        public void CourseOptedIn_AccreditingProviderOptedOut()
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
        public void CourseOptedOut_AccreditingProviderOptedIn()
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

        private void DoImport(UcasPayload ucasPayload)
        {
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, ucasPayload).UpdateUcasData();
        }
    }
}
