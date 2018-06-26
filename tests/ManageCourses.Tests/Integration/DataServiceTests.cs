using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using GovUk.Education.ManageCourses.Tests.Integration.DatabaseAccess;
using GovUk.Education.ManageCourses.Api.Data;

namespace GovUk.Education.ManageCourses.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    [Category("Integration_DB")]
    [Explicit]
    public class DataServiceTests : ManageCoursesDbContextIntegrationBase
    {
        public IDataService Subject = null;

        [SetUp]
        public void Setup()
        {
            Subject = new DataService(this.GetContext());
        }

        [Test]
        public void GetCoursesForUser_isEmpty()
        {
            var email = "email@test.com";

            var result = Subject.GetCoursesForUser(email);

            CollectionAssert.IsEmpty(result);

        }
    }
}
