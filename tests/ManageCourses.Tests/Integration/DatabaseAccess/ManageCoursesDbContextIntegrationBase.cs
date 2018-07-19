using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Api;

namespace GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess
{

    public class ManageCoursesDbContextIntegrationBase
    {
        protected ManageCoursesDbContext context;

        protected IList<EntityEntry> entitiesToCleanUp = new List<EntityEntry>();

        protected ManageCoursesDbContext GetContext()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("integration-tests.json")
                .Build();

            var options = new DbContextOptionsBuilder<ManageCoursesDbContext>()
                .UseNpgsql(Startup.GetConnectionString(config))
                .Options;

            return new ManageCoursesDbContext(options);
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