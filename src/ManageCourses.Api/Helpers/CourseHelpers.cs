using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;

namespace GovUk.Education.ManageCourses.Api.Helpers
{
    public static class CourseHelpers
    {
        public static string GetCourseVariantType(this Course course, IncludesPgce mappedQualification)
        {
            string result;
            
            switch(mappedQualification)
            {
                case IncludesPgce.No:
                    result = "QTS";
                    break;
                case IncludesPgce.Yes:
                    result = "PGCE with QTS";
                    break;
                case IncludesPgce.QtlsWithPgce:
                    result = "PGCE";
                    break;
                case IncludesPgce.QtlsWithPgde:
                    result = "PGDE";
                    break;
                case IncludesPgce.QtsWithPgde:
                    result = "PGDE with QTS";
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(mappedQualification)} is unknown value: {mappedQualification}");
            }

            if ((!string.IsNullOrWhiteSpace(result)) && string.Equals(course.StudyMode, "B", StringComparison.InvariantCultureIgnoreCase))
            {
                result += ", ";
            }
            else
            {
                result += " ";
            }

            result += GetStudyModeText(course.StudyMode);

            result += string.Equals(course.ProgramType, "ss", StringComparison.InvariantCultureIgnoreCase)
                ? " with salary"
                : "";

            return result;
        }
        public static string GetRoute(this Course course)
        {
            if (string.IsNullOrWhiteSpace(course.ProgramType))
            {
                return "";
            }

            var route = course.ProgramType.ToLowerInvariant();

            switch (route)
            {
                case "he":
                    return "Higher education programme";
                case "sd":
                    return "School Direct training programme";
                case "ss":
                    return "School Direct (salaried) training programme";
                case "sc":
                    return "SCITT programme";
                case "ta":
                    return "PG Teaching Apprenticeship";
                default:
                    return "";
            }
        }
        private static string GetStudyModeText(string studyMode)
        {
            var returnString = string.Empty;

            if (string.IsNullOrWhiteSpace(studyMode))
            {
                return returnString;//TODO clarify what happens if study mode is missing
            }

            if (studyMode.Equals("F", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "full time";
            }
            else if (studyMode.Equals("P", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "part time";
            }
            else if (studyMode.Equals("B", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "full time or part time";
            }

            return returnString;
        }
    }
}
