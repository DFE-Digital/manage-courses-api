using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Mapping;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.DbIntegration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class PgdeWhitelistTests : DbIntegrationTestBase
    {
        private PgdeCourse _whitelistedPgdeCourse;
        private PgdeWhitelist _pgdeWhitelist;

        protected override void Setup()
        {
            _pgdeWhitelist = new PgdeWhitelist(Context);

            _whitelistedPgdeCourse = new PgdeCourse
            {
                InstCode = "INST4",
                CourseCode = "CRSE7",
            };
            Context.PgdeCourses.Add(_whitelistedPgdeCourse);
            Context.SaveChanges();
        }

        [Test]
        public void InstCodeIsntInList()
        {
            _pgdeWhitelist.IsPgde("UNI3", _whitelistedPgdeCourse.CourseCode).Should().Be(false, "the course isn't whitelisted in the database");
        }

        [Test]
        public void CourseCodeIsntInList()
        {
            _pgdeWhitelist.IsPgde(_whitelistedPgdeCourse.InstCode, "Fooey4").Should().Be(false, "the course isn't whitelisted in the database");
        }

        [Test]
        public void LowerCaseMatches()
        {
            _pgdeWhitelist.IsPgde(_whitelistedPgdeCourse.InstCode.ToLower(), _whitelistedPgdeCourse.CourseCode.ToLower()).Should().Be(true, "the course is whitelisted in the database");
        }

        [Test]
        public void CourseIsInList()
        {
            _pgdeWhitelist.IsPgde(_whitelistedPgdeCourse.InstCode, _whitelistedPgdeCourse.CourseCode).Should().Be(true, "the course is whitelisted in the database");
        }
    }
}
