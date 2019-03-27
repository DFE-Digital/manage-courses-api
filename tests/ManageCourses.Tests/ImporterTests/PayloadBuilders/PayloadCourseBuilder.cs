using GovUk.Education.ManageCourses.Xls.Domain;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.PayloadBuilders
{
    internal class PayloadCourseBuilder
    {
        private readonly UcasCourse _course;

        public PayloadCourseBuilder()
        {
            _course = new UcasCourse();
        }

        public static implicit operator UcasCourse(PayloadCourseBuilder builder)
        {
            return builder._course;
        }

        public PayloadCourseBuilder WithInstCode(string instCode)
        {
            _course.InstCode = instCode;
            return this;
        }

        public PayloadCourseBuilder WithCrseCode(string courseCode)
        {
            _course.CrseCode = courseCode;
            return this;
        }

        public PayloadCourseBuilder WithStatus(string status)
        {
            _course.Status = status;
            return this;
        }

        public PayloadCourseBuilder WithCampusCode(string campusCode)
        {
            _course.CampusCode = campusCode;
            return this;
        }

        public PayloadCourseBuilder WithName(string courseTitle)
        {
            _course.CrseTitle = courseTitle;
            return this;
        }

        public PayloadCourseBuilder WithAccreditingProvider(string accreditingProviderCode)
        {
            _course.AccreditingProvider = accreditingProviderCode;
            return this;
        }
    }
}
