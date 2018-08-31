using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Services;
using GovUk.Education.ManageCourses.Api.Services.Data;
using GovUk.Education.ManageCourses.Domain.DatabaseAccess;
using GovUk.Education.ManageCourses.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GovUk.Education.ManageCourses.Tests.UnitTesting
{
    [TestFixture]
    public class DataServiceTests
    {
        private Mock<IManageCoursesDbContext> _contextMock;
        private Mock<IEnrichmentService> _enrichmentServiceMock;
        private Mock<IDataHelper> _dataHelperMock;
        private DataService _dataService;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IManageCoursesDbContext>();
            _enrichmentServiceMock = new Mock<IEnrichmentService>();
            _dataHelperMock = new Mock<IDataHelper>();
            _dataService = new DataService(_contextMock.Object, _enrichmentServiceMock.Object, _dataHelperMock.Object, new Mock<ILogger<DataService>>().Object);
        }

        /// <summary>
        /// Test status generation with no org enrichment present
        /// </summary>
        [Test]
        public void Test_Initial_OrgStatus()
        {
            const string email = "roger@example.org";
            const string instCode = "BAT4";
            _contextMock.Setup(c => c.GetUserOrganisation(email, instCode)).Returns(new McOrganisationInstitution
            {
                UcasInstitution = new UcasInstitution(),
            });
            var org = _dataService.GetOrganisationForUser(email, instCode);
            org.Should().NotBeNull();
            org.EnrichmentWorkflowStatus.Should().BeNull();
        }
    }
}
