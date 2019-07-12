using FluentAssertions;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.ImporterTests.DbBuilders;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    public class DbContextTests : DbIntegrationTestBase
    {
        private const string ProviderCode = "AB1";
        private const string ProviderName = "Aberdeen University";

        protected override void Setup()
        {
            Context.Providers.Add(new ProviderBuilder()
                .WithCode(ProviderCode)
                .WithName(ProviderName)
                .WithCycle("2019"));
            Context.Save();
        }

        [Test]
        public void TestGetProviderName()
        {
            Context.GetProviderName(ProviderCode).Should().Be(ProviderName);
        }

        [Test]
        public void TestGetProviderName_ReturnsNull_Empty()
        {
            Context.GetProviderName("").Should().BeNull("Code was an empty string");
            Context.GetProviderName("  ").Should().BeNull("Code was only whitespace");
        }

        [Test]
        public void TestGetProviderName_ReturnsNull_ForNullCode()
        {
            Context.GetProviderName(null).Should().BeNull();
        }

        [Test]
        public void TestGetProviderName_Throws_ForUnknownCode()
        {
            Assert.Throws<NotFoundException>(() =>
            {
                Context.GetProviderName("X0X").Should().BeNull();
            });
        }
    }
}
