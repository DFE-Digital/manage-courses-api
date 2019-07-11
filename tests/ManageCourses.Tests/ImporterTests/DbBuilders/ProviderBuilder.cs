using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.Manages.Tests.ImporterTests.DbBuilders;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders
{
    internal class ProviderBuilder
    {
        private readonly Provider _provider;

        public ProviderBuilder()
        {
            _provider = new Provider
            {
                Sites = new List<Site>
                {
                    new SiteBuilder(),
                }
            };
        }

        public ProviderBuilder WithCode(string providerCode)
        {
            _provider.ProviderCode = providerCode;
            return this;
        }

        public ProviderBuilder WithName(string providerName)
        {
            _provider.ProviderName = providerName;
            return this;
        }

        public static implicit operator Provider(ProviderBuilder builder)
        {
            return builder._provider;
        }

        public ProviderBuilder WithOptedIn()
        {
            return this;
        }

        public ProviderBuilder WithCourses(IList<Course> courses)
        {
            _provider.Courses = courses;
            return this;
        }

        public ProviderBuilder AddCourse(Course course)
        {
            if(_provider.Courses == null)
            {
                _provider.Courses = new List<Course>();
            }
            course.Provider = _provider;
            _provider.Courses.Add(course);
            return this;
        }

        public ProviderBuilder WithCycle(string year)
        {
            _provider.RecruitmentCycle = new RecruitmentCycle
            {
                Year = year,
            };
            return this;
        }

        public ProviderBuilder WithCycle(RecruitmentCycle recruitmentCycle)
        {
            _provider.RecruitmentCycle = recruitmentCycle;

            return this;
        }
    }
}
