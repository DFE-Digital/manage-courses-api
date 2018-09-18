using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;

namespace GovUk.Education.ManageCourses.Api.Services.Publish.Helpers
{
    public static class CourseHelpers
    {
        public static string GetCourseVariantType(this Course course)
        {
            var result = string.IsNullOrWhiteSpace(course.ProfpostFlag) ? "QTS" : "PGCE with QTS";

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
