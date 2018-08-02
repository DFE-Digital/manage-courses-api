using System.Collections.Generic;
using System.Linq;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration.DatabaseAccess
{

    /// <summary>
    /// Base class for test classes that connect to a real database
    /// </summary>
    public class DbIntegrationTestBase
    {
        protected ManageCoursesDbContext Context;
        protected IList<EntityEntry> EntitiesToCleanUp = new List<EntityEntry>();

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
            Context = GetContext();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (EntitiesToCleanUp.Any())
            {
                foreach (var e in EntitiesToCleanUp)
                {
                    e.State = EntityState.Deleted;
                }
                EntitiesToCleanUp.Clear();
                Context.SaveChanges();
            }
        }
    }
}
