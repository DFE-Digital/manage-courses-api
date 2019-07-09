using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using System;
using System.Linq;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders
{
    internal class CourseBuilder
    {
        private readonly Course _course;

        public CourseBuilder()
        {
            _course = new Course();
            _course.CourseSites = new List<CourseSite>();
            _course.CourseSubjects = new List<CourseSubject>
            {
                new CourseSubject
                {
                    Subject = new Subject
                    {
                        SubjectName = "Primary",
                    },
                }
            };
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

        public CourseBuilder WithProvider(Provider Provider)
        {
            _course.Provider = Provider;
            return this;
        }

        public CourseBuilder Update(Action<Course> action)
        {
            action(_course);

            return this;
        }

        public string GetCode()
        {
            return _course.CourseCode;
        }

        public CourseBuilder AddCourseSites(params CourseType[] courseTypes)
        {
            var result = new List<CourseSite>(_course.CourseSites) {};
            result.AddRange(courseTypes.Select(c => (CourseSite)CourseSiteBuilder.Build(c, _course)));
            _course.CourseSites = result;
            return this;
        }
        public CourseBuilder AddCourseSite(CourseType courseType)
        {
            return AddCourseSites(new[] {courseType});
        }

        public static CourseBuilder Build( string courseCode, CourseType[] additionalCoursesSites, Provider withProvider = null)
        {
            var result = new CourseBuilder()
                .WithCode(courseCode)
                .WithProvider(withProvider)
                .AddCourseSites(additionalCoursesSites);

            return result;
        }
        public static CourseBuilder Build(CourseType courseType, Provider withProvider = null)
        {
            var result = new CourseBuilder()
                .WithCode(courseType.ToString())
                .WithProvider(withProvider)
                .AddCourseSite(courseType);

            return result;
        }
    }
}
