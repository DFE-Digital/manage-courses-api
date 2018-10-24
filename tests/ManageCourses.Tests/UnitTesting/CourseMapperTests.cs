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
using Institution = GovUk.Education.ManageCourses.Domain.Models.Institution;

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
                GenerateUcasInstitution(),
                GenearteUcasCourse(),
                GenerateInstitutionEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.ProviderLocation.Should().NotBeNull();
            res.ProviderLocation.Address.Should().NotBeNull();
            res.ContactDetails.Address.Should().NotBeNull();
            res.ProviderLocation.Address.Should().Be(res.ContactDetails.Address);
        }

        [Test]
        public void MapToSearchAndCompareCourse_Fees()
        {
            var courseEnrichmentModel = GenerateCourseEnrichmentModel();
            courseEnrichmentModel.FeeUkEu = null;

            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasInstitution(),
                GenearteUcasCourse(),
                GenerateInstitutionEnrichmentWithoutContactDetails(),
                courseEnrichmentModel
            );

            res.Fees.Should().NotBeNull();
            res.Fees.Eu.Should().Be(0);
            res.Fees.International.Should().Be(0);
            res.Fees.Uk.Should().Be(0);
        }

        [Test]
        public void MapToSearchAndCompareCourse()
        {
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasInstitution(),
                GenearteUcasCourse(true),
                GenerateInstitutionEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.Duration.Should().Be("1 year");
            res.Name.Should().Be("Course.Name");
            res.ProgrammeCode.Should().Be("CourseCode");

            res.Provider.ProviderCode.Should().Be("ABC");
            res.Provider.Name.Should().Be("My institution");
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
        }

        [Test]
        public void MapToSearchAndCompareCourse_with_no_published_campuses()
        {
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasInstitution(),
                GenearteUcasCourse(),
                GenerateInstitutionEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.Duration.Should().Be("1 year");
            res.Name.Should().Be("Course.Name");
            res.ProgrammeCode.Should().Be("CourseCode");

            res.Provider.ProviderCode.Should().Be("ABC");
            res.Provider.Name.Should().Be("My institution");
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

            res.ApplicationsAcceptedFrom.Should().Be(new System.DateTime(2018, 10, 16));

            res.FullTime.Should().Be(SearchAndCompare.Domain.Models.Enums.VacancyStatus.Vacancies);
            res.PartTime.Should().Be(SearchAndCompare.Domain.Models.Enums.VacancyStatus.Vacancies);
        }

        [Test]
        public void MapToSearchAndCompareCourse_Nulls()
        {
            Assert.DoesNotThrow(() => mapper.MapToSearchAndCompareCourse(null, null, null, null));
        }

        [Test]
        public void MapToSearchAndCompareCourse_EnrichmentContactDetailsPreferred()
        {
            var instEnrichment = GenerateInstitutionEnrichmentWithoutContactDetails();

            instEnrichment.Email = "overridden@email.com";

            instEnrichment.Website = "https://overridden.com";

            instEnrichment.Address1 = "Overridden1";
            //nb Address2 is optional
            instEnrichment.Address3 = "Overridden3";
            instEnrichment.Address4 = "Overridden4";

            instEnrichment.Postcode = "OverriddenPostcode";

            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasInstitution(),
                GenearteUcasCourse(),
                instEnrichment,
                GenerateCourseEnrichmentModel()
            );

            res.ContactDetails.Address.Should().Be("Overridden1\nOverridden3\nOverridden4\nOverriddenPostcode");
            res.ContactDetails.Email.Should().Be("overridden@email.com");
            res.ContactDetails.Website.Should().Be("https://overridden.com");

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

        private static InstitutionEnrichmentModel GenerateInstitutionEnrichmentWithoutContactDetails()
        {
            return new InstitutionEnrichmentModel
            {
                TrainWithUs = "TrainWithUs",
                TrainWithDisability = "TrainWithDisability",
                AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>
                    {
                        new AccreditingProviderEnrichment
                        {
                            UcasInstitutionCode = "ACC123",
                            Description = "AccreditingProviderDescription"
                        }
                    }
            };
        }

        private static Course GenearteUcasCourse(bool publishedCampus = false)
        {
            return new Course
            {
                CourseCode = "CourseCode",
                AccreditingInstitution = new Institution {InstCode  = "ACC123", InstFull = "AccreditingProviderName"},
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
                        ApplicationsAcceptedFrom = "2018-10-16 00:00:00",
                        Status = "r",
                        Publish = publishedCampus ? "Y" : "n"
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
                            ApplicationsAcceptedFrom = "2018-10-16 00:00:00",
                            Status = "d",
                            Publish = publishedCampus ? "Y" : "n"
                            },

                    }
            };
        }

        private static Institution GenerateUcasInstitution()
        {
            return new Institution
            {
                Address1 = "Addr1",
                Address2 = "Addr2",
                Address3 = "Addr3",
                Address4 = "Addr4",
                Postcode = "Postcode",
                Url = "http://www.example.com",

                InstCode = "ABC",
                InstFull = "My institution"
            };
        }

    }
}
