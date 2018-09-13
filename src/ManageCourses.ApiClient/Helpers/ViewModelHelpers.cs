using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.ApiClient.Helpers
{
    public static class ViewModelHelpers
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
            var result = "";

            if (string.IsNullOrWhiteSpace(course.ProgramType))
            {
                return result;
            }

            var route = course.ProgramType.ToLowerInvariant();

            switch (route)
            {
                case "he":
                {
                    result = "Higher education programme";
                    break;
                }
                case "sd":
                {
                    result = "School Direct training programme";
                    break;
                }
                case "ss":
                {
                    result = "School Direct (salaried) training programme";
                    break;
                }
                case "sc":
                {
                    result = "SCITT programme";
                    break;
                }
                case "ta":
                {
                    result = "PG Teaching Apprenticeship";
                    break;
                }
                default:
                {
                    break;
                }
            }

            return result;
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
