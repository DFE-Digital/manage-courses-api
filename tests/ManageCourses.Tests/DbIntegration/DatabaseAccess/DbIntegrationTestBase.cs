using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration.DatabaseAccess
{

    /// <summary>
    /// Base class for test classes that connect to a real database
    /// </summary>
    public abstract class DbIntegrationTestBase
    {
        protected ManageCoursesDbContext Context;

        protected ManageCoursesDbContext GetContext()
        {
            var config = TestConfigBuilder.BuildTestConfig();
            return ContextLoader.GetDbContext(config);
        }

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            Context = GetContext();
            Context.Database.EnsureDeleted();
            Context.Database.Migrate();
        }

        [SetUp]
        public void SetUp()
        {
        }
    }
}
