using System;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.CourseExporterUtil;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration.CourseExporterUtilTests
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    public class CourseExporterUtilTests : DbIntegrationTestBase
    {
        private const string ProviderCode = "A19";
        private const string AccreditingProviderCode = "ACC19";
        private const string CourseName2019 = "Poetry 2019";
        private const string TrainWithUs2019 = "Train 2019";
        private const string AboutAccreditingProvider2019 = "About 2019";
        private const string TrainWithDisability2019 = "Disability 2019";

        protected override void Setup()
        {
            Course course2019 = CourseBuilder
                .Build(CourseType.RunningPublished,
                    new ProviderBuilder()
                        .WithCycle("2019")
                        .WithCode(ProviderCode))
                .WithName(CourseName2019)
                .WithAccreditingProvider(
                    new ProviderBuilder()
                        .WithCycle("2019")
                        .WithCode(AccreditingProviderCode)
                );
            Course course2020 = CourseBuilder
                .Build(CourseType.RunningPublished,
                    new ProviderBuilder()
                        .WithCycle("2020")
                        .WithCode(ProviderCode))
                .WithName("Operatics 2020")
                .WithAccreditingProvider(
                    new ProviderBuilder()
                        .WithCycle("2020")
                        .WithCode(AccreditingProviderCode)
                );

            Context.Courses.Add(course2019);
            Context.Courses.Add(course2020);
            var updatedAt = new DateTime(2001,02,03,04,05,06);
            var jsonData2019 = @"
            {
                ""Email"": ""x@example.org"",
                ""Website"": ""http://example.org"",
                ""Address1"": ""add1"",
                ""Address2"": ""add2"",
                ""Address3"": ""add3"",
                ""Address4"": ""add4"",
                ""Postcode"": ""SW1A 1AA"",
                ""Telephone"": ""0123321"",
                ""RegionCode"": 5,
                ""TrainWithUs"": """+ TrainWithUs2019 + @""",
                ""TrainWithDisability"": """+ TrainWithDisability2019 + @""",
                ""AccreditingProviderEnrichments"": [
                {
                    ""Description"": """ + AboutAccreditingProvider2019 + @""",
                    ""UcasProviderCode"": """ + AccreditingProviderCode + @"""
                }
                ]
            }";

            var jsonData2020 = @"
            {
                ""Email"": ""2020"",
                ""Website"": ""2020"",
                ""Address1"": ""2020"",
                ""Address2"": ""2020"",
                ""Address3"": ""2020"",
                ""Address4"": ""2020"",
                ""Postcode"": ""2020"",
                ""Telephone"": ""2020"",
                ""RegionCode"": 5,
                ""TrainWithUs"": ""2020"",
                ""TrainWithDisability"": ""2020"",
                ""AccreditingProviderEnrichments"": [
                {
                    ""Description"": ""2020"",
                    ""UcasProviderCode"": """ + AccreditingProviderCode + @"""
                }
                ]
            }";

            course2019.Provider.ProviderEnrichments.Add(
                new ProviderEnrichment
                {
                    ProviderCode = ProviderCode,
                    UpdatedAt = updatedAt,
                    JsonData = jsonData2019,
                    Status = EnumStatus.Published,
                    CreatedByUser = new User(),
                    UpdatedByUser = new User()
            });

            course2020.Provider.ProviderEnrichments.Add(
                new ProviderEnrichment
                {
                    ProviderCode = ProviderCode,
                    UpdatedAt = updatedAt.AddYears(1),
                    JsonData = jsonData2020,
                    Status = EnumStatus.Published,
                    CreatedByUser = new User(),
                    UpdatedByUser = new User()
            });

            Context.Save();
        }

        [Test]
        public void ReadAllCourseData_Returns_2019_Course()
        {
            var publisher = new Publisher(Config);

            var searchCourses = publisher.ReadAllCourseData(Context);

            searchCourses.Should().HaveCount(1,
                $"provider {ProviderCode} has one copy in the 2019 cycle and one in 2020");
            var course = searchCourses.Single();
            course.Provider.ProviderCode.Should().Be(ProviderCode);
            course.Name.Should().Be(CourseName2019);
        }

        [Test]
        public void ReadAllCourseData_Returns_2019_ProviderEnrichment()
        {
            var publisher = new Publisher(Config);

            var searchCourses = publisher.ReadAllCourseData(Context);

            var course = searchCourses.Single();

            course.AccreditingProvider.Should().NotBeNull();

            course.AccreditingProvider.ProviderCode.Should().Be(AccreditingProviderCode);

            course.DescriptionSections.Select(x => x.Text).All(x => string.IsNullOrWhiteSpace(x)).Should().BeFalse("The provider enrichment should be mapped.");

            var aboutTrainingProviderAccreditingSection = course.DescriptionSections.Single(cd => cd.Name == "about this training provider accrediting");
            aboutTrainingProviderAccreditingSection.Text.Should().Be(AboutAccreditingProvider2019);

            var aboutTrainingProviderSection = course.DescriptionSections.Single(cd => cd.Name == "about this training provider");
            aboutTrainingProviderSection.Text.Should().Be(TrainWithUs2019);

            var trainingDisabilitySection = course.DescriptionSections.Single(cd => cd.Name == "training with disabilities");
            trainingDisabilitySection.Text.Should().Be(TrainWithDisability2019);
        }
    }
}
