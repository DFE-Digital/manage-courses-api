using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders
{
    internal class CourseBuilder
    {
        private readonly Course _course;

        public CourseBuilder()
        {
            _course = new Course();
        }

        public CourseBuilder WithCode(string courseCode)
        {
            _course.CourseCode = courseCode;
            return this;
        }

        public CourseBuilder WithName(string courseName)
        {
            _course.Name = courseName;
            return this;
        }

        public static implicit operator Course(CourseBuilder builder)
        {
            return builder._course;
        }

        public CourseBuilder WithAccreditingProvider(Provider accreditingProvider)
        {
            _course.AccreditingProvider = accreditingProvider;
            return this;
        }
    }
}
