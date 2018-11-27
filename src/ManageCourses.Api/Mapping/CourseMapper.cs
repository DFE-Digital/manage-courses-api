using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using GovUk.Education.SearchAndCompare.Domain.Models.Joins;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class CourseMapper : ICourseMapper
    {
        private readonly SubjectMapper subjectMapper = new SubjectMapper();

        public SearchAndCompare.Domain.Models.Course MapToSearchAndCompareCourse(Domain.Models.Provider ucasProviderData, Domain.Models.Course ucasCourseData, ProviderEnrichmentModel providerEnrichmentModel, CourseEnrichmentModel courseEnrichmentModel)
        {
            ucasProviderData = ucasProviderData ?? new Domain.Models.Provider();
            ucasCourseData = ucasCourseData ?? new Domain.Models.Course();
            var sites = ucasCourseData.CourseSites ?? new ObservableCollection<CourseSite>();
            providerEnrichmentModel = providerEnrichmentModel ?? new ProviderEnrichmentModel();
            courseEnrichmentModel = courseEnrichmentModel ?? new CourseEnrichmentModel();

            var useUcasContact =
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Email) &&
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Website) &&
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Address1) &&
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Address2) &&
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Address3) &&
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Address4) &&
                string.IsNullOrWhiteSpace(providerEnrichmentModel.Postcode);

            var subjectStrings = ucasCourseData?.CourseSubjects != null
                ? subjectMapper.GetSubjectList(ucasCourseData.Name, ucasCourseData.CourseSubjects.Select(x => x.Subject.SubjectName))
                : new List<string>();

            var subjects = new Collection<SearchAndCompare.Domain.Models.Joins.CourseSubject>(subjectStrings.Select(subject =>
                new SearchAndCompare.Domain.Models.Joins.CourseSubject
                {
                    Subject = new SearchAndCompare.Domain.Models.Subject
                    {
                        Name = subject
                    }
                }).ToList());
            var isFurtherEducation = subjects.Any(c =>
                c.Subject.Name.Equals("Further education", StringComparison.InvariantCultureIgnoreCase));

            var provider = new SearchAndCompare.Domain.Models.Provider
            {
                Name = ucasProviderData.ProviderName,
                ProviderCode = ucasProviderData.ProviderCode
            };

            var accreditingProvider = ucasCourseData.AccreditingProvider == null ? null :
                new SearchAndCompare.Domain.Models.Provider
                {
                    Name = ucasCourseData.AccreditingProvider.ProviderName,
                    ProviderCode = ucasCourseData.AccreditingProvider.ProviderCode
                };

            var routeName = ucasCourseData.Route;
            var isSalaried = string.Equals(ucasCourseData?.ProgramType, "ss", StringComparison.InvariantCultureIgnoreCase)
                          || string.Equals(ucasCourseData?.ProgramType, "ta", StringComparison.InvariantCultureIgnoreCase);
            var fees = courseEnrichmentModel.FeeUkEu.HasValue ? new Fees
            {
                Uk = (int)(courseEnrichmentModel.FeeUkEu ?? 0),
                Eu = (int)(courseEnrichmentModel.FeeUkEu ?? 0),
                International = (int)(courseEnrichmentModel.FeeInternational ?? 0),
            } : new Fees();

            var address = useUcasContact ? MapAddress(ucasProviderData) : MapAddress(providerEnrichmentModel);
            var mappedCourse = new SearchAndCompare.Domain.Models.Course
            {
                ProviderLocation = new Location { Address = address },
                Duration = MapCourseLength(courseEnrichmentModel.CourseLength),
                StartDate = ucasCourseData.StartDate,
                Name = ucasCourseData.Name,
                ProgrammeCode = ucasCourseData.CourseCode,
                Provider = provider,
                AccreditingProvider = accreditingProvider,
                IsSen = ucasCourseData.IsSen,
                Route = new Route
                {
                    Name = routeName,
                    IsSalaried = isSalaried
                },
                IncludesPgce = MapQualification(ucasCourseData.Qualification),
                HasVacancies = ucasCourseData.HasVacancies,
                Campuses = new Collection<SearchAndCompare.Domain.Models.Campus>(sites
                    .Where(school => String.Equals(school.Status, "r", StringComparison.InvariantCultureIgnoreCase) && String.Equals(school.Publish, "y", StringComparison.InvariantCultureIgnoreCase))
                    .Select(school =>
                        new SearchAndCompare.Domain.Models.Campus
                        {
                            Name = school.Site.LocationName,
                            CampusCode = school.Site.Code,
                            Location = new Location
                            {
                                Address = MapAddress(school.Site)
                            },
                            VacStatus = school.VacStatus
                        }
                    ).ToList()),
                CourseSubjects = subjects,
                Fees = fees,

                IsSalaried = isSalaried,

                ContactDetails = new Contact
                {
                    Phone = useUcasContact ? ucasProviderData.Telephone : providerEnrichmentModel.Telephone,
                    Email = useUcasContact ? ucasProviderData.Email : providerEnrichmentModel.Email,
                    Website = useUcasContact ? ucasProviderData.Url : providerEnrichmentModel.Website,
                    Address = address
                },

                ApplicationsAcceptedFrom = sites.Select(x =>
                {
                    DateTime parsed;
                    return DateTime.TryParse(x.ApplicationsAcceptedFrom, out parsed) ? (DateTime?)parsed : null;
                }).Where(x => x != null && x.HasValue)
                    .OrderBy(x => x.Value)
                    .FirstOrDefault(),

                FullTime = ucasCourseData.StudyMode == "P" ? VacancyStatus.NA : VacancyStatus.Vacancies,
                PartTime = ucasCourseData.StudyMode == "F" ? VacancyStatus.NA : VacancyStatus.Vacancies,

                Mod = ucasCourseData.TypeDescription,
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
                Text = providerEnrichmentModel.TrainWithUs
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "about this training provider accrediting",//CourseDetailsSections.AboutTheAccreditingProvider,
                Text = GetAccreditingProviderEnrichment(ucasCourseData?.AccreditingProvider?.ProviderCode, providerEnrichmentModel)
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection
            {
                Name = "training with disabilities",//CourseDetailsSections.TrainWithDisabilities,
                Text = providerEnrichmentModel.TrainWithDisability
            });

            return mappedCourse;
        }

        private static IDictionary<CourseQualification, IncludesPgce> qualificationMap = new Dictionary<CourseQualification, IncludesPgce>
        {
            {CourseQualification.Qts, IncludesPgce.No},
            {CourseQualification.QtsWithPgce, IncludesPgce.Yes},
            {CourseQualification.QtsWithPgde, IncludesPgce.QtsWithPgde},
            {CourseQualification.QtlsWithPgce, IncludesPgce.QtlsWithPgce},
            {CourseQualification.QtlsWithPgde, IncludesPgce.QtlsWithPgde}
        };

        private IncludesPgce MapQualification(CourseQualification qualification)
        {
            if (qualificationMap.TryGetValue(qualification, out IncludesPgce result))
            {
                return result;
            }
            throw new ArgumentOutOfRangeException(nameof(qualification), qualification, "Could not map qualifications");
        }

        private string MapCourseLength(string courseLength)
        {
            return courseLength == "OneYear" ? "1 year"
                : courseLength == "TwoYears" ? "Up to 2 years"
                : courseLength;
        }

        private string GetAccreditingProviderEnrichment(string accreditingProviderCode, ProviderEnrichmentModel enrichmentModel)
        {
            if (string.IsNullOrWhiteSpace(accreditingProviderCode))
            {
                return "";
            }

            if (enrichmentModel.AccreditingProviderEnrichments == null)
            {
                return "";
            }

            var enrichment = enrichmentModel.AccreditingProviderEnrichments.FirstOrDefault(x => x.UcasProviderCode == accreditingProviderCode);

            if (enrichment == null)
            {
                return "";
            }

            return enrichment.Description;
        }

        private string MapAddress(Site school)
        {
            var addressFragments = new List<string>{
                school.Address1,
                school.Address2,
                school.Address3,
                school.Address4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = school.Postcode ?? "";

            return addressFragments.Any()
                ? String.Join(", ", addressFragments) + " " + postCode
                : postCode;
        }

        private string MapAddress(Domain.Models.Provider provider)
        {
            var addressFragments = new List<string>{
                provider.Address1,
                provider.Address2,
                provider.Address3,
                provider.Address4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = provider.Postcode ?? "";

            return addressFragments.Any()
                ? String.Join("\n", addressFragments) + "\n" + postCode
                : postCode;
        }

        private string MapAddress(ProviderEnrichmentModel orgEnrichmentModel)
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
