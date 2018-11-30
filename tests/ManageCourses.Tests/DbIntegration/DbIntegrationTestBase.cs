using System;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Tests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
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
        protected DateTime MockTime;
        protected Mock<IClock> MockClock;
        protected TestConfigReader TestConfig;

        protected virtual bool EnableRetryOnFailure => true;

        [OneTimeSetUp]
        public virtual void BaseOneTimeSetUp()
        {
            Config = TestConfigBuilder.BuildTestConfig();
            Context = ContextLoader.GetDbContext(Config, EnableRetryOnFailure);
            TestConfig = new TestConfigReader(Config);
            Context.Database.EnsureDeleted();
            Context.Database.Migrate();
            MockClock = new Mock<IClock>();
            MockClock.SetupGet(c => c.UtcNow).Returns(() => MockTime);
            OneTimeSetup();
        }

        /// <summary>
        /// Optionally override this in derived test classes to do any fixture-specific setup.
        /// This will be run once the database has been reset and migrated.
        /// </summary>
        public virtual void OneTimeSetup() { }

        [SetUp]
        public void BaseSetup()
        {
            // get a fresh context every time to avoid stale in-memory data contaminating subsequent tests
            Context = ContextLoader.GetDbContext(Config, EnableRetryOnFailure);
            // Truncate (delete all data from) all tables, following FK constraints by virtue of CASCADE
            // https://stackoverflow.com/questions/2829158/truncating-all-tables-in-a-postgres-database/12082038#12082038
            Context.Database.ExecuteSqlCommandAsync(@"
                DO
                $func$
                BEGIN
                   EXECUTE
                   (SELECT 'TRUNCATE TABLE ' || string_agg(oid::regclass::text, ', ') || ' CASCADE'
                    FROM   pg_class
                    WHERE  relkind = 'r'  -- only tables
                    AND    relnamespace = 'public'::regnamespace
                   );
                END
                $func$;").Wait();

            // reset clock
            MockTime = new DateTime(1977, 1, 2, 3, 4, 5, 7);

            // allow derived tests to do their own setup
            Setup();
        }

        /// <summary>
        /// Optionally override this in derived test classes to do any test-specific setup.
        /// This will be run once the database cleared of any stale data from previous tests
        /// and a fresh <see cref="Context"/> obtained.
        /// </summary>
        protected virtual void Setup() { }
    }
}
