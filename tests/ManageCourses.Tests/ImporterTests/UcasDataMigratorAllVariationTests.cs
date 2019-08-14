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
    [Ignore("Ucas importer is no longer in use.")]
    public class UcasDataMigratorAllVariationTests: DbIntegrationTestBase
    {
        /// <summary>
        /// Turn off retry as per console app configuration
        /// </summary>
        protected override bool EnableRetryOnFailure => false;

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
        }

        private void DoImport(UcasPayload ucasPayload)
        {
            new UcasDataMigrator(Context, new Mock<Serilog.ILogger>().Object, ucasPayload).UpdateUcasData();
        }
    }
}
