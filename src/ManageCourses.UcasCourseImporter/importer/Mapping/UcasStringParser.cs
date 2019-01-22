using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.UcasCourseImporter.Mapping
{
    public static class UcasStringParser
    {
        public static DateTime? GetDateTimeFromString(string dateToConvert)
        {
            if (DateTime.TryParse(dateToConvert, out var returnDate))
            {
                return returnDate;
            }
            else
            {
                return null;
            }
        }

    }
}
