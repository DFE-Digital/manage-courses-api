using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
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
        private readonly SubjectMapper subjectMapper = new SubjectMapper();

        /// <summary>
        /// Takes the UcasCourse records which are actually de-normalised course-campus info and turns them into
        /// proper British Courses that have the campus info re-normalised into the .Schools list property.
        /// </summary>
        /// <param name="courseRecords">UcasCourse records</param>
        /// <param name="enrichmentMetadata"></param>
        /// <param name="pgdeCourses"></param>
        /// <returns></returns>        
        public List<Course> LoadCourses(IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseSubject> courseSubjects, IEnumerable<UcasSubject> ucasSubjects, IEnumerable<PgdeCourse> pgdeCourses, IEnumerable<Subject> allSubjects, IEnumerable<Site> allSites, IEnumerable<Institution> allInstitutions)
        {
            var returnCourses = new List<Course>();
            
            var instDictionary = allInstitutions.Where(x => !string.IsNullOrWhiteSpace(x.InstCode)).ToDictionary(x => x.InstCode);

            // nb - this separator uses characters that are never used in inst codes - thus avoiding ambiguity
            var campusGroupings = courseRecords.GroupBy(x => x.InstCode + "_@@_" + x.CampusCode);
            var courseRecordGroupings = courseRecords.GroupBy(x => x.InstCode + "_@@_" + x.CrseCode);
            var pgdeCoursesSimple = pgdeCourses.Select(x => x.InstCode + "_@@_" + x.CourseCode).ToList();
            var courseSubjectGroupings = courseSubjects.GroupBy(x => x.InstCode + "_@@_" + x.CrseCode).ToDictionary(x => x.Key, x => x);

            foreach (var grouping in courseRecordGroupings)
            {
                returnCourses.Add(LoadCourse(
                    grouping.ToList(),
                    courseSubjectGroupings.GetValueOrDefault(grouping.Key) ?? (IEnumerable<UcasCourseSubject>) new List<UcasCourseSubject>(),
                    ucasSubjects,
                    pgdeCoursesSimple.Contains(grouping.Key),
                    allSubjects,
                    allSites,
                    instDictionary));
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
        private Course LoadCourse(IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseSubject> courseSubjects, IEnumerable<UcasSubject> ucasSubjects, bool isPgde, IEnumerable<Subject> allSubjects, IEnumerable<Site> allSites, IDictionary<string, Institution> allInstitutions)
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
                    returnCourse.AccreditingInstitution = allInstitutions[organisationCourseRecord.AccreditingProvider];
                }

                if (!string.IsNullOrWhiteSpace(organisationCourseRecord.InstCode))
                {
                    returnCourse.Institution = allInstitutions[organisationCourseRecord.InstCode];
                }
                returnCourse.CourseCode = organisationCourseRecord.CrseCode;
                returnCourse.AgeRange = organisationCourseRecord.Age;
                returnCourse.Name = organisationCourseRecord.CrseTitle;
                returnCourse.ProgramType = organisationCourseRecord.ProgramType;
                returnCourse.ProfpostFlag = organisationCourseRecord.ProfpostFlag;
                returnCourse.StudyMode = organisationCourseRecord.Studymode;
                returnCourse.StartDate = DateTime.TryParse($"{organisationCourseRecord.StartYear} {organisationCourseRecord.StartMonth}", out DateTime startDate) ? (DateTime?) startDate : null;
                
                var ucasSubjectsForThisCourse = courseSubjects.Select(x => ucasSubjects.Single(y => x.SubjectCode == y.SubjectCode).SubjectDescription);

                var mappedSubjects = subjectMapper.MapToSecondarySubjects(organisationCourseRecord.CrseTitle, ucasSubjectsForThisCourse)
                    .Select(s => new CourseSubject { Subject = allSubjects.Single(x => x.SubjectName == s)});

                returnCourse.CourseSubjects = new Collection<CourseSubject>(mappedSubjects.ToList());
                returnCourse.CourseSites = new Collection<CourseSite>(courseRecords.Select(x => new CourseSite
                { 
                    Site = allSites.Single(y => y.Institution?.InstCode == x.InstCode && y.Code == x.CampusCode),                    
                    ApplicationsAcceptedFrom = x.CrseOpenDate,
                    Status = x.Status,
                    Publish = x.Publish,
                    VacStatus = x.VacStatus
                }).ToList());
                returnCourse.Qualification = qualificationMapper.MapQualification(
                    organisationCourseRecord.ProfpostFlag,
                    subjectMapper.IsFurtherEducation(ucasSubjectsForThisCourse),
                    isPgde);
                
                const string both = "B";
                const string fullTime = "F";
                const string partTime = "P";
                var ucasVacancyStatusCodes = new HashSet<string> { both, fullTime, partTime };
            }

            return returnCourse;
        }

    }
}
