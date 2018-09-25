using System;
using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Data;
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
        public InstitutionCourses LoadCourses(IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseEnrichmentGetModel> enrichmentMetadata)
        {
            var returnCourses = new InstitutionCourses();
            if (courseRecords.Count() > 0)
            {
                var organisationCourseRecord = courseRecords.First();//all the records in the list hold identical institution info so just get the first one
                returnCourses.InstitutionName = organisationCourseRecord.UcasInstitution.InstFull;
                returnCourses.InstitutionCode = organisationCourseRecord.InstCode;
                returnCourses.Courses = new List<Course>();
                foreach (var courseCode in courseRecords.Select(c => c.CrseCode).Distinct())
                {
                    returnCourses.Courses.Add(LoadCourse(courseRecords.Where(c => c.CrseCode == courseCode).ToList(), enrichmentMetadata));
                }
            }

            return returnCourses;

        }
        
        public Course LoadCourse(IEnumerable<UcasCourse> courseRecords, IEnumerable<UcasCourseEnrichmentGetModel> enrichmentMetadata)
        {
            var returnCourse = new Course();
            if (courseRecords.Count() > 0)
            {
                var organisationCourseRecord = courseRecords.First();//all the records in the list hold identical institution info so just get the first one

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
                var subjects = organisationCourseRecord.CourseCode.UcasCourseSubjects
                    .Select(x => x.UcasSubject.SubjectDescription).ToList();

                returnCourse.Subjects = string.Join(", ", subjects);
                returnCourse.Schools = GetSchoolsData(courseRecords);
                returnCourse.EnrichmentWorkflowStatus = bestEnrichment?.Status;
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
