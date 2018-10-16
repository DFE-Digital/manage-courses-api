﻿using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
using GovUk.Education.ManageCourses.Api.Helpers;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.EqualityComparers;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class CourseLoader
    {
        private readonly QualificationMapper qualificationMapper = new QualificationMapper();
        private readonly SubjectMapper subjectMapper = new SubjectMapper();

        /// <summary>
        /// Takes the UcasCourse records which are actually de-normalised course-campus info and turns them into
        /// proper British InstitutionCourses that have the campus info re-normalised into the .Schools list property.
        /// </summary>
        /// <param name="courseRecords">UcasCourse records</param>
        /// <param name="enrichmentMetadata"></param>
        /// <param name="pgdeCourses"></param>
        /// <returns></returns>
        public InstitutionCourses LoadCourses(IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseEnrichmentGetModel> enrichmentMetadata, IEnumerable<PgdeCourse> pgdeCourses)
        {
            var returnCourses = new InstitutionCourses();
            if (courseRecords.Count() > 0)
            {
                var organisationCourseRecord = courseRecords.First();//all the records in the list hold identical institution info so just get the first one
                returnCourses.InstitutionName = organisationCourseRecord.UcasInstitution.InstFull;
                returnCourses.InstitutionCode = organisationCourseRecord.InstCode;
                returnCourses.Courses = new List<Course>();

                // nb - this separator uses characters that are never used in inst codes - thus avoiding ambiguity
                var courseRecordGroupings = courseRecords.GroupBy(x => x.InstCode + "_@@_" + x.CrseCode);
                var enrichmentGroupings = enrichmentMetadata.ToLookup(x => x.InstCode + "_@@_" + x.CourseCode);
                var pgdeCoursesSimple = pgdeCourses.Select(x => x.InstCode + "_@@_" + x.CourseCode).ToList();

                foreach (var grouping in courseRecordGroupings)
                {
                    returnCourses.Courses.Add(LoadCourse(
                        grouping.ToList(), 
                        enrichmentGroupings[grouping.Key] ?? new List<UcasCourseEnrichmentGetModel>(),
                        pgdeCoursesSimple.Contains(grouping.Key)));
                }
            }

            return returnCourses;

        }
        
        /// <summary>
        /// Takes the list of campus info within a single course and maps it into
        /// our version of a course, with the schools being read from the campus info in the UcasCourse (aka campus)
        /// </summary>
        /// <param name="courseRecords">List of UcasCourse records for a single course (no really a course, not the course-campus combination in ucas-land)</param>
        /// <param name="enrichmentMetadata">Relevant enrichment for the courseRecords</param>
        /// <param name="isPgde"></param>
        /// <returns></returns>
        public Course LoadCourse(IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseEnrichmentGetModel> enrichmentMetadata, bool isPgde)
        {
            var returnCourse = new Course();
            if (courseRecords.Count() > 0)
            {
                // pick a reference course record for shared data such as the accrediting provider
                // users can edit only running locations in UCAS so give preference to using running locations
                // if there are any.
                var organisationCourseRecord = 
                    courseRecords.FirstOrDefault(x => x.Status != null && x.Status.ToLowerInvariant() == "r") 
                    ?? courseRecords.First();

                var bestEnrichment = enrichmentMetadata.SingleOrDefault(x => x.InstCode == organisationCourseRecord.InstCode && x.CourseCode == organisationCourseRecord.CrseCode);

                returnCourse.InstCode = organisationCourseRecord.InstCode;
                returnCourse.CourseCode = organisationCourseRecord.CrseCode;
                returnCourse.AccreditingProviderId = organisationCourseRecord.AccreditingProvider;
                if (organisationCourseRecord.AccreditingProviderInstitution != null)
                {
                    returnCourse.AccreditingProviderName = organisationCourseRecord.AccreditingProviderInstitution.InstFull;
                }
                returnCourse.AgeRange = organisationCourseRecord.Age;
                returnCourse.Name = organisationCourseRecord.CrseTitle;
                returnCourse.ProgramType = organisationCourseRecord.ProgramType;
                returnCourse.ProfpostFlag = organisationCourseRecord.ProfpostFlag;
                returnCourse.StudyMode = organisationCourseRecord.Studymode;
                returnCourse.StartDate = DateTime.TryParse($"{organisationCourseRecord.StartYear} {organisationCourseRecord.StartMonth}", out DateTime startDate) ? (DateTime?) startDate : null;
                var subjects = organisationCourseRecord.CourseCode.UcasCourseSubjects
                    .Select(x => x.UcasSubject.SubjectDescription).ToList();

                returnCourse.Subjects = string.Join(", ", subjects);
                returnCourse.Schools = GetSchoolsData(courseRecords);
                returnCourse.EnrichmentWorkflowStatus = bestEnrichment?.Status;

                returnCourse.Qualification = qualificationMapper.MapQualification(
                    organisationCourseRecord.ProfpostFlag,
                    subjectMapper.IsFurtherEducation(subjects),
                    isPgde);

                returnCourse.TypeDescription = returnCourse.GetCourseVariantType();

                const string both = "B";
                const string fullTime = "F";
                const string partTime = "P";
                var ucasVacancyStatusCodes = new HashSet<string> { both, fullTime, partTime };
                returnCourse.HasVacancies = returnCourse.Schools.Any(s => ucasVacancyStatusCodes.Contains(s.VacStatus));
            }

            return returnCourse;
        }
        private IEnumerable<School> GetSchoolsData(IEnumerable<UcasCourse> courseRecords)
        {
            var schools = courseRecords.Select(courseRecord => new School
            {
                LocationName = courseRecord.UcasCampus.CampusName,
                Address1 = courseRecord.UcasCampus.Addr1,
                Address2 = courseRecord.UcasCampus.Addr2,
                Address3 = courseRecord.UcasCampus.Addr3,
                Address4 = courseRecord.UcasCampus.Addr4,
                PostCode = courseRecord.UcasCampus.Postcode,
                ApplicationsAcceptedFrom = courseRecord.CrseOpenDate,
                Code = courseRecord.UcasCampus.CampusCode,
                Status = courseRecord.Status,
                VacStatus = courseRecord.VacStatus,
            }).ToList();
            //look for the main site and move it to the top of the list
            var main = schools.FirstOrDefault(s => s.Code == "-");
            if (main != null)
            {
                schools.Remove(main);
                schools.Insert(0, main);
            }

            return schools;
        }
    }
}
