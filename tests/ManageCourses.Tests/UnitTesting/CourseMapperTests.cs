using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;
using Provider = GovUk.Education.ManageCourses.Domain.Models.Provider;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class CourseMapperTests
    {
        private ICourseMapper mapper = new CourseMapper();

        [Test]
        public void MapToSearchAndCompareCourse_ProviderLocation()
        {
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                GenerateUcasCourse(),
                GenerateProviderEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.ProviderLocation.Should().NotBeNull();
            res.ProviderLocation.Address.Should().NotBeNull();
            res.ContactDetails.Address.Should().NotBeNull();
            res.ProviderLocation.Address.Should().Be(res.ContactDetails.Address);
            res.IsSen.Should().BeFalse();
        }

        [Test]
        public void MapToSearchAndCompareCourse_IsSend()
        {
            var ucasCourse = GenerateUcasCourse();
            ucasCourse.IsSend = true;
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                ucasCourse,
                GenerateProviderEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.ProviderLocation.Should().NotBeNull();
            res.ProviderLocation.Address.Should().NotBeNull();
            res.ContactDetails.Address.Should().NotBeNull();
            res.ProviderLocation.Address.Should().Be(res.ContactDetails.Address);
            res.IsSen.Should().BeTrue();
        }

        [Test]
        public void MapToSearchAndCompareCourse_Fees()
        {
            var courseEnrichmentModel = GenerateCourseEnrichmentModel();
            courseEnrichmentModel.FeeUkEu = null;

            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                GenerateUcasCourse(),
                GenerateProviderEnrichmentWithoutContactDetails(),
                courseEnrichmentModel
            );

            res.Fees.Should().NotBeNull();
            res.Fees.Eu.Should().Be(0);
            res.Fees.International.Should().Be(0);
            res.Fees.Uk.Should().Be(0);
            res.IsSen.Should().BeFalse();
        }

        [Test]
        public void MapToSearchAndCompareCourse()
        {
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                GenerateUcasCourse(true),
                GenerateProviderEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.Duration.Should().Be("1 year");
            res.Name.Should().Be("Course.Name");
            res.ProgrammeCode.Should().Be("CourseCode");

            res.Provider.ProviderCode.Should().Be("ABC");
            res.Provider.Name.Should().Be("My provider");
            res.AccreditingProvider.ProviderCode.Should().Be("ACC123");
            res.AccreditingProvider.Name.Should().Be("AccreditingProviderName");

            res.Route.Name.Should().Be("School Direct (salaried) training programme");
            res.Route.IsSalaried.Should().Be(true);

            res.IncludesPgce.Should().Be(SearchAndCompare.Domain.Models.Enums.IncludesPgce.Yes);
            res.IsSalaried.Should().BeTrue();

            res.Campuses.Count.Should().Be(1);
            res.Campuses.Single().Name.Should().Be("School.Name");
            res.Campuses.Single().CampusCode.Should().Be("SCH");
            res.Campuses.Single().Location.Address.Should().Be("School.Address1, School.Address2, School.Address3, School.Address4 School.PostCode");


            res.CourseSubjects.Count.Should().Be(2);
            res.CourseSubjects.Any(x => x.Subject.Name == "Mathematics").Should().BeTrue();
            res.CourseSubjects.Any(x => x.Subject.Name == "Physics").Should().BeTrue();

            res.Fees.Uk.Should().Be(123);
            res.Fees.Eu.Should().Be(123);
            res.Fees.International.Should().Be(123000);

            res.ContactDetails.Website.Should().Be("http://www.example.com");
            res.ContactDetails.Address.Should().Be("Addr1\nAddr2\nAddr3\nAddr4\nPostcode");

            res.ApplicationsAcceptedFrom.Should().Be(new System.DateTime(2018, 10, 16));

            res.FullTime.Should().Be(SearchAndCompare.Domain.Models.Enums.VacancyStatus.Vacancies);
            res.PartTime.Should().Be(SearchAndCompare.Domain.Models.Enums.VacancyStatus.Vacancies);
            res.IsSen.Should().BeFalse();
        }

        [Test]
        public void MapToSearchAndCompareCourse_with_no_published_campuses()
        {
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                GenerateUcasCourse(),
                GenerateProviderEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.Duration.Should().Be("1 year");
            res.Name.Should().Be("Course.Name");
            res.ProgrammeCode.Should().Be("CourseCode");

            res.Provider.ProviderCode.Should().Be("ABC");
            res.Provider.Name.Should().Be("My provider");
            res.AccreditingProvider.ProviderCode.Should().Be("ACC123");
            res.AccreditingProvider.Name.Should().Be("AccreditingProviderName");

            res.Route.Name.Should().Be("School Direct (salaried) training programme");
            res.Route.IsSalaried.Should().Be(true);

            res.IncludesPgce.Should().Be(SearchAndCompare.Domain.Models.Enums.IncludesPgce.Yes);
            res.IsSalaried.Should().BeTrue();

            res.Campuses.Count.Should().Be(0);

            res.CourseSubjects.Count.Should().Be(2);
            res.CourseSubjects.Any(x => x.Subject.Name == "Mathematics").Should().BeTrue();
            res.CourseSubjects.Any(x => x.Subject.Name == "Physics").Should().BeTrue();

            res.Fees.Uk.Should().Be(123);
            res.Fees.Eu.Should().Be(123);
            res.Fees.International.Should().Be(123000);

            res.ContactDetails.Website.Should().Be("http://www.example.com");
            res.ContactDetails.Address.Should().Be("Addr1\nAddr2\nAddr3\nAddr4\nPostcode");

            res.ApplicationsAcceptedFrom.Should().BeNull();

            res.FullTime.Should().Be(SearchAndCompare.Domain.Models.Enums.VacancyStatus.Vacancies);
            res.PartTime.Should().Be(SearchAndCompare.Domain.Models.Enums.VacancyStatus.Vacancies);
            res.IsSen.Should().BeFalse();
        }

        [Test]
        public void MapToSearchAndCompareCourse_Nulls()
        {
            Assert.DoesNotThrow(() => mapper.MapToSearchAndCompareCourse(null, null, null, null));
        }

        [Test]
        public void MapToSearchAndCompareCourse_EnrichmentContactDetailsPreferred()
        {
            var providerEnrichment = GenerateProviderEnrichmentWithoutContactDetails();

            providerEnrichment.Email = "overridden@email.com";

            providerEnrichment.Website = "https://overridden.com";

            providerEnrichment.Address1 = "Overridden1";
            //nb Address2 is optional
            providerEnrichment.Address3 = "Overridden3";
            providerEnrichment.Address4 = "Overridden4";

            providerEnrichment.Postcode = "OverriddenPostcode";

            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                GenerateUcasCourse(),
                providerEnrichment,
                GenerateCourseEnrichmentModel()
            );

            res.ContactDetails.Address.Should().Be("Overridden1\nOverridden3\nOverridden4\nOverriddenPostcode");
            res.ContactDetails.Email.Should().Be("overridden@email.com");
            res.ContactDetails.Website.Should().Be("https://overridden.com");
            res.IsSen.Should().BeFalse();

        }
        [Test]
        public void MapToSearchAndCompareCourse_VacStatus()
        {
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasProvider(),
                GenerateUcasCourse(true),
                GenerateProviderEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.Campuses.Any(c => c.VacStatus == "F").Should().BeTrue();
            res.HasVacancies.Should().BeTrue();
            res.IsSen.Should().BeFalse();
        }

        private static CourseEnrichmentModel GenerateCourseEnrichmentModel()
        {
            return new CourseEnrichmentModel
            {
                AboutCourse = "AboutCourse",
                InterviewProcess = "InterviewProcess",
                HowSchoolPlacementsWork = "HowSchoolPlacementsWork",
                CourseLength = "OneYear",
                FeeUkEu = 123,
                FeeInternational = 123000,
                FeeDetails = "FeeDetails",
                FinancialSupport = "FinancialSupport",
                Qualifications = "Qualifications",
                PersonalQualities = "PersonalQualities",
                OtherRequirements = "OtherRequirements"
            };
        }

        private static ProviderEnrichmentModel GenerateProviderEnrichmentWithoutContactDetails()
        {
            return new ProviderEnrichmentModel
            {
                TrainWithUs = "TrainWithUs",
                TrainWithDisability = "TrainWithDisability",
                AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasProviderCode = "ACC123",
                            Description = "AccreditingProviderDescription"
                        }
                    }
            };
        }

        private static Course GenerateUcasCourse(bool publishedCampus = false)
        {
            return new Course
            {
                CourseCode = "CourseCode",
                AccreditingProvider = new Provider {ProviderCode  = "ACC123", ProviderName = "AccreditingProviderName"},
                Qualification = CourseQualification.QtsWithPgce,
                ProgramType = "SS", // school direct salaried
                Name = "Course.Name",
                ProfpostFlag = "T", // QTS+PGCE
                CourseSubjects = new List<CourseSubject> {
                    new CourseSubject { Subject = new Subject { SubjectName = "Mathematics"}},
                    new CourseSubject { Subject = new Subject { SubjectName = "Physics"}}
                },

                StudyMode = "B",
                CourseSites = new List<CourseSite>
                    {

                        new CourseSite { Site = new Site
                        {
                            LocationName = "School.Name",
                            Address1 = "School.Address1",
                            Address2 = "School.Address2",
                            Address3 = "School.Address3",
                            Address4 = "School.Address4",
                            Postcode = "School.PostCode",
                            Code = "SCH"
                        },
                        ApplicationsAcceptedFrom = DateTime.Parse("2018-10-16 00:00:00"),
                        Status = "r",
                        Publish = publishedCampus ? "Y" : "n",
                        VacStatus="F"
                        },

                        new CourseSite {
                            Site = new Site
                            {
                                LocationName = "NotIncludedSchool.Name",
                                Address1 = "NotIncludedSchool.Address1",
                                Address2 = "NotIncludedSchool.Address2",
                                Address3 = "NotIncludedSchool.Address3",
                                Address4 = "NotIncludedSchool.Address4",
                                Postcode = "NotIncludedSchool.PostCode",
                                Code = "SCHNI"
                            },
                            ApplicationsAcceptedFrom = DateTime.Parse("2018-10-16 00:00:00"),
                            Status = "d",
                            Publish = publishedCampus ? "Y" : "n",
                            VacStatus="F"
                            },

                    }
            };
        }

        private static Provider GenerateUcasProvider()
        {
            return new Provider
            {
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Postcode = "Postcode",
                Url = "http://www.example.com",

                ProviderCode = "ABC",
                ProviderName = "My provider"
            };
        }

    }
}
