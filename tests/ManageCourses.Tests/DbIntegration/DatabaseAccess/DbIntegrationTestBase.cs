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
        protected ManageCoursesDbContext context;

        protected IList<EntityEntry> entitiesToCleanUp = new List<EntityEntry>();

        protected ManageCoursesDbContext GetContext()
        {
            return ContextLoader.GetDbContext(ContextLoader.IntegrationTestJson);
        }

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            context = GetContext();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        [SetUp]
        public void SetUp()
        {
            context = GetContext();
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (entitiesToCleanUp.Any())
            {
                foreach (var e in entitiesToCleanUp)
                {
                    e.State = EntityState.Deleted;
                }
                entitiesToCleanUp.Clear();
                context.SaveChanges();
            }
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            context = GetContext();
            context.Database.EnsureDeleted();
        }
    }
}