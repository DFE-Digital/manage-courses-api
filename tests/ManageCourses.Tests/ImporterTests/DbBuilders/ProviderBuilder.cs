using System.Collections.Generic;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders
{
    internal class ProviderBuilder
    {
        private readonly Provider _provider;

        public ProviderBuilder()
        {
            _provider = new Provider();
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
            _provider.OptedIn = true;
            return this;
        }

        public ProviderBuilder WithCourses(IList<Course> courses)
        {
            _provider.Courses = courses;
            return this;
        }
    }
}
