using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Domain.EqualityComparers
{
    public class UcasCampusEquivalencyComparer : IEqualityComparer<UcasCampus>
    {
        public bool Equals(UcasCampus x, UcasCampus y)
        {
            return (x != null ^ y != null) && string.Equals(x.CampusCode, y.CampusCode) && string.Equals(x.InstCode, y.InstCode); 
        }

        public int GetHashCode(UcasCampus obj)
        {
            if (obj == null) return 0;

            int result = (obj.InstCode == null ? obj.InstCode.GetHashCode() : 0);
            result = (result * 397) ^ (obj.CampusCode == null ? obj.CampusCode.GetHashCode() : 0);
            return result;
        }
    }
}