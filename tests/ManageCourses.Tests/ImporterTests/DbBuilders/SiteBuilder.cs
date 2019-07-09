using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.Manages.Tests.ImporterTests.DbBuilders
{
    internal class SiteBuilder
    {
        private readonly Site _Site;

        public SiteBuilder()
        {
            _Site = new Site();
        }

        public static SiteBuilder Build()
        {
            var result = new SiteBuilder();
            return result;
        }

        public static implicit operator Site(SiteBuilder builder)
        {
            return builder._Site;
        }
    }
}
