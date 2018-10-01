using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Helpers;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using GovUk.Education.SearchAndCompare.Domain.Models.Joins;
using Course = GovUk.Education.ManageCourses.Api.Model.Course;


namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class CourseMapper : ICourseMapper
    {
        private readonly SubjectMapper _subjectMapper = new SubjectMapper();
        private readonly QualificationMapper _qualificationMapper = new QualificationMapper();

        public SearchAndCompare.Domain.Models.Course MapToSearchAndCompareCourse(UcasInstitution ucasInstData, Course ucasCourseData, InstitutionEnrichmentModel orgEnrichmentModel, CourseEnrichmentModel courseEnrichmentModel, bool isPgde)
        {
            ucasInstData = ucasInstData ?? new UcasInstitution();
            ucasCourseData = ucasCourseData ?? new Course();
            ucasCourseData.Schools = ucasCourseData.Schools ?? new ObservableCollection<School>();
            orgEnrichmentModel = orgEnrichmentModel ?? new InstitutionEnrichmentModel();
            courseEnrichmentModel = courseEnrichmentModel ?? new CourseEnrichmentModel();

            var useUcasContact =
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Email) &&
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Website) &&
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Address1) &&
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Address2) &&
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Address3) &&
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Address4) &&
                string.IsNullOrWhiteSpace(orgEnrichmentModel.Postcode);

            var subjectStrings = string.IsNullOrWhiteSpace(ucasCourseData.Subjects)
                ? new string[]{}
                : _subjectMapper.GetSubjectList(ucasCourseData.Name, ucasCourseData.Subjects.Split(", "));

            var subjects = new Collection<CourseSubject>(subjectStrings.Select(subject =>
                new CourseSubject
                {
                    Subject = new Subject
                    {
                        Name = subject
                    }
                }).ToList());
            var isFurtherEducation = subjects.Any(c =>
                c.Subject.Name.Equals("Further education", StringComparison.InvariantCultureIgnoreCase));

            var provider = new SearchAndCompare.Domain.Models.Provider
            {
                Name = ucasInstData.InstFull,
                ProviderCode = ucasInstData.InstCode
            };

            var accreditingProvider = ucasCourseData.AccreditingProviderId == null ? null :
                new SearchAndCompare.Domain.Models.Provider
                {
                    Name = ucasCourseData.AccreditingProviderName,
                    ProviderCode = ucasCourseData.AccreditingProviderId
                };

            var routeName = ucasCourseData.GetRoute();
            var isSalaried = string.Equals(ucasCourseData?.ProgramType, "ss", StringComparison.InvariantCultureIgnoreCase)
                          || string.Equals(ucasCourseData?.ProgramType, "ta", StringComparison.InvariantCultureIgnoreCase);
            var fees = courseEnrichmentModel.FeeUkEu.HasValue ? new Fees
            {
                Uk = (int)(courseEnrichmentModel.FeeUkEu ?? 0),
                Eu = (int)(courseEnrichmentModel.FeeUkEu ?? 0),
                International = (int)(courseEnrichmentModel.FeeInternational ?? 0),
            } : new Fees();

            var address = useUcasContact ? MapAddress(ucasInstData) :  MapAddress(orgEnrichmentModel);
            var mappedCourse = new SearchAndCompare.Domain.Models.Course
            {
                ProviderLocation = new Location { Address = address},
                Duration = MapCourseLength(courseEnrichmentModel.CourseLength),
                StartDate = ucasCourseData.StartDate,
                Name = ucasCourseData.Name,
                ProgrammeCode = ucasCourseData.CourseCode,
                Provider = provider,
                ProviderCodeName = ucasInstData.InstBig,
                AccreditingProvider = accreditingProvider,
                Route = new Route
                {
                    Name = routeName,
                    IsSalaried = isSalaried
                },
                IncludesPgce = _qualificationMapper.MapQualification(ucasCourseData.ProfpostFlag, isFurtherEducation, isPgde),
                Campuses = new Collection<SearchAndCompare.Domain.Models.Campus>(ucasCourseData.Schools
                    .Where(school => String.Equals(school.Status, "r", StringComparison.InvariantCultureIgnoreCase))
                    .Select(school =>
                        new SearchAndCompare.Domain.Models.Campus
                        {
                            Name = school.LocationName,
                            CampusCode = school.Code,
                            Location = new Location
                            {
                                Address = MapAddress(school)
                            }
                        }
                    ).ToList()),
                CourseSubjects = subjects,
                Fees = fees,

                IsSalaried = isSalaried,

                ContactDetails = new Contact
                {
                    Phone = useUcasContact ? ucasInstData.Telephone : orgEnrichmentModel.Telephone,
                    Email = useUcasContact ? ucasInstData.Email : orgEnrichmentModel.Email,
                    Website = useUcasContact ? ucasInstData.Url : orgEnrichmentModel.Website,
                    Address = address
                },

                ApplicationsAcceptedFrom = ucasCourseData.Schools.Select(x => {
                    DateTime parsed;
                    return DateTime.TryParse(x.ApplicationsAcceptedFrom, out parsed) ? (DateTime?)parsed : null;
                }).Where(x => x != null && x.HasValue)
                    .OrderBy(x => x.Value)
                    .FirstOrDefault(),

                FullTime = ucasCourseData.StudyMode == "P" ? VacancyStatus.NA : VacancyStatus.Vacancies,
                PartTime = ucasCourseData.StudyMode == "F" ? VacancyStatus.NA : VacancyStatus.Vacancies,

                Mod = ucasCourseData.GetCourseVariantType(),
            };

            mappedCourse.DescriptionSections = new Collection<CourseDescriptionSection>();

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                //TODO move the CourseDetailsSections constants into SearchAndCompare.Domain.Models
                // but this will work ftm
                Name = "about this training programme",//CourseDetailsSections.AboutTheCourse,
                Text = courseEnrichmentModel.AboutCourse
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "interview process",//CourseDetailsSections.InterviewProcess,
                Text = courseEnrichmentModel.InterviewProcess
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "about fees",//CourseDetailsSections.AboutFees,
                Text = courseEnrichmentModel.FeeDetails
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "about salary",//CourseDetailsSections.AboutSalary,
                Text = courseEnrichmentModel.SalaryDetails
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "entry requirements",//CourseDetailsSections.EntryRequirementsQualifications,
                Text = courseEnrichmentModel.Qualifications
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "entry requirements personal qualities",//CourseDetailsSections.EntryRequirementsPersonalQualities,
                Text = courseEnrichmentModel.PersonalQualities
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "entry requirements other",//CourseDetailsSections.EntryRequirementsOther,
                Text = courseEnrichmentModel.OtherRequirements
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "financial support",//CourseDetailsSections.FinancialSupport,
                Text = courseEnrichmentModel.FinancialSupport
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "about school placements",//CourseDetailsSections.AboutSchools,
                Text = courseEnrichmentModel.HowSchoolPlacementsWork
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "about this training provider",//CourseDetailsSections.AboutTheProvider,
                Text = orgEnrichmentModel.TrainWithUs
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "about this training provider accrediting",//CourseDetailsSections.AboutTheAccreditingProvider,
                Text = GetAccreditingProviderEnrichment(ucasCourseData.AccreditingProviderId, orgEnrichmentModel)
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "training with disabilities",//CourseDetailsSections.TrainWithDisabilities,
                Text = orgEnrichmentModel.TrainWithDisability
            });

            return mappedCourse;
        }

        private string MapCourseLength(string courseLength)
        {
            return courseLength == "OneYear" ? "1 year"
                : courseLength == "TwoYears" ? "Up to 2 years"
                : courseLength;
        }

        private string GetAccreditingProviderEnrichment(string accreditingProviderId, InstitutionEnrichmentModel enrichmentModel)
        {
            if (string.IsNullOrWhiteSpace(accreditingProviderId))
            {
                return "";
            }

            if (enrichmentModel.AccreditingProviderEnrichments == null)
            {
                return "";
            }

            var enrichment = enrichmentModel.AccreditingProviderEnrichments.FirstOrDefault(x => x.UcasInstitutionCode == accreditingProviderId);

            if (enrichment == null)
            {
                return "";
            }

            return enrichment.Description;
        }

        private string MapAddress(School school)
        {
            var addressFragments = new List<string>{
                school.Address1,
                school.Address2,
                school.Address3,
                school.Address4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = school.PostCode ?? "";

            return addressFragments.Any()
                ? String.Join(", ", addressFragments) + " " + postCode
                : postCode;
        }

        private string MapAddress(UcasInstitution inst)
        {
            var addressFragments = new List<string>{
                inst.Addr1,
                inst.Addr2,
                inst.Addr3,
                inst.Addr4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = inst.Postcode ?? "";

            return addressFragments.Any()
                ? String.Join("\n", addressFragments) + "\n" + postCode
                : postCode;
        }        

        private string MapAddress(InstitutionEnrichmentModel orgEnrichmentModel)
        {
            var addressFragments = new List<string>{
                orgEnrichmentModel.Address1,
                orgEnrichmentModel.Address2,
                orgEnrichmentModel.Address3,
                orgEnrichmentModel.Address4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = orgEnrichmentModel.Postcode ?? "";

            return addressFragments.Any()
                ? String.Join("\n", addressFragments) + "\n" + postCode
                : postCode;
        }
    }
}
