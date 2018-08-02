using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{

    /// <summary>
    /// Base class for test classes that connect to a real database
    /// </summary>
    public abstract class DbIntegrationTestBase
    {
        protected ManageCoursesDbContext Context;
        protected IConfigurationRoot Config;

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            Config = TestConfigBuilder.BuildTestConfig();
            Context = ContextLoader.GetDbContext(Config);
            Context.Database.EnsureDeleted();
            Context.Database.Migrate();
        }

        [SetUp]
        public void SetUp()
        {
        }
    }
}
