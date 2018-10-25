using System;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;

namespace GovUk.Education.ManageCourses.UcasCourseImporter.Mapping
{
    public class QualificationMapper
    {
        public CourseQualification MapQualification(string profpostFlag, bool isFurtherEducationCourse, bool isPgde)
        {
            if (isPgde)
            {
                return isFurtherEducationCourse ? CourseQualification.QtlsWithPgde : CourseQualification.QtsWithPgde;
            }

            if (isFurtherEducationCourse)
            {
                return CourseQualification.QtlsWithPgce;
            }

            var isPg = !string.IsNullOrWhiteSpace(profpostFlag);
            return isPg ? CourseQualification.QtsWithPgce : CourseQualification.Qts;
        }
    }
}
