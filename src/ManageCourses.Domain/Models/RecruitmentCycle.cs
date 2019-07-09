using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Models
{
    public class RecruitmentCycle
    {
        public const string CurrentYear = "2019";

        public int Id { get; set; }
        public string Year { get; set; }

        public ICollection<Provider> Providers { get; set; }
    }
}
