using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;

namespace GovUk.Education.ManageCourses.Api.Mapping
{
    public class QualificationMapper
    {
        public IncludesPgce MapQualification(string profpostFlag)
        {
            return string.IsNullOrWhiteSpace(profpostFlag) ? IncludesPgce.No : IncludesPgce.Yes;
        }
    }
}
