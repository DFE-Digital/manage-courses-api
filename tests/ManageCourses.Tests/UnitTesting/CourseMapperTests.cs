using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class CourseMapperTests
    {
        [Test]
        public void MapToSearchAndCompareCourse()
        {
            var mapper = new CourseMapper();
            var res = mapper.MapToSearchAndCompareCourse(
                GenerateUcasInstitution(),
                GenearteUcasCourse(),
                GenerateInstitutionEnrichmentWithoutContactDetails(),
                GenerateCourseEnrichmentModel()
            );

            res.Duration.Should().Be("1 year");
            res.Name.Should().Be("Course.Name");
            res.ProgrammeCode.Should().Be("CourseCode");
            res.ProviderCodeName.Should().Be("MYINST");

            res.Provider.ProviderCode.Should().Be("ABC");
            res.Provider.Name.Should().Be("My institution");
            res.AccreditingProvider.ProviderCode.Should().Be("ACC123");
            res.AccreditingProvider.Name.Should().Be("AccreditingProviderName");

            res.Route.Name.Should().Be("School Direct (salaried) training programme");
            res.Route.IsSalaried.Should().Be(true);

            res.IncludesPgce.Should().Be(IncludesPgce.Yes);
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

            res.FullTime.Should().Be(VacancyStatus.Vacancies);
            res.PartTime.Should().Be(VacancyStatus.Vacancies);
        }        

        [Test]
        public void MapToSearchAndCompareCourse_Nulls()
        {
            var mapper = new CourseMapper();
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

            var res = new CourseMapper().MapToSearchAndCompareCourse(
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

        private static Course GenearteUcasCourse()
        {
            return new Course
            {
                CourseCode = "CourseCode",
                AccreditingProviderId = "ACC123",
                AccreditingProviderName = "AccreditingProviderName",
                ProgramType = "SS", // school direct salaried
                Name = "Course.Name",
                ProfpostFlag = "T", // QTS+PGCE
                Subjects = "Mathematics, Physics",
                StudyMode = "B",
                Schools = new ObservableCollection<School>
                    {
                        new School
                        {
                            LocationName = "School.Name",
                            Address1 = "School.Address1",
                            Address2 = "School.Address2",
                            Address3 = "School.Address3",
                            Address4 = "School.Address4",
                            PostCode = "School.PostCode",
                            Code = "SCH",
                            ApplicationsAcceptedFrom = "2018-10-16 00:00:00",
                            FullTimeVacancies = "",
                            PartTimeVacancies = "",
                            Status = "r"
                        },

                        new School
                        {
                            LocationName = "NotIncludedSchool.Name",
                            Address1 = "NotIncludedSchool.Address1",
                            Address2 = "NotIncludedSchool.Address2",
                            Address3 = "NotIncludedSchool.Address3",
                            Address4 = "NotIncludedSchool.Address4",
                            PostCode = "NotIncludedSchool.PostCode",
                            Code = "SCHNI",
                            ApplicationsAcceptedFrom = "2018-10-16 00:00:00",
                            FullTimeVacancies = "",
                            PartTimeVacancies = "",
                            Status = "d"
                        },

                    }
            };
        }

        private static UcasInstitution GenerateUcasInstitution()
        {
            return new UcasInstitution
            {
                Addr1 = "Addr1",
                Addr2 = "Addr2",
                Addr3 = "Addr3",
                Addr4 = "Addr4",
                Postcode = "Postcode",
                Url = "http://www.example.com",

                InstCode = "ABC",
                InstBig = "MYINST",
                InstFull = "My institution"
            };
        }

    }
}
