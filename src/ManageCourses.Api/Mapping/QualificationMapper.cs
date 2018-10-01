using GovUk.Education.SearchAndCompare.Domain.Models.Enums;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class QualificationMapper
    {
        public IncludesPgce MapQualification(string profpostFlag, bool isFurtherEducationCourse)
        {
            var isPg = !string.IsNullOrWhiteSpace(profpostFlag);
            if (isFurtherEducationCourse)
            {
                return isPg ? IncludesPgce.QtlsWithPgce : IncludesPgce.QtlsOnly;
            }

            return isPg ? IncludesPgce.Yes : IncludesPgce.No;
        }
    }
}
