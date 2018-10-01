using GovUk.Education.SearchAndCompare.Domain.Models.Enums;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class QualificationMapper
    {
        public IncludesPgce MapQualification(string profpostFlag, bool isFurtherEducationCourse)
        {
            if (isFurtherEducationCourse)
            {
                return IncludesPgce.QtlsWithPgce;
            }

            var isPg = !string.IsNullOrWhiteSpace(profpostFlag);
            return isPg ? IncludesPgce.Yes : IncludesPgce.No;
        }
    }
}
