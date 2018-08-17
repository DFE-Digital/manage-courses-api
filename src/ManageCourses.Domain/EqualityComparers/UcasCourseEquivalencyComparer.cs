using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Domain.EqualityComparers
{
    public class CourseCodeEquivalencyComparer : IEqualityComparer<CourseCode>
    {
        public bool Equals(CourseCode x, CourseCode y)
        {
            return x != null && y != null && string.Equals(x.CrseCode, y.CrseCode) && string.Equals(x.InstCode, y.InstCode); 
        }

        public int GetHashCode(CourseCode obj)
        {
            if (obj == null) return 0;

            int result = (obj.InstCode == null ? obj.InstCode.GetHashCode() : 0);
            result = (result * 397) ^ (obj.CrseCode == null ? obj.CrseCode.GetHashCode() : 0);
            return result;
        }
    }
}