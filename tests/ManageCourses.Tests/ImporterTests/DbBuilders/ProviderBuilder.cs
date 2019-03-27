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
        public Provider WithCode(string providerCode)
        {
            _provider.ProviderCode = providerCode;
            return _provider;
        }

        public static implicit operator Provider(ProviderBuilder builder)
        {
            return builder._provider;
        }
    }
}
