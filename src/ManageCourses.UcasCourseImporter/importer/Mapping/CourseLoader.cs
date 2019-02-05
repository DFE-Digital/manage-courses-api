using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Xls.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.UcasCourseImporter.Mapping
{
    public class CourseLoader
    {
        private readonly QualificationMapper qualificationMapper = new QualificationMapper();
        private Dictionary<string, Provider> allProviders;
        private readonly List<string> pgdeCourses;
        private readonly Dictionary<string, Subject> allSubjects;

        public CourseLoader(Dictionary<string, Provider> allProviders, Dictionary<string, Subject> allSubjects, List<PgdeCourse> pgdeCourses)
        {
            this.allProviders = allProviders;
            this.pgdeCourses = pgdeCourses.Select(x => x.ProviderCode + "_@@_" + x.CourseCode).ToList();
            this.allSubjects = allSubjects;
        }

        /// <summary>
        /// Takes the UcasCourse records which are actually de-normalised course-campus info and turns them into
        /// actual courses that have the campus info re-normalised into the .Schools list property.
        /// </summary>
        /// <param name="provider">Provider</param>
        /// <param name="courseRecords">UcasCourse records</param>
        /// <param name="enrichmentMetadata"></param>
        /// <param name="pgdeCourses"></param>
        /// <returns></returns>
        public List<Course> LoadCourses(Provider provider, IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseSubject> courseSubjects, IEnumerable<Site> allSites)
        {
            var returnCourses = new List<Course>();

            // nb - this separator uses characters that are never used in inst codes - thus avoiding ambiguity
            var campusGroupings = courseRecords.GroupBy(x => x.InstCode + "_@@_" + x.CampusCode);
            var courseRecordGroupings = courseRecords.GroupBy(x => x.InstCode + "_@@_" + x.CrseCode);
            var courseSubjectGroupings = courseSubjects.GroupBy(x => x.InstCode + "_@@_" + x.CrseCode).ToDictionary(x => x.Key, x => x);

            foreach (var grouping in courseRecordGroupings)
            {
                returnCourses.Add(LoadCourse(
                    provider,
                    grouping.ToList(),
                    courseSubjectGroupings.GetValueOrDefault(grouping.Key).AsEnumerable() ?? new List<UcasCourseSubject>(),
                    allSites));
            }

            return returnCourses;
        }

        /// <summary>
        /// Takes the list of campus info within a single course and maps it into
        /// our version of a course, with the sites being read from the campus info in the UcasCourse (aka campus)
        /// </summary>
        /// <param name="courseRecords">List of UcasCourse records for a single course (no really a course, not the course-campus combination in ucas-land)</param>
        /// <param name="isPgde"></param>
        /// <returns></returns>
        private Course LoadCourse(Provider provider, IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseSubject> courseSubjects, IEnumerable<Site> allSites)
        {
            var returnCourse = new Course();
            if (courseRecords.Count() > 0)
            {
                // pick a reference course record for shared data such as the accrediting provider
                // users can edit only:
                // running locations in UCAS and
                // published locations in UCAS
                // so give preference to using running and published locations
                // if there are any.
                var organisationCourseRecord =
                    courseRecords.FirstOrDefault(x => x.Status != null && x.Status.ToLowerInvariant() == "r" && x.Publish != null && x.Publish.ToLowerInvariant() == "y")
                    ?? courseRecords.First();

                if (!string.IsNullOrWhiteSpace(organisationCourseRecord.AccreditingProvider))
                {
                    returnCourse.AccreditingProvider = allProviders[organisationCourseRecord.AccreditingProvider];
                }

                if (!string.IsNullOrWhiteSpace(organisationCourseRecord.InstCode))
                {
                    returnCourse.Provider = allProviders[organisationCourseRecord.InstCode];
                }
                returnCourse.CourseCode = organisationCourseRecord.CrseCode;
                returnCourse.AgeRange = organisationCourseRecord.Age;
                returnCourse.Name = organisationCourseRecord.CrseTitle;
                returnCourse.ProgramType = organisationCourseRecord.ProgramType;
                returnCourse.ProfpostFlag = organisationCourseRecord.ProfpostFlag;
                returnCourse.StudyMode = organisationCourseRecord.Studymode;
                returnCourse.Modular = organisationCourseRecord.Modular;
                returnCourse.English = organisationCourseRecord.English;
                returnCourse.Maths = organisationCourseRecord.Maths;
                returnCourse.Science = organisationCourseRecord.Science;
                returnCourse.StartDate = DateTime.TryParse($"{organisationCourseRecord.StartYear} {organisationCourseRecord.StartMonth}", out DateTime startDate) ? (DateTime?) startDate : null;

                returnCourse.CourseSubjects = new Collection<CourseSubject>(courseSubjects.Select(x => new CourseSubject {
                    Subject = allSubjects[x.SubjectCode],
                    Course = returnCourse
                }).ToList());

                returnCourse.CourseSites = new Collection<CourseSite>(courseRecords.Select(x => new CourseSite
                {
                    Site = allSites.Single(y => y.Provider?.ProviderCode == x.InstCode && y.Code == x.CampusCode),
                    ApplicationsAcceptedFrom = UcasStringParser.GetDateTimeFromString(x.CrseOpenDate),
                    Status = x.Status,
                    Publish = x.Publish,
                    VacStatus = x.VacStatus
                }).ToList());

                returnCourse.Qualification = qualificationMapper.MapQualification(
                    organisationCourseRecord.ProfpostFlag,
                    new SubjectMapper().IsFurtherEducation(returnCourse.CourseSubjects.Select(x => x.Subject.SubjectName)),
                    pgdeCourses.Contains(organisationCourseRecord.InstCode + "_@@_" + organisationCourseRecord.CrseCode));
            }

            return returnCourse;
        }

    }
}
