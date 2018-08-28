namespace GovUk.Education.ManageCourses.Api.Model
{
    public class CourseEnrichmentModel
    {
        public string AboutCourse { get; set; }
        public string InterviewProcess { get; set; }
        public string HowSchoolPlacementsWork { get; set; }

        public string CourseLength { get; set; }
        public decimal? FeeUkEu { get; set; }
        public decimal? FeeInternational { get; set; }
        public string SalaryDetails { get; set; }
        public string FeeDetails { get; set; }
        public string FinancialSupport { get; set; }

        public string Qualifications { get; set; }
        public string PersonalQualities { get; set; }
        public string OtherRequirements { get; set; }
    }
}
