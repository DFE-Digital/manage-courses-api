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
    [Ignore("Ucas importer is no longer in use.")]
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
                .AccreditingProviderCode.Should().Be(accreditingProviderCode);

            // import as modified
            ucasCourse.AccreditingProvider = accreditingProviderCode2;
            DoImport(ucasPayload);
            var updatedProvider = Context.Providers.Single(p => p.ProviderCode == instCode);
            updatedProvider.Courses.Count.Should().Be(1);
            updatedProvider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProviderCode.Should().Be(accreditingProviderCode2);
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
                .AccreditingProviderCode.Should().Be(accreditingProviderCode);

            // import as modified
            ucasCourse.AccreditingProvider = accreditingProviderCode2;
            DoImport(ucasPayload);
            var updatedProvider = Context.Providers.Single(p => p.ProviderCode == instCode);
            updatedProvider.Courses.Count.Should().Be(1);
            updatedProvider.Courses.Single(c => c.CourseCode == courseCode)
                .AccreditingProviderCode.Should().Be(accreditingProviderCode2);
        }

        private void DoImport(UcasPayload ucasPayload)
        {
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, ucasPayload).UpdateUcasData();
        }
    }
}
