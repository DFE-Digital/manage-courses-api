using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class McUserByEmailTests
    {
        const string FredExampleOrg = "fred@example.org";
        private const string WilmaExampleOrg = "wilma@example.org";
        private const int FredId = 11;
        private const int WilmaId = 12;
        private IQueryable<McUser> _mcUsers;

        [SetUp]
        public void Setup()
        {
            _mcUsers = new List<McUser> {
                new McUser {Id = FredId, Email = FredExampleOrg },
                new McUser {Id = WilmaId, Email = WilmaExampleOrg },
            }.AsQueryable();
        }

        [Test]
        [TestCase(FredExampleOrg, FredId)]
        [TestCase(WilmaExampleOrg, WilmaId)]
        [TestCase("no-one@example.org", null)]
        public void Test_McUser_ByEmail(string email, int? expectedId)
        {
            TestByEmailVariation(email, expectedId);
            TestByEmailVariation(email.ToUpper(), expectedId);
        }

        private void TestByEmailVariation(string email, int? exectedId)
        {
            var mcUser = _mcUsers.ByEmail(email).SingleOrDefault();

            if (exectedId != null)
            {
                mcUser.Should().NotBeNull();
                mcUser.Id.Should().Be(exectedId);
            }
            else
            {
                mcUser.Should().BeNull();
            }
        }
    }
}
